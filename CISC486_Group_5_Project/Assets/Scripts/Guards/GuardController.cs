using UnityEngine;

/**
 * This script controls the various behaviours of a standard
 * guard. It selects which behaviour to execute at a given time,
 * and when to switch between them. Behaviours include pathfollowing,
 * chasing, vision, etc.
 */
public class GuardController : MonoBehaviour
{
    /**
     * The movement actions/phases of guards. Does not include,
     * vision, since the guard can be watching while moving.
     */
    enum GuardMovementPhase
    {
        Chasing,
        Patrolling
    }

    [Header("Movement Behaviours")]
    [Tooltip("The Chase behaviour script attached to this guard.")]
    public Chase chase;
    [Tooltip("The Pathfollowing behaviour script attached to this guard.")]
    public Pathfollowing patrol;

    [Header("Other Behaviours")]
    [Tooltip("The VisionRange behaviour script attached to this guard.")]
    public VisionRange vision;

    [Header("Player Information")]
    [Tooltip("The movement component of the player, used to find its position.")]
    public PlayerMovement player;

    /** 
     * The current movement action of the guard. Only one movement
     * behaviour can be perfomed at once.
     */
    private GuardMovementPhase phase;
    /** Prevents guards from abandoning the chase after every server is scored */
    private bool lockdownActive;

    void Start()
    {
        // Detect when the player has been reached during chase
        // TODO: Will cause issue if the player remaisn in the line of sight
        // TODO: Could cause issue if the player is caught before pathforming occurs
        GameEvents.guardReachedPlayer.AddListener(playerCaught);
        // Begin permanent pursuit when the level objective is completed.
        GameEvents.allServersScored.AddListener(activateLockdown);

        // The guard starts off by watching while patrolling
        phase = GuardMovementPhase.Patrolling;
        patrol.startFollowingPath();
        vision.startWatching();
    }

    void FixedUpdate()
    {
        // Start chasing a player if they have been spotted
        if (phase != GuardMovementPhase.Chasing && vision.playerSpotted())
        {
            startChasingAndStopWatching();
        }
        // Normal chases end outside the vision distance; lockdown chases never end.
        if (!lockdownActive &&
            phase == GuardMovementPhase.Chasing &&
            vision.playerOutsideDistance())
        {
            stopChasingAndResumePath();
        }
    }

    private void startChasingAndStopWatching()
    {
        phase = GuardMovementPhase.Chasing;
        // Before chasing, stop following the path
        patrol.stopFollowingPath();
        // Disable active watching while chasing, as it is unecessary
        vision.stopWatching();
        chase.startChase();
    }

    private void stopChasingAndResumePath()
    {
        phase = GuardMovementPhase.Patrolling;
        chase.stopChase();
        // Resume path following and watching out for the player
        vision.startWatching();
        patrol.resumeFollowingPath();
    }

    private void playerCaught()
    {
        // During lockdown, catching the player does not end permanent pursuit.
        if (lockdownActive)
        {
            return;
        }

        Debug.Log("Player has been caught! Stop chasing.");
        stopChasingAndResumePath();
    }

    /**
     * Stops patrol behavior and permanently gives this guard the player's location.
     */
    private void activateLockdown()
    {
        lockdownActive = true;
        chase.enableLockdownTracking();

        if (phase != GuardMovementPhase.Chasing)
        {
            startChasingAndStopWatching();
        }
    }

    void OnDestroy()
    {
        GameEvents.guardReachedPlayer.RemoveListener(playerCaught);
        GameEvents.allServersScored.RemoveListener(activateLockdown);
    }
}
