using UnityEngine;
using UnityEngine.EventSystems;

public class PauseButtonController : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")] 
    [SerializeField] private GameObject pauseMenuPanel;         // Pause menu UI panel
    [SerializeField] private GameObject resumeButton;           // Resume button shown inside pause menu

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(true);
    }

    // Called when the player taps/clicks the pause button
    public void OnPointerDown(PointerEventData eventData)
    {
        // Show pause menu panel
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }

        // Freeze game time
        Time.timeScale = 0.0f;

        // Enable resume button inside pause menu
        if (resumeButton != null)
        {
            resumeButton.SetActive(true);
        }

        // Hide pause button while game is paused
        gameObject.SetActive(false);
    }
}
