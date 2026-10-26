using UnityEngine;

/**
 * Signals a level win when the player reaches the active exit trigger.
 */
public class WinZone : MonoBehaviour
{
    /** Prevents a player with multiple colliders from invoking the event repeatedly */
    private bool winTriggered;

    private void OnTriggerEnter(Collider other)
    {
        // The player's collider is on a child, so check the root object's tag.
        if (!winTriggered && other.transform.root.CompareTag("Player"))
        {
            winTriggered = true;
            GameEvents.playerWon.Invoke();
        }
    }
}
