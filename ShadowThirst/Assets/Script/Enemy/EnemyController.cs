using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{

    [Header("Patrol Settings")]
    [SerializeField] private Vector3 pointA;                    // First patrol point
    [SerializeField] private Vector3 pointB;                    // Second patrol point
    [SerializeField] private float movementSpeed = 2f;          // Movement speed for patrol and chase
    [SerializeField] private float waitAfterAttack = 0.5f;

    [Header("Combat Settings")]
    [SerializeField] private float attackDamage = 5f;           // Health damage dealt to player per hit
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackWindup = 0.3f;

    [Header("Death & Respawn")]
    [SerializeField] private float deathDelay = 2f;             // Delay before destroying enemy after death
    [SerializeField] private float respawnDelay = 5f;

    [Header("References")]
    [SerializeField] private Animator enemyAnimator;            // Controls enemy animations

    private Vector3 patrolTarget;           // Current patrol destination
    private Vector3 spawnPoisition;

    private bool isDead = false;            // Prevents logic after enemy death
    private bool playerInRange = false;
    private bool canAttack = true;
    private bool isWaiting = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPoisition = transform.position;

        // Choose the closest patrol point at spawn
        patrolTarget = (Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB)) ? pointA : pointB;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (playerInRange )
        {
            WaitAndAttack();
        }
        else
        {
            PatrolEnemy();
        }

    }

    private void PatrolEnemy()
    {
        if (isWaiting) return;
        
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, movementSpeed * Time.deltaTime);

        // Swap patrol target when destination reached
        if (Vector3.Distance(transform.position, patrolTarget) <0.05f)
        {
            patrolTarget = patrolTarget == pointB ? pointA : pointB;
        }

        FaceTarget(patrolTarget);

        // Set animation
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("Moving", true);
            enemyAnimator.SetBool("Attacking", false);
        }

    }


    private void WaitAndAttack()
    {
        enemyAnimator.SetBool("Moving", false);

        if (!canAttack && isWaiting) return;
        
        isWaiting = true;
        StartCoroutine(AttackRoutine());
        
    }

    private IEnumerator AttackRoutine()
    {
        canAttack = false;

        SetAttackAnimation(true);


        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.Play(Sounds.EnemyAttack);
        }

        yield return new WaitForSeconds(attackWindup);

        GiveDamage();

        SetAttackAnimation(false);

        yield return new WaitForSeconds(waitAfterAttack);
        yield return new WaitForSeconds(attackCooldown);

        isWaiting = false;
        canAttack = true;
    }

    public void GiveDamage()
    {
        if (!playerInRange || isDead) return;

        // Apply health pressure
        if (ShadowSpiritController.Instance != null)
        {
            ShadowSpiritController.Instance.TakeDamage(attackDamage);
        }

        // Apply score pressure
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.SubtractBatPoints(ScoreManager.Instance.EnemyDamage);
        }
    }

    private void SetAttackAnimation(bool isAttacking)
    {
        // Centralized animation state control
        if (enemyAnimator != null)
        {
            if (ShadowSpiritController.Instance == null) return;
            FaceTarget(ShadowSpiritController.Instance.transform.position);

            enemyAnimator.SetBool("Attacking", isAttacking);
            enemyAnimator.SetBool("Moving", !isAttacking);
        }
    }

    private void FaceTarget(Vector3 targetPosition)
    {
        // Orient animation based on dominant movement axis
        if (enemyAnimator == null) return;

        Vector3 rawDirection = targetPosition - transform.position;

        float absX = Mathf.Abs(rawDirection.x);
        float absY = Mathf.Abs(rawDirection.y);

        if (absX < absY)
        {
            enemyAnimator.SetFloat("X", 0f);
            enemyAnimator.SetFloat("Y", Mathf.Sign(rawDirection.y));
        }
        else
        {
            enemyAnimator.SetFloat("X", Mathf.Sign(rawDirection.x));
            enemyAnimator.SetFloat("Y", 0f);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Enter attack mode when player enters range
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (ShadowSpiritController.Instance != null)
            {
                FaceTarget(ShadowSpiritController.Instance.transform.position);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (isDead) return;

        // Return to patrol when player leaves range
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            SetAttackAnimation(false);
        }
    }

    public void Die()
    {
        if (isDead) return;

        // Lock enemy state
        isDead = true;
        canAttack = false;
        playerInRange = false;

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Dead");
            enemyAnimator.SetBool("Attacking", false);
            enemyAnimator.SetBool("Moving", false);
        }


        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.Play(Sounds.EnemyDeath);
        }


        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddBatPoints(ScoreManager.Instance.EnemyKillPoints);
        }

        // Prevent further collisions
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(DisableAfterDelay(deathDelay));
    }

    private IEnumerator DisableAfterDelay(float delay)
    {
        // Allow death animation to finish before cleanup
        yield return new WaitForSeconds(delay);

        GameController.Instance.RespawnEnemy(this, respawnDelay);

        gameObject.SetActive(false);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);

        isDead = false;
        canAttack = true;
        playerInRange= false;
        isWaiting = false;

        transform.position = spawnPoisition;

        GetComponent<Collider2D>().enabled = true;

        if (enemyAnimator != null)
        {
            enemyAnimator.Rebind();
            enemyAnimator.Update(0f);
        }
    }
}
