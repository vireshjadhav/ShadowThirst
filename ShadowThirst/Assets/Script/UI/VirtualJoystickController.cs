using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualJoystickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Settings")]
    [SerializeField] private RectTransform joystickHandle;
    [SerializeField] private float radius = 10.0f;

    public Vector2 Direction { get; private set; }
    private RectTransform joystickBG;

    private void Awake()
    {
        joystickBG = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Start dragging immediately when pointer is pressed down
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        //Convert screen position to local position within joystick background
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, radius);

        //Move joystick handle to the calculated position
        joystickHandle.anchoredPosition = pos;

        //Normalize the direction for consistent input values (-1 to 1)
        Direction = Vector2.ClampMagnitude(pos / radius, 1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Reset joystick to center position when released
        joystickHandle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }
}
