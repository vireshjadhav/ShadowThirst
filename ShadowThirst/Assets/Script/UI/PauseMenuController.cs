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
    [SerializeField] private GameObject resumeIcon;
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

    private float lastMusicVolume;
    private float lastEffectVolume;

    private bool isInitializing = true;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeScoreManager();
        InitializeUIReferences();
        //Initialize audio UI elements with current values
        InitializeAudioUI();

        AddAllListeners();

        isInitializing = false;

        //Ensure panels are closed initially
        if (gameOverPanel != null ) gameOverPanel.SetActive(false);
        if (optionMenuPanel != null ) optionMenuPanel.SetActive(false);
        if (resumeIcon != null ) resumeIcon.SetActive(false);
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
        if (gameOverCoroutine != null) return;
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


    private void InitializeAudioUI()
    {
        // Set initial values for audio UI elements if SoundManager exists
        if (SoundManager.Instance == null) return;

        bool isMuted = SoundManager.Instance.isMute;

        // Cache real volumes
        lastMusicVolume = SoundManager.Instance.GetMusicVolume();
        lastEffectVolume = SoundManager.Instance.GetEffectVolume();

        // Set mute toggle 
        muteToggle.SetIsOnWithoutNotify(isMuted);

        if (isMuted)
        {
            // Show sliders as zero when muted
            musicSlider.SetValueWithoutNotify(0f);
            effectSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            // Show actual volumes
            musicSlider.SetValueWithoutNotify(lastMusicVolume);
            effectSlider.SetValueWithoutNotify(lastEffectVolume);
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
        Time.timeScale = 1f;

        // Stop any running coroutines
        if (gameOverCoroutine != null)
        {
            StopCoroutine(gameOverCoroutine);
        }


        ResetGameOverState();
        RemoveAllListeners();

        SceneManager.LoadScene(levelIndex);
        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Opens the options/settings menu
    private void OpenOptionPanel()
    {
        SyncSliders();

        if (optionMenuPanel != null)
        {
            optionMenuPanel.SetActive(true);
            resumeIcon.SetActive(false);
        }

        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Returns from options menu back to pause menu
    private void BackToPauseMenu()
    {
        if (optionMenuPanel != null)
        {
            optionMenuPanel.SetActive(false);
            resumeIcon.SetActive(true);
        }

        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Exits the game (editor-safe)
    private void Quit()
    {
        SoundManager.Instance.Play(Sounds.ButtonClick);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


    // Adusts sound music volume 
    private void SetMusicVolume(float value)
    {
        if(SoundManager.Instance ==  null)  return;
        SoundManager.Instance.SetMusicVolume(value);
    }

    // Adjusts sound effects volume 
    private void SetEffectVolume(float value)
    {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.SetEffectVolume(value);
    }

    // Toggles global mute state
    private void MuteToggle(bool isOn)
    {
        if (isInitializing) return;
        if (SoundManager.Instance == null) return;

        SoundManager.Instance.Play(Sounds.ButtonClick);

        if (isOn)
        {
            lastEffectVolume = effectSlider.value;
            lastMusicVolume = musicSlider.value;

            musicSlider.SetValueWithoutNotify(0f);
            effectSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            musicSlider.SetValueWithoutNotify(lastMusicVolume);
            effectSlider.SetValueWithoutNotify(lastEffectVolume);

            SoundManager.Instance.SetMusicVolume(lastMusicVolume);
            SoundManager.Instance.SetEffectVolume(lastEffectVolume);
        }

        SoundManager.Instance.Mute(isOn);
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

    private void SyncSliders()
    {
        if (SoundManager.Instance == null) return;

        if (SoundManager.Instance.isMute)
        {
            musicSlider.SetValueWithoutNotify(0f);
            effectSlider.SetValueWithoutNotify(0f);
        }
        else
        {
            musicSlider.SetValueWithoutNotify(SoundManager.Instance.GetMusicVolume());
            effectSlider.SetValueWithoutNotify(SoundManager.Instance.GetEffectVolume());
        }
        
    }
}
