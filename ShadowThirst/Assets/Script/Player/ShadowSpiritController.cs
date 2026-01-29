using System.Collections;
using UnityEngine;

public class ShadowSpiritController : MonoBehaviour
{
    public static ShadowSpiritController Instance { get; private set; }   // Singleton instance for global, controlled access

    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;       //The maximum amount of health the player can have
    [SerializeField] private float initialHealth = 100f;   // Health value when the game starts
    [SerializeField] private float healPoint = 10f;        // Amount healed when player uses healing ability/item
    [SerializeField] private float damagePoint = 20f;      // Base damage amount dealt to player when hit


    [Header("Movement Settings")] 
    [SerializeField] private float movementSpeed;                           // Base speed at which the player character moves
    [SerializeField] private VirtualJoystickController joystickController;  // Reference to the on-screen joystick for movement input
    [SerializeField] private Rigidbody2D rb2D;                              // Physics component used for character movement
    [SerializeField] private float speedBoostMultiplier;                    // Multiplier applied to movement speed during speed boost
    [SerializeField] private float speedEffectTimer;                        // How long (in seconds) the speed boost effect lasts


    [Header("Time Intervals")]
    [SerializeField] private float safeDuration = 30f;           // Starting phase where player doesn't lose health over time
    [SerializeField] private float phaseOneEndTime = 120f;       // Time when phase 1 ends (easier phase)
    [SerializeField] private float phaseTwoEndTime = 240f;       // Time when phase 2 ends (medium difficulty)
    [SerializeField] private float phaseThreeEndTime = 500f;     // Time when phase 3 ends (hard difficulty)

    [Header("Decay Settings")]
    [SerializeField] private float firstPhaseInterval = 5.0f;      // How often (in seconds) health decays in phase 1
    [SerializeField] private float secondPhaseInterval = 2.0f;     // How often (in seconds) health decays in phase 2
    [SerializeField] private float thirdPhaseInterval = 1.0f;      // How often (in seconds) health decays in phase 3
    [SerializeField] private float fourthPhaseInterval = 0.5f;     // How often (in seconds) health decays in final phase


    [Header("Decay Amounts")]
    [SerializeField] private float normalDecayAmount = 1f;             // Base amount of health lost per decay tick
    [SerializeField] private float finalPhaseDecayMultiplier = 2f;     // Multiplier applied during the final (most dangerous) phase

    [Header("Shield Settings")]
    [SerializeField] private float shieldLife = 10f;   // How long (in seconds) the shield stays active once used
    private bool haveShield = false;                   // Whether the player has collected/purchased a shield

    [Header("Animator Reference")]
    [SerializeField] private Animator playerAnimator;    // Controls all player animations (walking, idle, hurt, death)
    [SerializeField] private AttackRange attackRange;
 
    [Header("Reference")]
    [SerializeField] private EnemyController enemyController;     // Enemy linked to this player

    private float startTime;                            // Timestamp when gameplay started
    private float healthDecayTimer = 0f;                // Accumulates time to trigger tick-based health decay
    private bool isMoving = false;                      // True when player is currently moving, false when idle
    private Vector2 previousDirection = Vector2.zero;   // Last movement direction, used for facing when idle
    private float originalSpeed = 0f;                   // Stores the base movement speed before any boosts
    private float maxBoostSpeed = 0f;                   // Maximum possible speed during speed boost (original * multiplier)
    private bool isSpeedBoosted = false;                // Whether the speed boost effect is currently active
    private Coroutine currentSpeedBoostCoroutine;       // Reference to the active speed boost timer coroutine
    private Coroutine currentShieldCoroutine;           // Reference to the active shield timer coroutine
    private Coroutine deathCoroutine;                   // Reference to the death/destruction sequence coroutine
    
    


    #region Public State (Read-Only)
    // Public getter with private setter provides controlled access to shield status
    public bool IsShieldActivated { get; private set; }   // Indicates whether the player is currently protected from damage
    public bool IsDead { get; private set; }              // Indicates whether the player is dead (prevents duplicate logic)
    public float HealthPoint { get; private set; }        // Current health value (clamped between 0 and maxHealth)
    public float MaxHealth => maxHealth;                  // Exposes max health safely
    public float DamagePoint => damagePoint;              // Expose damage point safely
    public float HealPoint => healPoint;                  // Expose heal point safely
    public bool IsSpeedBoosted => isSpeedBoosted;         // Expose speed boost status safely
    public float PhaseOneEndTime => phaseOneEndTime;      // Expose phase one time safely
    public float PhaseTwoEndTime => phaseTwoEndTime;      // Expose phase two time safely
    public float PhaseThreeEndTime => phaseThreeEndTime;  // Expose phase three time safely
    public float StartTime => startTime;                  // Expose game start time safely

    public bool HaveShield => haveShield;
    #endregion

    private void Awake()
    {
        // Enforce singleton pattern (only one player instance allowed)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Cache Rigidbody reference for movement and death handling
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startTime = Time.time;   // Record gameplay start time

        HealthPoint = Mathf.Clamp(initialHealth, 0f, maxHealth);      // Initialize health with clamping
        originalSpeed = movementSpeed;     // Store original speed for resetting later
        maxBoostSpeed = movementSpeed * speedBoostMultiplier;        // Calculate maximum boosted speed
    }

    private void Update()
    {
        if (IsDead) return;     // Stop all logic if player is dead

        HandleBloodLoss();      // Handle blood loss over time
    }

    private void FixedUpdate()
    {
        HandleMovement();     // Handle physics-based movement in FixedUpdate
    }


    // Processes player movement based on joystick input
    private void HandleMovement()
    {
        //Early exit if references are missing or player is dead
        if (rb2D == null || joystickController == null || IsDead) return;

        Vector2 input = joystickController.Direction;

        //Only move if there's significant input
        if (input.sqrMagnitude > 0.001f)
        {
            //Move the character using physics-based movement
            rb2D.MovePosition(rb2D.position + input * movementSpeed * Time.fixedDeltaTime);
        }

        UpdateSprite(input);

        previousDirection = input;
    }


    // Updates sprite animations based on movement direction
    private void UpdateSprite(Vector2 input)
    {
        // Calculate absolute values for comparison
        float absX = Mathf.Abs(input.x);
        float absY = Mathf.Abs(input.y);
        float prevX = Mathf.Abs(previousDirection.x);
        float prevY = Mathf.Abs(previousDirection.y);

        isMoving = input.sqrMagnitude > 0.001f;


        // Handle animation states when moving
        if (isMoving)
        {
            if ( absX > absY && playerAnimator != null)
            {
                playerAnimator.SetFloat("X", input.x);
                playerAnimator.SetFloat("Y", 0f);
            }

            if ( absY > absX && playerAnimator != null)
            {
                playerAnimator.SetFloat("X", 0f);
                playerAnimator.SetFloat("Y", input.y);
            }
        }
        else
        {
            if ( prevX > prevY && playerAnimator != null)
            {
                playerAnimator.SetFloat("X", previousDirection.x);
                playerAnimator.SetFloat("Y", 0f);
            }

            if ( prevY > prevX && playerAnimator != null)
            {
                playerAnimator.SetFloat("X", 0f);
                playerAnimator.SetFloat("Y", previousDirection.y);
            }
        }


        // Update the "Moving" parameter in animator
        if (playerAnimator != null)
            playerAnimator.SetBool("Moving", isMoving);
    }


    // Controls tick-based blood loss logic based on elapsed gameplay time.
    private void HandleBloodLoss()
    {
        float elapsedSinceStart = Time.time - startTime;

        // Do not apply blood loss during safe period
        if (elapsedSinceStart < safeDuration) return;

        // Accumulate time for tick-based decay
        healthDecayTimer += Time.deltaTime;

        float currentInterval = GetCurrentDecayInterval(elapsedSinceStart);

        // Apply health loss only when the current interval is reached
        if (healthDecayTimer >= currentInterval)
        {
            healthDecayTimer = 0f;

            float decayAmount = GetDecayAmount(elapsedSinceStart);
            UpdateHealth(-decayAmount);
        }
    }

    // Returns the decay interval (seconds between ticks) based on elapsed time.
    private float GetCurrentDecayInterval(float elapsedTime)
    {
        if (elapsedTime < phaseOneEndTime) return firstPhaseInterval;
        if (elapsedTime < phaseTwoEndTime) return secondPhaseInterval;
        if (elapsedTime < phaseThreeEndTime) return thirdPhaseInterval;
        return fourthPhaseInterval;
    }

    // Returns the amount of health lost per tick based on elapsed time.
    private float GetDecayAmount(float elapsedTime)
    {
        float baseDecay = normalDecayAmount;

        // No decay in safe phase
        if (elapsedTime < safeDuration)
            return 0f;

        // Early decay phase (after safe period, before phaseOneEndTime)
        if (elapsedTime < phaseOneEndTime)
            return baseDecay * 0.5f;

        // Transitional phase (between early and mid)
        if (elapsedTime < phaseTwoEndTime)
            return baseDecay;

        // Late-game pressure increase
        if (elapsedTime < phaseThreeEndTime)
            return baseDecay * 1.5f;

        return baseDecay * finalPhaseDecayMultiplier;      // Final phase: highest danger  
    }

    // Modifies health safely and checks for death condition.
    private void UpdateHealth(float amount)
    {
        if (IsDead) return;

        HealthPoint = Mathf.Clamp(HealthPoint + amount, 0f, maxHealth);

        if (HealthPoint <= 0f)
        {
            DieFromBloodThirst();
        }
    }


    // Applies damage unless shielded or already dead.
    public void TakeDamage(float damage)
    {
        Vector2 input = Vector2.zero;

        if (joystickController != null)
        {
            input = joystickController.Direction;
        }
        else
        {
            Debug.LogWarning("JoystickController reference missing in TakeDamage");
        }

        if (IsShieldActivated || IsDead) return;

        UpdateHealth(-damage);

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Hurt");
        }

        UpdateSprite(input);
    }


    // Heals the player (clamped internally).
    public void Heal(float amount)
    {

        UpdateHealth(amount);

    }


    // Activates temporary speed boost effect
    public void SpeedBoost()
    {
        if (currentSpeedBoostCoroutine != null)
        {
            StopCoroutine(currentSpeedBoostCoroutine);
        }

        movementSpeed = maxBoostSpeed;
        isSpeedBoosted = true;

        currentSpeedBoostCoroutine = StartCoroutine(ResetSpeedAfterDelay(speedEffectTimer));
    }


    // Coroutine to reset speed after boost duration expires
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        ResetSpeed();
    }


    // Resets movement speed to original value and cleans up coroutine
    private void ResetSpeed()
    {
        movementSpeed = originalSpeed;
        isSpeedBoosted = false;

        if (currentSpeedBoostCoroutine != null)
        {
            StopCoroutine(currentSpeedBoostCoroutine);
            currentSpeedBoostCoroutine = null;
        }
    }

    public void Attack()
    {
        if (IsDead) return;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Attacking");
        }
    }

    private void KillEnemy()
    {
        EnemyController enemyController = attackRange.GetClosestEnemy();
        // Called when player defeats enemy
        if (enemyController != null)
        {
            enemyController.Die();
        }
    }

    // Common death handling logic for all death types
    private void DieShadow()
    {
        if (IsDead) return;

        IsDead = true;

        // Disable physics simulation so the character stops interacting with the world
        if (rb2D != null)
        {
            rb2D.simulated = false;
        }

        Debug.Log($"Game Time from start:  {Time.time - startTime}");

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Dead", IsDead);
        }

        if (deathCoroutine != null)
        {
            StopCoroutine(deathCoroutine);
        }

        deathCoroutine = StartCoroutine(DestroyAfterDelay(2f));
    }

    // Handles death caused by blood depletion.
    private void DieFromBloodThirst()
    {
        DieShadow();
    }


    // Handles instant death caused by light exposure.
    public void DieFromLight()
    {
        HealthPoint = 0f;

        DieShadow();
    }


    // Coroutine to destroy game object after specified delay
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }


    // Player collects a shield for later use
    public void CollectShield()
    {
        if (haveShield) return;
        haveShield = true;
    }


    // Activates collected shield for temporary protection
    public void ActivateShield()
    {
        if (!haveShield) return;

        if (currentShieldCoroutine != null)
        {
            StopCoroutine(currentShieldCoroutine);
        }

        IsShieldActivated = true;
        currentShieldCoroutine = StartCoroutine(DeactivateShieldAfterDelay(shieldLife));
    }


    // Coroutine to deactivate shield after its duration expires
    private IEnumerator DeactivateShieldAfterDelay(float shieldLife)
    {
        yield return new WaitForSeconds(shieldLife);
        IsShieldActivated = false;
        haveShield = false;
        currentShieldCoroutine = null;
    }

    // Cleans up singleton reference when object is destroyed
    private void OnDestroy()
    {
        // Clear singleton reference when destroyed
        if (Instance == this)
            Instance = null;
    }
}