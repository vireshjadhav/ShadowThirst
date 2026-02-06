using UnityEngine;

public class BloodVialController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Triggers when player collides with the blood vial
        if (other.gameObject.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.gameObject.GetComponent<ShadowSpiritController>();

            if (shadowSpirit != null)
            {
                shadowSpirit.Heal(shadowSpirit.HealPoint);      // Heal player

                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.AddBatPoints(ScoreManager.Instance.BloodBatPoints);           // Add score
                }
            }


            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.Play(Sounds.BloodVialPickUp);
            }

            Destroy(this.gameObject);       // Consume pickup
        }
    }
}
