using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

/**
 * Handles push mechanics for obstacles. The player holds down
 * a button to push. While pushing, the player can only move in
 * one direction and movement is slowed.
 */
public class PushInteraction : MonoBehaviour
{
    [Header("Label")]
    [Tooltip("The world-space Canvas displayed above the server.")]
    [SerializeField] private GameObject interactionPrompt;
    [Tooltip("The text used for the interaction instruction and countdown.")]
    [SerializeField] private TMP_Text interactionPromptText;

    [Header("Obstacle Information")]
    [SerializeField] private Rigidbody rigidbody;

    private PlayerMovement player;

    private void Awake()
    {
        // The prompt should only be visible while the player is in range.
        interactionPrompt.SetActive(false);

        // Make the object immovable by default
        rigidbody.isKinematic = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerIsPlayer(other))
        {
            player = other.attachedRigidbody.GetComponent<PlayerMovement>();
            // Display the interaction text
            interactionPromptText.text = "Hold E to push";
            interactionPrompt.SetActive(true);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (triggerIsPlayer(other))
        {
            if (Keyboard.current.eKey.isPressed)
            {
                // Object should only be affect by forces when E is pressed
                rigidbody.isKinematic = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (triggerIsPlayer(other))
        {
            // Make the object immovable again
            rigidbody.isKinematic = true;

            // Hide the interaction text
            interactionPrompt.SetActive(false);
        }
    }

    /*
     * Returns true if the colliding object is the player
     */
    static private bool triggerIsPlayer(Collider trigger)
    {
        // The player's collider is on a child, so check its attached root Rigidbody.
        return trigger 
                && trigger.attachedRigidbody 
                && trigger.attachedRigidbody.CompareTag("Player");
    }
}
