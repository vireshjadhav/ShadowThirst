using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShieldButtonController : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")]
    [SerializeField] private Image shieldIcon;            // Shield button icon

    [Header("Alpha Settings")]
    [SerializeField] private float minAlpha = 0.1f;       // Inactive / no shield state
    [SerializeField] private float maxAlpha = 0.6f;       // Shield available state

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize shield icon as inactive
        if (shieldIcon == null) return;

        SetAlpha(minAlpha);
    }

    void Update()
    {
        // Safety check for player singleton
        if (ShadowSpiritController.Instance == null) return;
        if (shieldIcon == null) return;

        // Update icon visibility based on shield availability
        if (ShadowSpiritController.Instance.HaveShield)
        {
            SetAlpha(maxAlpha);
        }
        else
        {
            SetAlpha(minAlpha);
        }
    }

    // Handles shield button press
    public void OnPointerDown(PointerEventData eventData)
    {
        // Prevent null reference or invalid state
        if (ShadowSpiritController.Instance == null) return;
        if (!ShadowSpiritController.Instance.isActiveAndEnabled) return;

        // Activate shield logic handled by player controller
        ShadowSpiritController.Instance.ActivateShield();
    }

    // Utility method to control shield icon transparency
    private void SetAlpha(float value)
    {
        Color color = shieldIcon.color;
        color.a = value;
        shieldIcon.color = color;
    }
}
