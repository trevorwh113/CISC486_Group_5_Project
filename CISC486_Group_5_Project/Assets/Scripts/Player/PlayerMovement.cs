using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Controls player movements and actions.
 * Requires a reference to the player object.
 */
public class PlayerMovement : MonoBehaviour
{
    /**
     * An enum representing the possible movement states of the player.
     */
    enum MovementPhase
    {
        Moving,
        Stopping,
        Stopped
    }

    [Header("Player Information")]
    public Rigidbody rigidbody;

    [Header("Movement")]
    [Tooltip("The basic walking speed for the player. The great the value, the faster they move (range: 0+).")]
    public float speed;

    [Header("Rotation")]
    [Tooltip("How quickly the player turns in degrees per second.")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Stopping")]
    [Tooltip("The rate at which the player slows after releasing input. The greater the value, the faster they stop (range: 0-1).")]
    public float decceleration;
    [Tooltip("The distance the player moves after releasing input. The greater the value, the further they slide (range: 0+).")]
    public float stoppingDistance;

    [Header("Hiding")]
    [Tooltip("The child object containing the player's model and collider.")]
    [SerializeField] private GameObject playerModel;

    /** The latest movement vector calculated from input */
    private Vector3 curMovement = new Vector3(0,0,0);
    /** The previous movement vector calculated from input */
    private Vector3 prevMovement = new Vector3(0,0,0);
    /** The current movement phase of the player. Default to Stopped. */
    private MovementPhase phase = MovementPhase.Stopped;
    /** The remaining distance to travel until stoppingDistance is reached */
    private float remStoppingDistance = 0;
    /** The slowest arrival speed at which point decceleration stops */
    private float MIN_STOPPING_SPEED = 0.1f;
    /** Whether the player is currently inside a hiding spot */
    private bool isHidden;
    /** The position where the player will reappear after leaving a hiding spot */
    private Vector3 hidingExitPosition;

    /** Allows other systems, such as guards, to check whether the player is hidden */
    public bool IsHidden => isHidden;

    public void Start()
    {
        
    }

    public void OnMove(InputValue inputValue)
    {
        Vector2 newMovement = inputValue.Get<Vector2>();

        // A non-zero movement input causes a hidden player to leave their hiding spot.
        if (isHidden)
        {
            if (newMovement.sqrMagnitude == 0)
            {
                return;
            }

            reappear();
        }

        // Save the previous movement for reference when stopping
        prevMovement = curMovement;
    
        // Calculate the current movement based on the input direction and the player speed
        newMovement = newMovement.normalized; // normalize to keep only the directional info
        curMovement = new Vector3(newMovement.x, 0, newMovement.y) * Time.fixedDeltaTime * speed;
    }

    void FixedUpdate()
    {
        // If there is a movement from input, apply it to the player
        if (curMovement.magnitude != 0)
        {
            applyMovement();
        }

        // If the input was released (magnitude == 0), slow down over stoppingDistance until stopped
        else if (phase != MovementPhase.Stopped)
        {
            dampenArrival();
        }
    }

    public Vector3 getCurrentPosition()
    {
        return transform.position;
    }

    public Vector3 getProjectedPosition()
    {
        return transform.position + curMovement;
    }

    /**
     * Hides the player and stores the position where they will reappear.
     */
    public void Hide(Vector3 exitPosition)
    {
        isHidden = true;
        hidingExitPosition = exitPosition;

        // Stop all existing movement before disabling the player's physical presence.
        curMovement = Vector3.zero;
        prevMovement = Vector3.zero;
        phase = MovementPhase.Stopped;
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.isKinematic = true;
        playerModel.SetActive(false);
    }

    /**
     * Restores the player at the hiding spot's exit position.
     */
    private void reappear()
    {
        rigidbody.position = hidingExitPosition;
        playerModel.SetActive(true);
        rigidbody.isKinematic = false;
        isHidden = false;
    }

    private void applyMovement()
    {
        // Begin movement and apply the current movement
        phase = MovementPhase.Moving;

        // Smoothly rotate the player's forward direction toward their movement.
        Quaternion targetRotation = Quaternion.LookRotation(curMovement.normalized, Vector3.up);
        Quaternion nextRotation = Quaternion.RotateTowards(
            rigidbody.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
        rigidbody.MoveRotation(nextRotation);

        Vector3 target = transform.position + curMovement;
        rigidbody.MovePosition(target);
    }

    private void dampenArrival()
    {
        // If input was just released, begin stopping and reset the stop point
        if (phase == MovementPhase.Moving)
        {
            phase = MovementPhase.Stopping;
            remStoppingDistance = stoppingDistance;
        }
        
        // If we haven't reached the stop point, approach one step towards it
        if (remStoppingDistance > 0)
        {
            // Use a decreasing step size proportional to the remaining distance.
            // If the size is too small, use MIN_STOPPING_SPEED instead to ensure 
            // the player will eventually reach or surpass the stop point.
            float curStoppingDistance = Mathf.Max(MIN_STOPPING_SPEED, remStoppingDistance * decceleration);

            // Move in the direction of the previous movement, capped to the step size
            Vector3 target = transform.position + Vector3.ClampMagnitude(prevMovement, curStoppingDistance);
            rigidbody.MovePosition(target);

            // Update the remaining distance to the stop point
            remStoppingDistance -= curStoppingDistance;
        }

        // If we've reached the stop point, stop moving
        else
        {
            phase = MovementPhase.Stopped;
        }
    }

}
