using UnityEngine;

public class ShadowSpiritController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private VirtualJoystickController joystickController;
    [SerializeField] private Rigidbody2D rb2D;

    private bool isShielded = false;
    private bool isDead = false;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if ( rb2D  == null || joystickController == null ) return;

        Vector2 input = joystickController.Direction;
        if (input.sqrMagnitude > 0.001f)
        {
            rb2D.MovePosition(rb2D.position + input * movementSpeed * Time.fixedDeltaTime);
        }
    }

    public void DieFromLight()
    {
        if (isShielded) return;

        if (isDead) return;

        isDead = true;

        if (rb2D != null)
        {
            GetComponent<Rigidbody2D>().simulated = false;
        }
    }
}
