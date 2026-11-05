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
        if (playerTouching)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                // Apply a push force based on position and strength
                rigidbody.AddForce(direction * strength, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            playerTouching = true;
            interactionPromptText.text = "Press E to push";
            interactionPrompt.SetActive(true);
            setPushDirection(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            setPushDirection(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            playerTouching = false;
            interactionPrompt.SetActive(false);
        }
    }

    private void setPushDirection(Collider other)
    {
        direction = (transform.position - other.transform.position).normalized;
    }
}
