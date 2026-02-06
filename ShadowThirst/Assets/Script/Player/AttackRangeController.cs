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

                ShadowSpiritController.Instance.GetEnemyInRange(enemyController.transform.position);

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
        EnemyController closest = null;
        float minDist = float.MaxValue;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(enemy.transform.position, ShadowSpiritController.Instance.transform.position);

            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy;
            }
        }

        return closest;
    }
}
