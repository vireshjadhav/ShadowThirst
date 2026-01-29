using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RespawnEnemy(EnemyController enemy, float delay)
    {
        StartCoroutine(RespawnRoutine(enemy, delay));
    }

    private IEnumerator RespawnRoutine(EnemyController enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy == null) yield break;

        enemy.Respawn();
    }
}
