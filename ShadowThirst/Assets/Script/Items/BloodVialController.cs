using UnityEngine;

public class BloodVialController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Triggers when player collides with the blood vial
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(this.gameObject);
            ShadowSpiritController shadowSpirit = other.gameObject.GetComponent<ShadowSpiritController>();

            if (shadowSpirit != null)
            {
                shadowSpirit.Heal(shadowSpirit.HealPoint);
            }

            // Destroy vial 
            Destroy(this.gameObject);
        }
    }
}
