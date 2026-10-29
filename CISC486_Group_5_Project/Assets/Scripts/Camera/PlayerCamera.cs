using UnityEngine;
public class PlayerCameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float height = 12f;
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 velocity;
    private void LateUpdate()
    {
        Vector3 destination = target.position + Vector3.up * height;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            destination,
            ref velocity,
            smoothTime
        );
        transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }
}
