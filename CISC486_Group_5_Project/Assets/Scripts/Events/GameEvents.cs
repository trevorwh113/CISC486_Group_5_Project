using UnityEngine;
using UnityEngine.Events;

/**
* All event signals and listeners that need to be shared
* between multiple scripts
*/
public static class GameEvents
{
    public static UnityEvent guardReachedPlayer = new UnityEvent();
}