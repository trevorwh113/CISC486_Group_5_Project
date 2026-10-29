
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
    [SerializeField] private bool isPlayerDetected;

    [Header("Flashlight")]
    public Light flashlight;

    [Header("Tracking")]
    public float trackingSpeed = 120f;
    public float loseTargetDelay = 1f;
    public float returnToSweepSpeed = 180f;

    private float lastSeenTime = float.NegativeInfinity;
    private Vector3 lastKnownPlayerPosition;
    private bool returningToSweep;

    void Start()
    {
        initialRotation = transform.localEulerAngles;

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

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
        // Detect player
        bool detected = CanSeePlayer();

        if (detected)
        {
            lastSeenTime = Time.time;
            lastKnownPlayerPosition = playerCollider.bounds.center;
        }

        if (detected && !isPlayerDetected)
        {
            Debug.Log("Player detected!");
        }

        if (!detected && isPlayerDetected)
        {
            Debug.Log("Player lost!");
        }

        isPlayerDetected = detected;

        if (isPlayerDetected ||
            Time.time - lastSeenTime <= loseTargetDelay)
        {
            TrackPlayer(lastKnownPlayerPosition);
            returningToSweep = true;
        }
        else
        {
            // Sweep while no player detected
            SweepCamera();
        }

        if (flashlight != null)
        {
            flashlight.range = viewDistance;
            flashlight.spotAngle = viewAngle;
        }
    }

    void SweepCamera()
    {
        // Original sweeping calculation
        float angle = Mathf.Sin(Time.time * sweepSpeed)
                      * sweepAngle;

        Quaternion targetRotation = Quaternion.Euler(
            initialRotation.x,
            initialRotation.y + angle,
            initialRotation.z
        );

        // Smoothly return to the sweep after tracking
        if (returningToSweep)
        {
            transform.localRotation =
                Quaternion.RotateTowards(
                    transform.localRotation,
                    targetRotation,
                    returnToSweepSpeed * Time.deltaTime
                );

            if (Quaternion.Angle(
                transform.localRotation,
                targetRotation) < 0.1f)
            {
                returningToSweep = false;
            }
        }
        else
        {
            // Normal sweeping - unchanged
            transform.localRotation = targetRotation;
        }
    }

    void TrackPlayer(Vector3 targetPosition)
    {
        Vector3 direction =
            targetPosition - transform.position;

        // Convert direction into parent's local space
        if (transform.parent != null)
        {
            direction =
                transform.parent.InverseTransformDirection(
                    direction
                );
        }

        // Keep tracking horizontal (Y-axis rotation)
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Camera's original forward direction
        Vector3 initialForward =
            Quaternion.Euler(initialRotation)
            * Vector3.forward;

        initialForward.y = 0f;

        if (initialForward.sqrMagnitude < 0.001f)
            return;

        // Find how far to rotate toward player
        float targetAngle = Vector3.SignedAngle(
            initialForward,
            direction,
            Vector3.up
        );

        Quaternion targetRotation = Quaternion.Euler(
            initialRotation.x,
            initialRotation.y + targetAngle,
            initialRotation.z
        );

        // Smoothly rotate toward the player
        transform.localRotation =
            Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                trackingSpeed * Time.deltaTime
            );
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

        // Player is on Ignore Raycast (layer 2); include its layer
        int layerMask =
            Physics.DefaultRaycastLayers |
            (1 << player.gameObject.layer);

        // Check line of sight
        if (Physics.Raycast(
            transform.position,
            direction.normalized,
            out RaycastHit hit,
            distance,
            layerMask,
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
