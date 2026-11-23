using TMPro;
using UnityEngine;

/**
 * Allows the player to enter a hiding spot by pressing E while inside its
 * interaction trigger.
 */
public class HidingSpotInteraction : MonoBehaviour, IInteractable
{
    [Header("Exit Point")]
    [Tooltip("The position where the player will reappear after leaving the hiding spot.")]
    [SerializeField] private Transform exitPoint;

    [Header("HUD")]
    [Tooltip("The world-space Canvas displayed above the hiding spot.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used to display the current interaction instruction.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("The sound clip to play when entering or existing a box.")]
    [SerializeField] private AudioClip wooshSound;

    /** The player currently inside the interaction trigger */
    private PlayerMovement player;
    /** The interaction controller belonging to the nearby player */
    private PlayerInteraction playerInteraction;
    /** Whether the player is close enough to use this hiding spot */
    private bool playerInRange;
    /** Whether the player is hidden and waiting for movement input */
    private bool waitingForExit;

    public Transform InteractionTransform => transform;
    public bool CanInteract => playerInRange && !waitingForExit;
    public bool IsInteracting => waitingForExit;
    public int InteractionPriority => 0;

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
                playerInteraction = null;
                audioSource.PlayOneShot(wooshSound);
            }

            return;
        }
    }

    public void SetSelected(bool selected)
    {
        if (waitingForExit)
        {
            return;
        }

        if (selected)
        {
            interactionPromptText.text = "Press E to hide";
        }

        interactionPrompt.SetActive(selected);
    }

    public void Interact(PlayerMovement interactingPlayer)
    {
        if (!playerInRange || waitingForExit)
        {
            return;
        }

        // Set this state before hiding because disabling the player collider
        // causes it to leave the interaction trigger.
        waitingForExit = true;
        playerInRange = false;
        interactionPromptText.text = "Move to exit";
        player = interactingPlayer;
        player.Hide(exitPoint.position);
        audioSource.PlayOneShot(wooshSound);
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (!waitingForExit && other.attachedRigidbody.CompareTag("Player"))
        {
            player = other.attachedRigidbody.GetComponent<PlayerMovement>();
            playerInteraction = other.attachedRigidbody.GetComponent<PlayerInteraction>();
            playerInRange = true;
            playerInteraction.Register(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the prompt when a visible player walks away without interacting.
        if (!waitingForExit && other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = false;
            playerInteraction.Unregister(this);
            player = null;
            playerInteraction = null;
        }
        else if (waitingForExit && other.attachedRigidbody.CompareTag("Player"))
        {
            // Keep the exit instruction visible while removing this spot from
            // the nearby candidates after the hidden collider is disabled.
            playerInteraction.Unregister(this, false);
        }
    }
}
