using UnityEngine;

/**
 * This script controls the various behaviours of a standard
 * janitor. It selects which behaviour to execute at a given time,
 * and when to switch between them. Behaviours include pathfollowing,
 * fleeing, vision, etc.
 */
public class JanitorController : MonoBehaviour
{
    /**
     * The movement actions/phases of guards. Does not include,
     * vision, since the guard can be watching while moving.
     */
    enum JanitorMovementPhase
    {
        Fleeing,
        Patrolling,
    }

    [Header("Movement Behaviours")]
    [Tooltip("The Flee behaviour script attached to this guard.")]
    public Flee flee;
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
    private JanitorMovementPhase phase;

    void Start()
    {
        // The guard starts off by watching while patrolling
        phase = JanitorMovementPhase.Patrolling;
        patrol.startFollowingPath();
        vision.startWatching();
    }

    void FixedUpdate()
    {
        // Start fleeing from a player if they have been spotted
        if (phase != JanitorMovementPhase.Fleeing && vision.playerSpotted())
        {
            startFleeingAndStopWatching();
        }
        // Stop feeling when the player is outside of a certain distance
        if (phase == JanitorMovementPhase.Fleeing &&
            vision.playerOutsideDistance())
        {
            stopFleeingAndResumePath();
        }
    }

    private void startFleeingAndStopWatching()
    {
        phase = JanitorMovementPhase.Fleeing;
        // Before fleeing, stop following the path
        patrol.stopFollowingPath();
        // Disable active watching while fleeing, as it is unecessary
        vision.stopWatching();
        flee.startFlee();
    }

    private void stopFleeingAndResumePath()
    {
        phase = JanitorMovementPhase.Patrolling;
        flee.stopFlee();
        // Resume path following and watching out for the player
        vision.startWatching();
        patrol.resumeFollowingPath();
    }
}
