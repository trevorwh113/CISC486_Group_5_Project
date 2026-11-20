using System.Collections;
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
    [Tooltip("The duration in seconds after starting to flee until the agent becomes exhausted (range: >0).")]
    public float runDuration;
    [Tooltip("The duration in seconds it takes to resume flee behaviour after being exhausted (range: >0).")]
    public float recoveryDuration;
    [Tooltip("The percentage of a flee sequence at which the agent can move at the fleeRate. (range: 0-1).")]
    public float stamina;

    /** The original speed of the agent while walking */
    private float walkSpeed;

    /** Indicates whether player chasing is activated */
    private bool isFleeing = false;
    /** Indicates whether agent is exhausted from fleeing */
    private bool isExhausted = false;

    void FixedUpdate()
    {
        // Run from the player except when exhausted or when they are hiding
        if (isFleeing && !isExhausted && !player.IsHidden)
        {
            fleeFromPlayer();
        }
    }

    /** 
     * Begin flee behaviour and double the agent's speed. Start
     * a count down to become exhausted after some time.
     */
    public void startFlee()
    {
        isFleeing = true;
        isExhausted = false;
        walkSpeed = navAgent.speed;
        navAgent.speed *= fleeRate;
        StartCoroutine("goUntilExhausted");
    }

    /** 
     * Stops flee behaviour and reset the agent's speed
     */
    public void stopFlee()
    {
        isFleeing = false;
        isExhausted = false;
        navAgent.speed = walkSpeed;
        navAgent.SetDestination(transform.position);

        // Ensure coroutines are killed when flee is stopped
        StopCoroutine("goUntilExhausted");
        StopCoroutine("recoverThenResume");
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

    /**
     * Starts a timer for full speed running. When the timer
     * elapses, begins the recoveryDuration period. Incrementally slows 
     * the agent as they get exhausted.
     */
    private IEnumerator goUntilExhausted()
    {
        yield return new WaitForSeconds(runDuration * stamina);
        navAgent.speed = walkSpeed;
        yield return new WaitForSeconds(runDuration * (1-stamina));
        if (isFleeing)
        {
            isExhausted = true;
            StartCoroutine("recoverThenResume");
        }
    }

    /**
     * Starts a timer to recover. When it elapses, starts
     * full speed running again.
     */
    private IEnumerator recoverThenResume()
    {
        yield return new WaitForSeconds(recoveryDuration);
        if (isFleeing)
        {
            isExhausted = false;
            navAgent.speed = walkSpeed * fleeRate;
            StartCoroutine("goUntilExhausted");
        }
    }
}
