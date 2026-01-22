using UnityEngine;

public class PoisonVialController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Triggers when player collides with the poison vial
        if (other.gameObject.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.gameObject.GetComponent<ShadowSpiritController>();

            if (shadowSpirit != null)
            {
                shadowSpirit.TakeDamage(shadowSpirit.DamagePoint);
            }

            // Destroy vial 
            Destroy(this.gameObject);
        }
    }
}
