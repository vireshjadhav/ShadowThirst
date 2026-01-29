using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AttackButtonController : MonoBehaviour, IPointerDownHandler
{
    [Header("UI References")]
    [SerializeField] private Image attackIcon;            // Attack button icon

    [Header("Alpha Settings")]
    [SerializeField] private float minAlpha= 0.1f;        // Alpha when attack is inactive
    [SerializeField] private float maxAlpha= 0.6f;        // Alpha when attack is active


    public bool isAttackButtonActive= false;              // Controls whether attack input is allowed

    private void Start()
    {
        // Start with inactive visual state
        SetAlpha(minAlpha);
    }

    // Sets transparency of the attack icon
    private void SetAlpha(float value)
    {
        if (attackIcon == null) return;

        Color color = attackIcon.color;
        color.a = value;
        attackIcon.color = color;
    }

    // Called when attack becomes available 
    public void ActivateAttackIcon()
    {
        if (attackIcon != null)
        {
            isAttackButtonActive = true;
            Color color = attackIcon.color;
            color.a = maxAlpha;
            attackIcon.color = color;
        }
    }

    // Called when attack is disabled 
    public void DeactivateAttackIcon()
    {
        if (attackIcon != null)
        {
            isAttackButtonActive = false;
            Color color = attackIcon.color;
            color.a = minAlpha;
            attackIcon.color = color;
        }
    }

    // Triggered when player presses the attack button
    public void OnPointerDown(PointerEventData eventData)
    {
        // Prevent attack if button is inactive
        if (isAttackButtonActive == false) return;

        // Delegate attack logic to player controller
        ShadowSpiritController.Instance.Attack();
    }
}
