using UnityEngine;

public class BloodBarController : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private RectTransform filler;

    
    private Vector3 originalScale;    // Stores original scale to avoid cumulative scaling errors
    private float lastHealth = -1f;   // Tracks last health value to avoid unnecessary UI updates

    private void Start()
    {
        // Cache initial scale of the filler
        if (filler != null)
            originalScale = filler.localScale;
    }

    private void Update()
    {
        // Abort if player instance is not available
        if (ShadowSpiritController.Instance == null)
            return;
        

        // Get current health
        float currentHealth = ShadowSpiritController.Instance.HealthPoint;
        float maxHealth = ShadowSpiritController.Instance.MaxHealth;

        // Only update UI when health changes significantly (0.01 threshold) 
        if (Mathf.Abs(currentHealth - lastHealth) > 0.01f)
        {
            UpdateHealthBar(currentHealth, maxHealth);
            lastHealth = currentHealth;
        }
    }

    // Updates the blood bar scale based on current health percentage.
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (filler == null) return;

        float healthPointPercent = Mathf.Clamp01(currentHealth / maxHealth);

        // Update the scale
        Vector3 newScale = filler.localScale;
        newScale.x = originalScale.x * healthPointPercent;
        filler.localScale = newScale;
    }
}
