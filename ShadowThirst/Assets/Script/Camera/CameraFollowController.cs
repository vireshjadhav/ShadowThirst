using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    [Header("Follow Setting")]
    [SerializeField][Range(0.1f, 1f)] private float smoothTime;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z;

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime, Mathf.Infinity, Time.deltaTime);
    }
}
