using UnityEngine;

public class LightHazardController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the colliding object has the "Player" tag
        if (other.CompareTag("Player"))
        {
            ShadowSpiritController shadowSpirit = other.GetComponent<ShadowSpiritController>();

            //If we found a ShadowSpiritController component, check shield status  and trigger its light death
            if (shadowSpirit != null && !shadowSpirit.isShielded)
            {
                shadowSpirit.DieFromLight();
            }
        }
    }
}
