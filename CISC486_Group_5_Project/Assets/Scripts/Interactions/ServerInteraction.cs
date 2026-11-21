using System.Collections;
using TMPro;
using UnityEngine;

/**
 * Handles timed server interactions and awards one point when the player
 * remains in range for the full interaction duration.
 */
public class ServerInteraction : MonoBehaviour, IInteractable
{
    [Header("Countdown")]
    [Tooltip("How long the player must remain in range to complete the interaction.")]
    [SerializeField] private float interactionDuration = 1f;

    [Header("HUD")]
    [Tooltip("The world-space Canvas displayed above the server.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used for the interaction instruction and countdown.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;

    /** Prevents this server instance from granting more than one point */
    private bool hasGrantedPoint;
    /** The active timer, or null when no interaction is in progress */
    private Coroutine interactionCoroutine;

    public Transform InteractionTransform => transform;
    public bool CanInteract => !hasGrantedPoint;
    public bool IsInteracting => interactionCoroutine != null;

    private void Awake()
    {
        // The prompt should only be visible while the player is in range.
        interactionPrompt.SetActive(false);
    }

    public void SetSelected(bool selected)
    {
        if (selected && !hasGrantedPoint)
        {
            interactionPromptText.text = "Press E to steal";
        }

        interactionPrompt.SetActive(selected && !hasGrantedPoint);
    }

    public void Interact(PlayerMovement player)
    {
        if (!hasGrantedPoint &&
            interactionCoroutine == null &&
            (!GameManager.Instance.IsFinalServer(this) ||
             GameManager.Instance.FinalDoorUnlocked))
        {
            interactionCoroutine = StartCoroutine(InteractionTimer());
        }
    }

    private IEnumerator InteractionTimer()
    {
        // Display the remaining interaction time once per frame.
        float remainingTime = interactionDuration;
        audioSource.Play();

        while (remainingTime > 0f)
        {
            interactionPromptText.text = $"Stealing: {remainingTime:0.0}";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Completing the timer consumes this server's point permanently.
        hasGrantedPoint = true;
        //if the server is not final, generate password digit
        if (!GameManager.Instance.IsFinalServer(this))
            GameManager.Instance.CollectCodeDigit();
        ScoreManager.Instance.RegisterServerScored();
        interactionPrompt.SetActive(false);
        interactionCoroutine = null;
        audioSource.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            other.attachedRigidbody
                .GetComponent<PlayerInteraction>()
                .Register(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player"))
        {
            other.attachedRigidbody
                .GetComponent<PlayerInteraction>()
                .Unregister(this);

            // Leaving early cancels the timer without consuming the server's point.
            if (interactionCoroutine != null)
            {
                StopCoroutine(interactionCoroutine);
                interactionCoroutine = null;
                audioSource.Stop();
            }
        }
    }
}
