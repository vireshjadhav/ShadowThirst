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
        //Early exit if no target to follow
        if (target == null) return;

        //Calculate desired camera position
        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = transform.position.z; //Maintain camera's original Z position

        //Smoothly move camera towards desired position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime, Mathf.Infinity, Time.deltaTime);
    }
}
