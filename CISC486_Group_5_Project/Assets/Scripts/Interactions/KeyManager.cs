using System.Collections.Generic;
using TMPro;
using UnityEngine;

/**
 * Stores the player's keys and keeps the score UI synchronized.
 */
public class KeyManager : MonoBehaviour
{
    /** Shared scene instance used by interactable objects */
    public static KeyManager Instance;

    // [Tooltip("The UI text used to display the held keys.")]
    // [SerializeField] private TMP_Text keyText;

    /** The list of held and remaining keys */
    private Dictionary<int, bool> allKeys = new Dictionary<int, bool>();

    private void Awake()
    {
        // Register this scene's manager and initialize its display.
        Instance = this;
        UpdateDisplay();
    }

    private void Start()
    {
        // Build the list of keys that exist from the number of key pickup scripts
        KeyPickupInteraction[] keys = FindObjectsByType<KeyPickupInteraction>();
        foreach (KeyPickupInteraction key in keys)
        {
            allKeys.Add(key.keyId, false);
        }
    }

    /**
     * Sets the key status as picked up (true).
     */
    public void PickUpKey(int key)
    {
        if (allKeys.ContainsKey(key))
        {
            allKeys[key] = true;
            UpdateDisplay();
        }
    }

    /**
     * Refreshes the UI to match the held keys.
     */
    private void UpdateDisplay()
    {
        // keyText.text = "Keys: " + allKeys;
        // TODO: Implement the graphical display of collected keys
    }

    /**
     * Allows other scripts to check if a key has been aquired by the player
     */
    public bool keyAquired(int key)
    {
        return allKeys.ContainsKey(key) && allKeys[key];
    }
}