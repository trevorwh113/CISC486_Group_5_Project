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

    [Header("Stopping")]
    [Tooltip("The rate at which the player slows after releasing input. The greater the value, the faster they stop (range: 0-1).")]
    public float decceleration;
    [Tooltip("The distance the player moves after releasing input. The greater the value, the further they slide (range: 0+).")]
    public float stoppingDistance;

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

    public void OnMove(InputValue inputValue)
    {
        // Save the previous movement for reference when stopping
        prevMovement = curMovement;
    
        // Calculate the current movement based on the input direction and the player speed
        Vector2 newMovement = inputValue.Get<Vector2>();
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

    private void applyMovement()
    {
        // Begin movement and apply the current movement
        phase = MovementPhase.Moving;
        Vector3 target = transform.position + curMovement;
        // Debug.Log("Moving to:" + target);
        rigidbody.MovePosition(target);
    }

    private void dampenArrival()
    {
        // If input was just released, begin stopping and reset the stop point
        if (phase == MovementPhase.Moving)
        {
            phase = MovementPhase.Stopping;
            remStoppingDistance = stoppingDistance;
            // Debug.Log("Begin Stopping at distance: " + remStoppingDistance);
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
            // Debug.Log("Stopping step: " + target);
            rigidbody.MovePosition(target);

            // Update the remaining distance to the stop point
            remStoppingDistance -= curStoppingDistance;
        }

        // If we've reached the stop point, stop moving
        else
        {
            phase = MovementPhase.Stopped;
            // Debug.Log("Stopped at: " + transform.position);
        }
    }
}
