using Unity.VisualScripting;
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
    [SerializeField] private Slider musicVolumeSlider;       // Music volume control
    [SerializeField] private Slider effectVolumeSlider;      // SFX volume control

    [Header("Scene Settings")]
    [SerializeField] private int mainLevelIndex = 1;          // Gameplay scene index

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (effectVolumeSlider != null) effectVolumeSlider.onValueChanged.AddListener(SetEffectVolume);
    }

    // Loads the main gameplay scene
    private void StartLevel()
    {
        SceneManager.LoadScene(mainLevelIndex);
    }

    // Switches from start menu to options panel
    private void OpenOptionPanel()
    {
        if (optionPanel != null)optionPanel.SetActive(true);

        if (startPanel != null) startPanel.SetActive(false);
    }

    // Returns from options panel to start menu
    private void OpenStartPanel()
    {
        if (optionPanel != null) optionPanel.SetActive(false);

        if (startPanel != null) startPanel.SetActive(true);
    }

    // Quits the game (editor + build safe)
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Handles master mute toggle
    private void MuteToggle(bool isOn)
    {
        // Implement audio mute logic here
    }

    // Updates background music volume
    private void SetMusicVolume(float value)
    {
        // Implement music volume logic here
    }

    // Updates sound effects volume
    private void SetEffectVolume(float value)
    {
        // Implement SFX volume logic here
    }

    private void OnDestroy()
    {
        // Cleanup listeners to prevent memory leaks
        if (startButton != null) startButton.onClick.RemoveListener(StartLevel);
        if (optionButton != null) optionButton.onClick.RemoveListener(OpenOptionPanel);
        if (quitButton != null) quitButton.onClick.RemoveListener(Quit);
        if (muteToggle != null) muteToggle.onValueChanged.RemoveListener(MuteToggle);

        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        if (effectVolumeSlider != null) effectVolumeSlider.onValueChanged.RemoveListener(SetEffectVolume);
        if (backButton != null) backButton.onClick.RemoveListener(OpenStartPanel);
    }
}
