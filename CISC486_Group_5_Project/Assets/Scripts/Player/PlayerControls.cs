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
    private Vector3 curMovement = new Vector3(0,0,0);

    public void OnMove(InputValue inputValue)
    {        
        Vector2 newMovement = inputValue.Get<Vector2>();
        curMovement = new Vector3(newMovement.x, 0, newMovement.y);
    }

    void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + curMovement * Time.fixedDeltaTime * speed);
    }
}
