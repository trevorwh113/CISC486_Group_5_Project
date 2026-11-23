using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Implements a door that can be unlocked with a key.
 */
public class KeyDoorInteraction : MonoBehaviour
{
    [Header("Interaction Prompt")]
    [Tooltip("The world-space Canvas displayed above the locked door.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used to display the current interaction instruction.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Key")]
    [Tooltip("The integer denoting the ID of the required key.")]
    public int keyId;

    [Header("Animation")]
    [Tooltip("The speed with which the door descend after being unlocked.")]
    public float speed;
    [Tooltip("The rigidbody used to animate the door opening.")]
    public Rigidbody rigidbody;

    [Header("Sound Effects")]
    [Tooltip("The audio source component attached to this object.")]
    [SerializeField] private AudioSource audioSource;
    [Tooltip("The sound clip to play when unlocking a door succeeds.")]
    [SerializeField] private AudioClip unlockSuccess;
    [Tooltip("The sound clip to play when unlocking a door succeeds fails.")]
    [SerializeField] private AudioClip unlockFail;
    [Tooltip("The sound clip to play when opening a door.")]
    [SerializeField] private AudioClip doorOpens;

    /** Whether the player is close enough to use this hiding spot */
    private bool playerInRange = false;

    /** True if the player has picked up the key that unlocked this door */
    private bool keyAquired = false;

    /** True after the door has been unlocked once */
    private bool unlocked = false;

    private void Awake()
    {
        // The prompt should only appear while the player is near the door.
        interactionPrompt.SetActive(false);
    }

    private void Update()
    {
        // The player opens a door when nearby, holding the key, and pressing E
        if (!unlocked && playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (keyAquired)
            {
                // Mark the door as unlcoked and open it
                unlocked = true;
                interactionPrompt.SetActive(false);
                audioSource.PlayOneShot(unlockSuccess);
                audioSource.PlayOneShot(doorOpens);
                StartCoroutine("openDoor");
            }
            else
            {
                audioSource.PlayOneShot(unlockFail);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        if (!unlocked && isPlayer(other))
        {
            playerInRange = true;
            keyAquired = KeyManager.Instance.keyAquired(keyId);
            interactionPromptText.text = (keyAquired) ? "Press E to unlock" : "Key required";
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Hide the prompt when the player walks away without interacting
        if (isPlayer(other))
        {
            playerInRange = false;
            interactionPrompt.SetActive(false);
        }
    }

    /**
     * Open the door gradually by dropping into the ground at
     * the set speed. Disble the door once it is hidden as it will
     * never be needed again.
     */
    private IEnumerator openDoor()
    {
        // Calculate the object's height and descent increment based on speed
        float height = transform.position.y + transform.localScale.y * 2;
        float descentTime = height / (Time.fixedDeltaTime * speed);
        Vector3 openingIncrement = Vector3.up * Time.fixedDeltaTime * speed;

        // Move the object down by one increment each frame
        float seconds = 0;
        while (seconds < descentTime)
        {
            rigidbody.MovePosition(transform.position - openingIncrement);
            seconds++;
            yield return new WaitForFixedUpdate();
        }

        // Disable the object once it is out of sight, as it is not needed
        gameObject.SetActive(false);
    }

    /**
     * Checks that the given collider is the player. Since the tag is on
     * the rigidbody, need to ensure the collider has a rigidbody component
     * before checking (only seems to be required when the object with this
     * script itself has a rigidbody).
     */
    private bool isPlayer(Collider other)
    {
        return other && 
               other.attachedRigidbody && 
               other.attachedRigidbody.CompareTag("Player");
    }
}
