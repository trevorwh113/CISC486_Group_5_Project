using UnityEngine;
using UnityEngine.InputSystem;

/**
Controls player movements and actions.
Requires a reference to the player object.
*/
public class PlayerControls : MonoBehaviour
{
    public float speed;
    public float drag;
    public float stopRatio;
    public Rigidbody rigidbody;

    private Vector3 curMovement = new Vector3(0,0,0);
    private Vector3 prevMovement = new Vector3(0,0,0);
    private bool isMoving = false;

    public void OnMove(InputValue inputValue)
    {
        Vector2 newMovement = inputValue.Get<Vector2>();
        prevMovement = curMovement;
        curMovement = new Vector3(newMovement.x, 0, newMovement.y) * Time.fixedDeltaTime * speed;
    }

    void FixedUpdate()
    {
        if (curMovement.magnitude != 0)
        {
            applyMovement();
        }
        else if (isMoving)
        {
            dampenArrival();
        }
    }

    private void applyMovement()
    {
        isMoving = true;
        Vector3 target = transform.position + curMovement;
        Debug.Log("Moving to:" + target);
        rigidbody.MovePosition(target);
    }

    private void dampenArrival()
    {
        Vector3 stoppingMovement = prevMovement / drag;
        Vector3 target = transform.position + stoppingMovement;
        Debug.Log("Stopping to: " + target);
        rigidbody.MovePosition(target);
        prevMovement = stoppingMovement;

        if (stoppingMovement.magnitude < stopRatio)
        {
            isMoving = false;
            Debug.Log("Stopped");
        }
    }

/**
    Actions:
        1 - Moving -> Apply the movement exactly as is
        2 - Stopping -> Dampen the movement 
        3 - Stopped -> Do nothing (i.e. apply the 0 movement)
*/
}
