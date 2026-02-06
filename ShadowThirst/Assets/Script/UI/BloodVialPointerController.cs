using UnityEngine;
using UnityEngine.UI;

public class BloodVialIndicatorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image pointerImage;
    [SerializeField] private Camera cam;

    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 6f;
    [SerializeField] private float edgePadding = 12f;
    [SerializeField] private float onScreenOffset = 90f;

    [Header("Vial targeting")]
    [SerializeField] private float vialYOffset = 0.5f;

    private Transform nearestBloodVial;
    private RectTransform  rectTransform;


    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (cam ==  null )
            cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        FindNearestBloodVial();
        UpdatePointerPosition();
    }

    private void FindNearestBloodVial()
    {
        GameObject[] vials = GameObject.FindGameObjectsWithTag("BloodVial");

        if (vials.Length == 0)
        {
            nearestBloodVial = null;
            return;
        }

        float closestDistance = float.MaxValue;
        Transform closest = null;

        if (ShadowSpiritController.Instance == null) return;

        Vector3 playerPos = ShadowSpiritController.Instance.transform.position;

        foreach (GameObject vial in vials)
        {
            float dist = Vector2.Distance(playerPos, vial.transform.position);

            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = vial.transform;
            }

        }
        nearestBloodVial = closest;
    }    
    
    private void UpdatePointerPosition()
    {
        if (nearestBloodVial == null)
        {
            pointerImage.enabled = false;
            return;
        }

        pointerImage.enabled = true;

        Vector2 halfSize = rectTransform.sizeDelta * 0.5f;

        Vector3 vialWorldPos = nearestBloodVial.position + Vector3.up * vialYOffset;
        Vector3 vialScreenPos = cam.WorldToScreenPoint(vialWorldPos);

        if (vialScreenPos.z < 0)
        {
            pointerImage.enabled = false;
            return;
        }

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height/2f);
        Vector2 direction = ((Vector2)vialScreenPos - screenCenter).normalized;


        float minX = halfSize.x + edgePadding;
        float maxX = Screen.width - halfSize.x - edgePadding;
        float minY = halfSize.y + edgePadding;
        float maxY = Screen.height - halfSize.y - edgePadding;


        bool isOnScreen = 
            vialScreenPos.x > 0 && vialScreenPos.x < Screen.width &&
            vialScreenPos.y > 0 && vialScreenPos.y < Screen.height;

        Vector2 targetPosition;

        if (isOnScreen)
        {
            targetPosition = (Vector2)vialScreenPos - direction * onScreenOffset;
        }
        else
        {
            targetPosition = screenCenter + direction * (Screen.width * 0.75f);
        }

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        rectTransform.position = Vector2.Lerp(rectTransform.position, targetPosition, Time.deltaTime * followSpeed);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rectTransform.rotation = Quaternion.Euler(0, 0 , angle);
    }
}
