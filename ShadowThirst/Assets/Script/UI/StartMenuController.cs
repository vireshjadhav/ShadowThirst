using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject startPanel;          // Main start menu panel
    [SerializeField] private GameObject optionPanel;         // Options/settings panel

    [SerializeField] private Button startButton;             // Start game button
    [SerializeField] private Button optionButton;            // Open options button
    [SerializeField] private Button quitButton;              // Quit game button
    [SerializeField] private Button backButton;              // Back to start menu button

    [SerializeField] private Toggle muteToggle;              // Master mute toggle
    [SerializeField] private Slider musicSlider;       // Music volume control
    [SerializeField] private Slider effectSlider;      // SFX volume control

    [Header("Scene Settings")]
    [SerializeField] private int mainLevelIndex = 1;          // Gameplay scene index

    private float lastMusicVolume;
    private float lastEffectVolume;

    private bool isInitializing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //Initialize audio UI elements with current values
        InitializeAudioUI();

        // Initial panel state
        if (startPanel != null)  startPanel.SetActive(true);
        if (optionPanel  != null) optionPanel.SetActive(false);

        // Button bindings
        if (startButton != null) startButton.onClick.AddListener(StartLevel);
        if (optionButton != null) optionButton.onClick.AddListener(OpenOptionPanel);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);
        if (backButton != null) backButton.onClick.AddListener(OpenStartPanel);

        // Audio UI bindings
        if (muteToggle != null) muteToggle.onValueChanged.AddListener(MuteToggle);
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (effectSlider != null) effectSlider.onValueChanged.AddListener(SetEffectVolume);


        isInitializing = false;
    }

    // Loads the main gameplay scene
    private void StartLevel()
    {
        SceneManager.LoadScene(mainLevelIndex);
        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Switches from start menu to options panel
    private void OpenOptionPanel()
    {
        SyncSliders();
        if (optionPanel != null)optionPanel.SetActive(true);
        if (startPanel != null) startPanel.SetActive(false);
        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Returns from options panel to start menu
    private void OpenStartPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);
        if (startPanel != null) startPanel.SetActive(true);
        SoundManager.Instance.Play(Sounds.ButtonClick);
    }

    // Quits the game (editor + build safe)
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
        
        if (SoundManager.Instance == null) return;
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


    private void OnDestroy()
    {
        // Cleanup listeners to prevent memory leaks
        if (startButton != null) startButton.onClick.RemoveListener(StartLevel);
        if (optionButton != null) optionButton.onClick.RemoveListener(OpenOptionPanel);
        if (quitButton != null) quitButton.onClick.RemoveListener(Quit);
        if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(MuteToggle);

        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (effectSlider != null) effectSlider.onValueChanged.RemoveListener(SetEffectVolume);
        if (backButton != null) backButton.onClick.RemoveListener(OpenStartPanel);
    }
}
