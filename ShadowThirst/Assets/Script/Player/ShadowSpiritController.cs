using UnityEngine;

public class ShadowSpiritController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private VirtualJoystickController joystickController;
    [SerializeField] private Rigidbody2D rb2D;

    //Public getter with private setter provides controlled access to shield status
    public bool isShielded { get; private set; }
    private bool isDead = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //Early exit if references are missing or player is dead
        if ( rb2D  == null || joystickController == null  || isDead) return;

        Vector2 input = joystickController.Direction;
        //Only move if there's significant input
        if (input.sqrMagnitude > 0.001f)
        {
            //Move the character using physics-based movement
            rb2D.MovePosition(rb2D.position + input * movementSpeed * Time.fixedDeltaTime);
        }
    }

    public void DieFromLight()
    {
        //Don't die if shielded or already dead
        if (isShielded) return;
        if (isDead) return;

        isDead = true;
        
        //Disable physics simulation so the character stops interacting with the world
        if (rb2D != null)
        {
            rb2D.simulated = false;
        }
    }
}