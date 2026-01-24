using TMPro;
using UnityEngine;

public class ScorePanelController : MonoBehaviour
{
    [Header ("References")]
    [SerializeField] private ScoreManager scoreManager;      // Score source
    [SerializeField] private TextMeshProUGUI scoreText;      // UI text
    private float batPoints = 0f;
    private float previousBat = 0f;        // Cache last value

    private void Start()
    {
        // Use singleton if not set from Inspector
        if (scoreManager == null) {
            scoreManager = ScoreManager.Instance;
        }

        // Auto-fetch text if missing
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Late-bind safety (execution order / scene reload)
        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
            if (scoreManager == null) return;
        }

        UpdateScore();
    }

    private void UpdateScore()
    {
        batPoints = scoreManager.BatPoints;


        // Update UI only if value changed
        if (!Mathf.Approximately(previousBat, batPoints))
        {
            scoreText.text = batPoints.ToString("N0");       // No decimals
            previousBat = batPoints;
        }
    }
}
