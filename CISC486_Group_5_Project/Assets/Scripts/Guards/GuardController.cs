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

    void Start()
    {
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
        // Stop chasing a player if they have left the vision distance
        // TODO: When objects to hide exist, incorporate that into this decision making
        if (phase == GuardMovementPhase.Chasing && vision.playerOutsideDistance())
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
}
