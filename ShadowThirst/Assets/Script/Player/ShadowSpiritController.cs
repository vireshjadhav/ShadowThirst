using System;
using UnityEngine;

public class ShadowSpiritController : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float initialHealth = 100f;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private VirtualJoystickController joystickController;
    [SerializeField] private Rigidbody2D rb2D;

    [Header("Time Intervals")]
    //Duration during which no blood loss happens
    [SerializeField] private float safeDuration = 30f;      

    //Duration during which no blood loss happens
    [SerializeField] private float phaseOneEndTime = 120f;
    [SerializeField] private float phaseTwoEndTime = 240f;
    [SerializeField] private float phaseThreeEndTime = 500f; 

    [Header("Decay Settings")]
    // Time interval between health decay ticks for each phase
    [SerializeField] private float firstPhaseInterval = 5.0f;
    [SerializeField] private float secondPhaseInterval = 2.0f;
    [SerializeField] private float thirdPhaseInterval = 1.0f;
    [SerializeField] private float fourthPhaseInterval = 0.5f;

    [Header("Decay Amounts")]
    [SerializeField] private float normalDecayAmount = 1f;     // Base amount of health lost per decay tick
    [SerializeField] private float finalPhaseDecayMultiplier = 2f;     // Multiplier applied during the final (most dangerous) phase

    // Singleton instance for global, controlled access
    public static ShadowSpiritController Instance { get; private set; }

    private float startTime;     // Timestamp when gameplay started
    private float healthDecayTimer = 0f;       // Accumulates time to trigger tick-based health decay

    #region Public State(Read-Only)
    //Public getter with private setter provides controlled access to shield status
    public bool IsShielded { get; private set; }     // Indicates whether the player is currently protected from damage
    public bool IsDead { get; private set; }      // Indicates whether the player is dead (prevents duplicate logic)
    public float HealthPoint { get; private set; }     // Current health value (clamped between 0 and maxHealth)
    public float MaxHealth => maxHealth;      // Exposes max health safely

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
        HealthPoint = Mathf.Clamp(initialHealth, 0f, maxHealth);      // Initialize health safely
    }

    private void Update()
    {
        if (IsDead) return;     // Stop all logic if player is dead

        HandleBloodLoss();      // Handle blood loss over time
    }

    private void FixedUpdate()
    {
        //Early exit if references are missing or player is dead
        if ( rb2D  == null || joystickController == null  || IsDead) return;

        Vector2 input = joystickController.Direction;
        //Only move if there's significant input
        if (input.sqrMagnitude > 0.001f)
        {
            //Move the character using physics-based movement
            rb2D.MovePosition(rb2D.position + input * movementSpeed * Time.fixedDeltaTime);
        }
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

        // Final phase: highest danger
        if (elapsedTime < phaseOneEndTime)
            return baseDecay * 0.5f;

        // Late-game pressure increase
        if (elapsedTime < phaseTwoEndTime)
            return baseDecay;

        // Early decay phase
        if (elapsedTime < phaseThreeEndTime)
            return baseDecay * 1.5f;

        // Transitional phase (between early and mid)
        return baseDecay * finalPhaseDecayMultiplier;
    }

    // Modifies health safely and checks for death condition.
    public void UpdateHealth(float amount)
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
        if (IsShielded || IsDead) return;
        UpdateHealth(-damage);
    }


    // Heals the player (clamped internally).
    public void Heal (float amount)
    {
        UpdateHealth(amount);
    }


    // Handles death caused by blood depletion.
    private void DieFromBloodThirst()
    {
        if (IsDead) return;

        IsDead = true;

        //Disable physics simulation so the character stops interacting with the world
        if (rb2D != null)
        {
            rb2D.simulated = false;
            Debug.Log("Player died from blood thirst");
        }

        Debug.Log($"Game Time from start:  {Time.time - startTime}");
    }


    // Handles instant death caused by light exposure.
    public void DieFromLight()
    {
        //Don't die if shielded or already dead
        if (IsShielded) return;
        if (IsDead) return;

        IsDead = true;

        //Disable physics simulation so the character stops interacting with the world
        if (rb2D != null)
        {
            rb2D.simulated = false;
        }

        HealthPoint = 0f;
    }

    private void OnDestroy()
    {
        // Clear singleton reference when destroyed
        if (Instance == this)
            Instance = null;
    }
}