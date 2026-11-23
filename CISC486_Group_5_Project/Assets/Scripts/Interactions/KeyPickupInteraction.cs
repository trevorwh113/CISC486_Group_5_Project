using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Allows the player to take a key from a guard or janitor.
 */
public class KeyPickupInteraction : MonoBehaviour
{
    [Header("Interaction Prompt")]
    [Tooltip("The world-space Canvas displayed above the janitor.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used to display the current interaction instruction.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Key")]
    [Tooltip("The integer denoting this key. Unlocks doors with matching key IDs.")]
    public int keyId;

    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("The sound clip to play when successfully stealing the key.")]
    [SerializeField] private AudioClip stealingSound;

    /** Whether the player is close enough to use this hiding spot */
    private bool playerInRange = false;

    /** True when the key has been stolen; interaction is no longer be possible */
    private bool keyTaken = false;

    private void Awake()
    {
        // The prompt should only appear while the player is near the janitor.
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // Player takes the key when nearby by pressing E
        if (!keyTaken && playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Mark the key as taken
            keyTaken = true;
            KeyManager.Instance.PickUpKey(keyId);
            audioSource.PlayOneShot(stealingSound);

            // Hide the prompt
            interactionPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (!keyTaken && other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = true;
            interactionPromptText.text = "Press E to steal";
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the prompt when the player walks away without interacting
        if (other.attachedRigidbody.CompareTag("Player"))
        {
            playerInRange = false;
            interactionPrompt.SetActive(false);
        }
    }
}
