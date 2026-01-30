using UnityEngine;

public class ShieldVialController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Triggers when player collides with the shield vial
        if (other.gameObject.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.gameObject.GetComponent<ShadowSpiritController>();

            if (shadowSpirit != null)
            {
                shadowSpirit.CollectShield();
            }


            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.Play(Sounds.ShieldVialPickUp);
            }

            Destroy(this.gameObject);    // Collect shield 
        }
    }
}

