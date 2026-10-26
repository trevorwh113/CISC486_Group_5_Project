using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Handles timed server interactions and awards one point when the player
 * remains in range for the full interaction duration.
 */
public class ServerInteraction : MonoBehaviour
{
    [Tooltip("How long the player must remain in range to complete the interaction.")]
    [SerializeField] private float interactionDuration = 1f;
    [Tooltip("The world-space Canvas displayed above the server.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used for the interaction instruction and countdown.")]
    [SerializeField] private TMP_Text interactionPromptText;

    /** Whether the player is currently inside the server's trigger */
    private bool playerTouching;
    /** Prevents this server instance from granting more than one point */
    private bool hasGrantedPoint;
    /** The active timer, or null when no interaction is in progress */
    private Coroutine interactionCoroutine;

    private void Awake()
    {
        // The prompt should only be visible while the player is in range.
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // Start one interaction timer when the nearby player presses E.
        if (playerTouching &&
            !hasGrantedPoint &&
            interactionCoroutine == null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            interactionCoroutine = StartCoroutine(InteractionTimer());
        }
    }

    private IEnumerator InteractionTimer()
    {
        // Display the remaining interaction time once per frame.
        float remainingTime = interactionDuration;

        while (remainingTime > 0f)
        {
            interactionPromptText.text = $"Stealing: {remainingTime:0.0}";
            remainingTime -= Time.deltaTime;
            yield return null;
        }

        // Completing the timer consumes this server's point permanently.
        hasGrantedPoint = true;
        ScoreManager.Instance.RegisterServerScored();
        interactionPrompt.SetActive(false);
        interactionCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            playerTouching = true;

            if (!hasGrantedPoint)
            {
                interactionPromptText.text = "Press E to steal";
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            playerTouching = false;
            interactionPrompt.SetActive(false);

            // Leaving early cancels the timer without consuming the server's point.
            if (interactionCoroutine != null)
            {
                StopCoroutine(interactionCoroutine);
                interactionCoroutine = null;
            }
        }
    }
}
