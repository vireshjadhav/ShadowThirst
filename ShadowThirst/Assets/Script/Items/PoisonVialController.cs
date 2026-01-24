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
                shadowSpirit.TakeDamage(shadowSpirit.DamagePoint);       // Damage player
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.SubtractBatPoints(ScoreManager.Instance.ToxicDamage);     // Reduce score by toxic damage
                }
            }

            Destroy(this.gameObject);        // Consume pickup
        }
    }
}
