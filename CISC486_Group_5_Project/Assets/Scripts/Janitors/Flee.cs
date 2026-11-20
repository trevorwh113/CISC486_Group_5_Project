using UnityEngine;
using UnityEngine.AI;

/**
 * Implements flee behaviour when attached to a janitor agent.
 * Must be enabled with startFlee() and disabled with stopFlee().
 */
public class Flee : MonoBehaviour
{
    [Header("Navigation")]
    [Tooltip("The NavMeshAgent component of this game object.")]
    public NavMeshAgent navAgent;
    [Tooltip("The movement component of the player to chase.")]
    public PlayerMovement player;

    [Header("Movement")]
    [Tooltip("The proportion of the based speed to increase while running from a target (range: 1+).")]
    public float fleeRate;

    /** The original speed of the agent while walking */
    private float walkSpeed;

    /** Indicates whether player chasing is activated */
    private bool isFleeing = false;

    void FixedUpdate()
    {
        // Run from the player except while they are hidden, even in lockdown
        if (isFleeing && !player.IsHidden)
        {
            fleeFromPlayer();
        }
    }

    /** 
     * Begin flee behaviour and double the agent's speed 
     */
    public void startFlee()
    {
        isFleeing = true;
        walkSpeed = navAgent.speed;
        navAgent.speed *= fleeRate;
    }

    /** 
     * Stops flee behaviour and reset the agent's speed
     */
    public void stopFlee()
    {
        isFleeing = false;
        navAgent.speed = walkSpeed;
        navAgent.SetDestination(transform.position);
    }

    /**
     * Calculate the player's projected position and move away from it
     */
    private void fleeFromPlayer()
    {
        Vector3 projectedPosition = player.getProjectedPosition();
        Vector3 projectedDistance = transform.position - projectedPosition;
        navAgent.SetDestination(transform.position + projectedDistance);
    }
}
