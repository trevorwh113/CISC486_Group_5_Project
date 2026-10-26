using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Allows the player to enter a hiding spot by pressing E while inside its
 * interaction trigger.
 */
public class HidingSpotInteraction : MonoBehaviour
{
    [Tooltip("The position where the player will reappear after leaving the hiding spot.")]
    [SerializeField] private Transform exitPoint;
    [Tooltip("The world-space Canvas displayed above the hiding spot.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used to display the current interaction instruction.")]
    [SerializeField] private TMP_Text interactionPromptText;

    /** The player currently inside the interaction trigger */
    private PlayerMovement player;
    /** Whether the player is close enough to use this hiding spot */
    private bool playerInRange;
    /** Whether the player is hidden and waiting for movement input */
    private bool waitingForExit;

    private void Awake()
    {
        // The prompt should only appear while the player can use this hiding spot.
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // Hide the prompt after movement input restores the player.
        if (waitingForExit)
        {
            if (!player.IsHidden)
            {
                waitingForExit = false;
                interactionPrompt.SetActive(false);
                player = null;
            }

            return;
        }

        // Enter the hiding spot when the nearby player presses E.
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Set this state before hiding because disabling the player collider
            // causes it to leave the interaction trigger.
            waitingForExit = true;
            playerInRange = false;
            interactionPromptText.text = "Move to exit";
            player.Hide(exitPoint.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (!waitingForExit && other.attachedRigidbody.CompareTag("Player"))
        {
            player = other.attachedRigidbody.GetComponent<PlayerMovement>();
            playerInRange = true;
            interactionPromptText.text = "Press E to hide";
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the prompt when a visible player walks away without interacting.
        if (!waitingForExit && other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = false;
            interactionPrompt.SetActive(false);
            player = null;
        }
    }
}
