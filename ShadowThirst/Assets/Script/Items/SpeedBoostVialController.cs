using UnityEngine;

public class SpeedBoostVialController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Triggers when player collides with the speed boost vial
        if (other.gameObject.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.gameObject.GetComponent<ShadowSpiritController>();

            if (shadowSpirit != null)
            {
                shadowSpirit.SpeedBoost();
            }

            // Destroy vial
            Destroy(this.gameObject);
        }
    }
}
