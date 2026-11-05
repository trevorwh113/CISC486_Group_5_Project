using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Handles push and slide mechanics for certain obstacles
 */
public class PushInteraction : MonoBehaviour
{
    [Header("Push strength")]
    [Tooltip("The strength of the push impulse.")]
    [SerializeField] private float strength = 1f;

    [Header("Label")]
    [Tooltip("The world-space Canvas displayed above the server.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used for the interaction instruction and countdown.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Obstacle Information")]
    [SerializeField] private Rigidbody rigidbody;

    /** Whether the player is currently inside the server's trigger */
    private bool playerTouching;
    /** The direction from player to obstacle */
    private Vector3 direction;

    private void Awake()
    {
        // The prompt should only be visible while the player is in range.
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        if (playerTouching && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Apply a push force based on position and strength
            rigidbody.AddForce(direction * strength, ForceMode.Impulse);
            Debug.Log("Pushing in direction: " + direction);
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (triggerIsPlayer(trigger))
        {
            playerTouching = true;
            interactionPromptText.text = "Press E to push";
            interactionPrompt.SetActive(true);
            setPushDirection(trigger);
        }
    }

    private void OnTriggerStay(Collider trigger)
    {
        if (triggerIsPlayer(trigger))
        {
            setPushDirection(trigger);
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (triggerIsPlayer(trigger))
        {
            playerTouching = false;
            interactionPrompt.SetActive(false);
        }
    }

    private void setPushDirection(Collider trigger)
    {
        direction = (transform.position - trigger.transform.position).normalized;
        direction.y = 0;
    }

    static private bool triggerIsPlayer(Collider trigger)
    {
        return trigger 
                && trigger.attachedRigidbody 
                && trigger.attachedRigidbody.CompareTag("Player");
    }
}
