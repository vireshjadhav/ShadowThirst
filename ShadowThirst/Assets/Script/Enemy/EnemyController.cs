using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [Header("Patrol Settings")]
    [SerializeField] private Vector3 pointA;                    // First patrol point
    [SerializeField] private Vector3 pointB;                    // Second patrol point
    [SerializeField] private float movementSpeed = 2f;          // Movement speed for patrol and chase
    [SerializeField] private Animator enemyAnimator;            // Controls enemy animations
    [SerializeField] private float deathDelay = 2f;             // Delay before destroying enemy after death
    [SerializeField] private float attackDamage = 5f;           // Health damage dealt to player per hit

    private Vector3 patrolTarget;           // Current patrol destination
    private bool attacking = false;         // True when player is inside attack range
    private bool isDead = false;            // Prevents logic after enemy death

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Choose the closest patrol point at spawn
        patrolTarget = (Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB)) ? pointA : pointB;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        // Switch behavior based on player proximity
        if (!attacking)
        {
            PatrolEnemy();
        }
        else
        {
            AttackMovement();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead) return;

        // Enter attack mode when player enters range
        if (other.CompareTag("Player"))
        {
            attacking = true;
            SetAttackAnimation(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (isDead) return;

        // Return to patrol when player leaves range
        if (other.CompareTag("Player"))
        {
            attacking = false;
            SetAttackAnimation(false);
        }
    }

    private void PatrolEnemy()
    {
        // Move between patrol points
        transform.position = Vector3.MoveTowards(transform.position, patrolTarget, movementSpeed * Time.deltaTime);

        // Swap patrol target when destination reached
        if (Vector3.Distance(transform.position, patrolTarget) < 0.01f)
        {
            patrolTarget = patrolTarget == pointB ? pointA : pointB;
        }

        // Set animation
        if (enemyAnimator != null)
        {
            FaceTarget(patrolTarget);
            enemyAnimator.SetBool("Moving", true);
            enemyAnimator.SetBool("Attacking", false);
        }
        
    }

    private void AttackMovement()
    {
        // Chase player while attacking
        if (ShadowSpiritController.Instance != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, ShadowSpiritController.Instance.transform.position, movementSpeed * Time.deltaTime);
            
            // Set facing direction
            FaceTarget(ShadowSpiritController.Instance.transform.position);


            if (enemyAnimator != null)
            {
                enemyAnimator.SetBool("Attacking", true);
                enemyAnimator.SetBool("Moving", true);
            }
        }
    }

    private void SetAttackAnimation(bool isAttacking)
    {
        // Centralized animation state control
        if (enemyAnimator != null)
        {
            enemyAnimator.SetBool("Attacking", isAttacking);
            enemyAnimator.SetBool("Moving", !isAttacking);

            if (isAttacking && ShadowSpiritController.Instance != null)
            {
                FaceTarget(ShadowSpiritController.Instance.transform.position);
            }
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

    private void GiveDamage()
    {
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

    public void Die()
    {
        // Lock enemy state
        isDead = true;
        attacking = false;

        if (enemyAnimator != null)
        {
            enemyAnimator.SetTrigger("Dead");
            enemyAnimator.SetBool("Attacking", false);
            enemyAnimator.SetBool("Moving", false);

            // Face player for death animation consistency
            if (ShadowSpiritController.Instance != null)
            {
                FaceTarget(ShadowSpiritController.Instance.transform.position);
            }
        }

        // Prevent further collisions
        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(DestroyAfterDelay(deathDelay));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Allow death animation to finish before cleanup
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
