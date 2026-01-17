using UnityEngine;

public class LightHazardController : MonoBehaviour
{
    private Collider2D hazardCollider;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Cache required components
        hazardCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Enables or disables the light hazard
    // Controls both collision and visibility
    public void SetHazardActive(bool active)
    {
        if (hazardCollider != null)
            hazardCollider.enabled = active;

        if (spriteRenderer != null)
            spriteRenderer.enabled = active;
    }

    // Returns current hazard state
    public bool GetHazardActive()
    {
        return hazardCollider.enabled;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.GetComponent<ShadowSpiritController>();

            //If we found a ShadowSpiritController component, check shield status  and trigger its light death
            if (shadowSpirit != null && !shadowSpirit.IsShielded)
            {
                shadowSpirit.DieFromLight();
            }
        }
    }
}
