using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject optionMenuPanel;            // Options/settings panel
    [SerializeField] private GameObject gameOverPanel;              // Game over UI
    [SerializeField] private float gameOverPanelDelay = 2.0f;       // Delay before showing game over

    [Header("Button & Slider References")]
    [SerializeField] private Button restartButton;                  // Restart button in pause menu
    [SerializeField] private Button gameOverRestartButton;          // Restart button in game over menu
    [SerializeField] private Button optionButton;                   // Open option button
    [SerializeField] private Button quitButton;                     // Quit application button in  puase menu
    [SerializeField] private Button gameOverQuitButton;             // Quit application button in game over menu        
    [SerializeField] private Button backButton;                     // Back button to go back to pause menu 
    [SerializeField] private Slider musicSlider;                    // Master mute toggle
    [SerializeField] private Slider effectSlider;                   // Music volume control
    [SerializeField] private Toggle muteToggle;                     // SFX volume control

    [Header("Score References")]
    [SerializeField] private ScoreManager scoreManager;             // Score source
    [SerializeField] private TextMeshProUGUI scoreText;             // Score display text


    private float batPoints = 0f;
    private float previousBat = 0f;                                 // Cache last score value

    [Header("Scene Settings")]
    [SerializeField] private int levelIndex = 1;

    private bool isGameOverPanelShown = false;                      // Prevents multiple triggers
    private Coroutine gameOverCoroutine;                            // Tracks delayed game over



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeScoreManager();
        InitializeUIReferences();
        AddAllListeners();

        //Ensure panels are closed initially
        if (gameOverPanel != null ) gameOverPanel.SetActive(false);
        if (optionMenuPanel != null ) optionMenuPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
        UpdateScore();
    }


    // Detects player death and triggers game over once
    private void CheckGameOver()
    {
        //Only check if not already showing game over panel
        if (isGameOverPanelShown) return;
        if (ShadowSpiritController.Instance == null) return;
        if (!ShadowSpiritController.Instance.IsDead) return;

        // Start coroutine and track it
        gameOverCoroutine = StartCoroutine(OpenGameOverPanelAfterDelay());
        isGameOverPanelShown = true;
        
    }

    // Delays game-over panel appearance for better visual pacing
    private IEnumerator OpenGameOverPanelAfterDelay()
    {
        yield return new WaitForSeconds(gameOverPanelDelay);
        OpenGameOverPanel();
    }

    // Activates the game-over UI panel
    private void OpenGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // Ensures score manager reference is assigned (singleton fallback)
    private void InitializeScoreManager()
    {
        if (scoreManager == null)
        {
            scoreManager = ScoreManager.Instance;
        }
    }

    // Validates or auto-fetches UI references needed at runtime
    private void InitializeUIReferences()
    {
        // Only auto-fetch if absolutely necessary
        if (scoreText == null)
        {
            scoreText = GetComponent<TextMeshProUGUI>();
            if (scoreText == null)
            {
                {
                    Debug.LogWarning("ScoreText not assigned and not found on this GameObject", this);
                }
            }
        }
    }

    // Syncs score value from ScoreManager and updates UI only on change
    private void UpdateScore()
    {
        if (scoreManager != null)
        {
            scoreManager = ScoreManager.Instance;
            if (scoreManager == null) return;
        }

        float currentBatPoints = scoreManager.BatPoints;

        // Update UI only if value changed
        if (!Mathf.Approximately(previousBat, batPoints))
        {
            batPoints = currentBatPoints;
            UpdateScoreDisplay();
            previousBat = batPoints;
        }
    }

    // Updates the score text shown on screen
    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {

            scoreText.text = batPoints.ToString("N0");       // No decimals
        }
    }

    // Registers all UI button, slider, and toggle listeners
    private void AddAllListeners()
    {
        // Pause Menu buttons
        if (restartButton != null) restartButton.onClick.AddListener(RestartLevel);
        if (optionButton != null) optionButton.onClick.AddListener(OpenOptionPanel);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);

        // Game over buttons
        if (gameOverRestartButton != null) gameOverRestartButton.onClick.AddListener(RestartLevel);
        if (gameOverQuitButton != null) gameOverQuitButton.onClick.AddListener(Quit);

        // Audio controls
        if (backButton != null) backButton.onClick.AddListener(BackToPauseMenu);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (effectSlider != null) effectSlider.onValueChanged.AddListener(SetEffectVolume);
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(MuteToggle);
    }

    // Unregisters all listeners to avoid memory leaks and duplicate calls
    private void RemoveAllListeners()
    {
        // Pause Menu buttons
        if (restartButton != null) restartButton.onClick.RemoveListener(RestartLevel);
        if (optionButton != null) optionButton.onClick.RemoveListener(OpenOptionPanel);
        if (quitButton != null) quitButton.onClick.RemoveListener(Quit);

        // Game over buttons
        if (gameOverRestartButton != null) gameOverRestartButton.onClick.RemoveListener(RestartLevel);
        if (gameOverQuitButton != null) gameOverQuitButton.onClick.RemoveListener(Quit);

        // Audio controls
        if (backButton != null) backButton.onClick.RemoveListener(BackToPauseMenu);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (effectSlider != null) effectSlider.onValueChanged.RemoveListener(SetEffectVolume);
        if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(MuteToggle);
    }

    // Reloads the current gameplay scene safely
    private void RestartLevel()
    {
        // Stop any running coroutines
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }

        ResetGameOverState();
        RemoveAllListeners();
        SceneManager.LoadScene(levelIndex);
    }

    // Opens the options/settings menu
    private void OpenOptionPanel()
    {
        if (optionMenuPanel != null)
        {
            optionMenuPanel.SetActive(true);
        }
    }

    // Returns from options menu back to pause menu
    private void BackToPauseMenu()
    {
        if (optionMenuPanel != null)
        {
            optionMenuPanel.SetActive(false);
        }
    }

    // Exits the game (editor-safe)
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    // Exits the game (editor-safe)
    private void SetMusicVolume(float value)
    {
        // Implement music volume logic here
    }

    // Adjusts sound effects volume (hook for AudioManager)
    private void SetEffectVolume(float value)
    {
        // Implement SFX volume logic here
    }

    // Toggles global mute state
    private void MuteToggle(bool isOn)
    {
        // Implement audio mute logic here
    }

    // Cleans up listeners and coroutines when object is destroyed
    private void OnDestroy()
    {
        RemoveAllListeners();

        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = null;
        }
    }

    // Resets game-over UI and internal state for restart or reuse
    public void ResetGameOverState()
    {
        isGameOverPanelShown = false;
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }

        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
            gameOverCoroutine = null;
        }
    }
}
