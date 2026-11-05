using UnityEngine;
using UnityEngine.Events;

/**
* All event signals and listeners that need to be shared
* between multiple scripts
*/
public static class GameEvents
{
    public static UnityEvent guardReachedPlayer = new UnityEvent();
    /** Signals that every server in the current level has been scored */
    public static UnityEvent allServersScored = new UnityEvent();
    /** Signals that the player reached the active exit and won the level */
    public static UnityEvent playerWon = new UnityEvent();

    // Signals that a player was spotted and invokes last known position of the player
    public static UnityEvent<Vector3> cameraSpottedPlayer = new UnityEvent<Vector3>();
}