using UnityEngine;
using UnityEngine.EventSystems;

public class ResumeButtonController : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")]
    [SerializeField] private GameObject pauseMenuPanel;       // Pause menu UI panel
    [SerializeField] private GameObject pauseButton;          // Pause button shown during gameplay

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Called when the resume button is pressed by the player
    public void OnPointerDown(PointerEventData eventData)
    {
        // Hide pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // Resume normal game time
        Time.timeScale = 1.0f;

        // Re-enable pause button for gameplay
        if (pauseButton != null)
        {
            pauseButton.SetActive(true);
        }

        // Hide resume button itself
        gameObject.SetActive(false);
    }
}
