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
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickBG, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, radius);

        joystickHandle.anchoredPosition = pos;

        Direction = Vector2.ClampMagnitude(pos / radius, 1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickHandle.anchoredPosition = Vector2.zero;
        Direction = Vector2.zero;
    }
}
