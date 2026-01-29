using System.Collections.Generic;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    [SerializeField] private AttackButtonController attackButtonController;

    private List<EnemyController> enemiesInRange = new List<EnemyController>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null && !enemiesInRange.Contains(enemyController))
            {
                {
                    enemiesInRange.Add(enemyController);
                }

                attackButtonController?.ActivateAttackIcon();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemiesInRange.Remove(enemyController);
            }

            if (enemiesInRange.Count == 0)
            {
                attackButtonController?.DeactivateAttackIcon();
            }
        }
    }


    public EnemyController GetClosestEnemy()
    {
        if (enemiesInRange.Count == 0) return null;

        return enemiesInRange[0];
    }
}
