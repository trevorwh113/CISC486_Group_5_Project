
using UnityEngine;

public class Sweeping : MonoBehaviour
{
    [Header("Sweeping")]
    public float sweepAngle = 60f;
    public float sweepSpeed = 1f;

    [Header("Player Detection")]
    public Transform player;
    public Collider playerCollider;

    [Min(0f)]
    public float viewDistance = 15f;

    [Range(1f, 180f)]
    public float viewAngle = 70f;

    [Header("Field of View Visualization")]
    public bool showFieldOfView = true;
    public Color coneColor = Color.green;
    public Color detectedColor = Color.red;

    [Range(8, 64)]
    public int coneSegments = 32;

    private Vector3 initialRotation;

    [Header("Detection State")]
    [SerializeField]
    private bool isPlayerDetected;

    void Start()
    {
        initialRotation = transform.localEulerAngles;

        // Find players collision box
        if (player != null && playerCollider == null)
        {
            playerCollider =
                player.GetComponentInChildren<Collider>();
        }

        if (player == null)
        {
            Debug.LogWarning("No player assigned!");
        }
        else if (playerCollider == null)
        {
            Debug.LogWarning("Player has no Collider!");
        }
    }

    void Update()
    {
        // Original sweeping rotation - unchanged
        float angle = Mathf.Sin(Time.time * sweepSpeed)
                      * sweepAngle;

        transform.localRotation = Quaternion.Euler(
            initialRotation.x,
            initialRotation.y + angle,
            initialRotation.z
        );

        // Detect player
        bool detected = CanSeePlayer();

        if (detected && !isPlayerDetected)
        {
            Debug.Log("Player detected!");
        }

        if (!detected && isPlayerDetected)
        {
            Debug.Log("Player lost!");
        }

        isPlayerDetected = detected;
    }

    bool CanSeePlayer()
    {
        if (player == null)
            return false;

        // Check if player has a collider
        if (playerCollider == null)
        {
            playerCollider =
                player.GetComponentInChildren<Collider>();
        }

        if (playerCollider == null ||
            !playerCollider.enabled ||
            !playerCollider.gameObject.activeInHierarchy)
        {
            return false;
        }

        // Aim toward center of player's collision box
        Vector3 target = playerCollider.bounds.center;

        Vector3 direction = target - transform.position;
        float distance = direction.magnitude;

        // Check Range
        if (distance > viewDistance || distance < 0.01f)
            return false;

        // Check view angle
        float angle = Vector3.Angle(
            transform.forward,
            direction
        );

        if (angle > viewAngle / 2f)
            return false;

        // Check line of sight
        if (Physics.Raycast(
            transform.position,
            direction.normalized,
            out RaycastHit hit,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore))
        {
            // Check whether we hit the player
            if (hit.collider == playerCollider ||
                hit.transform == player ||
                hit.transform.IsChildOf(player))
            {
                Debug.DrawLine(
                    transform.position,
                    hit.point,
                    Color.red
                );

                return true;
            }
        }

        return false;
    }

    // Draw detection cone in Unity Scene view
    void OnDrawGizmos()
    {
        if (!showFieldOfView)
            return;

        Gizmos.color =
            Application.isPlaying && isPlayerDetected
            ? detectedColor
            : coneColor;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        float halfAngle = viewAngle / 2f;

        // Center line showing detection range
        Gizmos.DrawRay(
            origin,
            forward * viewDistance
        );

        // Starting direction of cone
        Vector3 edgeDirection = Quaternion.AngleAxis(
            halfAngle, transform.up
        ) * forward;

        Vector3 previousPoint = Vector3.zero;

        for (int i = 0; i <= coneSegments; i++)
        {
            float rotation = (360f * i) / coneSegments;

            // Rotate around forward axis
            Vector3 direction = Quaternion.AngleAxis(
                rotation, forward
            ) * edgeDirection;

            Vector3 point =
                origin + direction * viewDistance;

            if (i > 0)
            {
                // Draw circular edge
                Gizmos.DrawLine(previousPoint, point);
            }

            // Draw lines from camera to cone edge
            if (i % 4 == 0)
            {
                Gizmos.DrawLine(origin, point);
            }

            previousPoint = point;
        }
    }

    // Access detection status from other scripts
    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }
}
