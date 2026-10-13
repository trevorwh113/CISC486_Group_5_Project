using UnityEngine;
using UnityEngine.InputSystem;

/**
Controls player movements and actions.
Requires a reference to the player object.
*/
public class PlayerControls : MonoBehaviour
{
    public float speed = 0;
    public Rigidbody rigidbody;

    private float movementX;
    private float movementY;
    
    public void OnMove(InputValue inputValue)
    {
        Vector2 newMovement = inputValue.Get<Vector2>();
        movementX = newMovement.x;
        movementY = newMovement.y;
    }

    void FixedUpdate()
    {
        Vector3 force = new Vector3(movementX, 0, movementY) * speed;
        rigidbody.AddForce(force);
    }
}
