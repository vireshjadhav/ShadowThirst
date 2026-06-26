using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShieldButtonController : MonoBehaviour, IPointerDownHandler
{
    [Header("Reference")]
    [SerializeField] private Image shieldIcon;            // Shield button icon
    [SerializeField] private Image shieldCooldown;

    [Header("Alpha Settings")]
    [SerializeField] private float minAlpha = 0.1f;       // Inactive / no shield state
    [SerializeField] private float maxAlpha = 0.6f;       // Shield available state

    private bool isCooldownActive = false;
    private float cooldownTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize shield icon as inactive
        if (shieldIcon == null) return;

        SetAlpha(minAlpha);

        if (shieldCooldown != null)
            shieldCooldown.fillAmount = 0f;

        cooldownTimer = ShadowSpiritController.Instance.ShieldLife;
    }

    void Update()
    {
        // Safety check for player singleton
        if (ShadowSpiritController.Instance == null) return;
        if (shieldIcon == null) return;

        if (!isCooldownActive)
        {
            // Update icon visibility based on shield availability
            if (ShadowSpiritController.Instance.HaveShield)
            {
                SetAlpha(maxAlpha);
                shieldCooldown.fillAmount = 1f;
            }
            else
            {
                SetAlpha(minAlpha);
            }
        }

        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;

            float totalTime = ShadowSpiritController.Instance.ShieldLife;
            shieldCooldown.fillAmount = cooldownTimer / totalTime;
            if (cooldownTimer <= 0f)
            {
                isCooldownActive = false;
                shieldCooldown.fillAmount = 0f;
            }
        }
    }

    // Handles shield button press
    public void OnPointerDown(PointerEventData eventData)
    {
        // Prevent null reference or invalid state
        if (ShadowSpiritController.Instance == null) return;
        if (!ShadowSpiritController.Instance.HaveShield) return;
        if (isCooldownActive) return;


        // Activate shield logic handled by player controller
        ShadowSpiritController.Instance.ActivateShield();

        cooldownTimer = ShadowSpiritController.Instance.ShieldLife;

        shieldCooldown.fillAmount = 1f;
        isCooldownActive = true;

        SetAlpha(minAlpha);
    }

    // Utility method to control shield icon transparency
    private void SetAlpha(float value)
    {
        Color color = shieldIcon.color;
        color.a = value;
        shieldIcon.color = color;
    }
}
