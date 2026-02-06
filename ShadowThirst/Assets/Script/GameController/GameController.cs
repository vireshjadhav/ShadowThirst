using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        if (enemy == null || !enemy.gameObject.scene.isLoaded) yield break;

        enemy.Respawn();
    }

    public void RestartGame(int sceneIndex)
    {
        Time.timeScale = 1.0f;

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        SceneManager.LoadScene(sceneIndex);
    }
}
