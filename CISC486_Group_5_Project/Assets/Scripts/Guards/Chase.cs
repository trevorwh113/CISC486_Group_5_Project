using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    [Header("Navigation")]
    [Tooltip("The NavMeshAgent component of this game object.")]
    public NavMeshAgent navAgent;
    [Tooltip("The movement component of the player to chase.")]
    public PlayerMovement player;

    [Header("Movement")]
    [Tooltip("The proportion of the based speed to increase while chasing a target (range: 1+).")]
    public float chaseRate;

    [Header("Stopping")]
    [Tooltip("How close to the player the enemy gets before stopping (range: 0+).")]
    public float stoppingDistance;

    /** The original speed of the agent while walking */
    private float walkSpeed;

    /** Indicates whether player chasing is activated */
    private bool isChasing = false;

    void FixedUpdate()
    {
        if (isChasing)
        {
            chasePlayer();
        }
    }

    /** 
     * Begin chase behaviour and double the agent's speed 
     */
    public void beginChase()
    {
        isChasing = true;
        walkSpeed = navAgent.speed;
        navAgent.speed *= chaseRate;
    }

    /** 
     * Stops chase behaviour and reset the agent's speed
     */
    public void stopChase()
    {
        isChasing = false;
        navAgent.speed = walkSpeed;
        navAgent.SetDestination(transform.position);
    }

    /**
     * Calculate the player's projected position and set it as the destination
     */
    private void chasePlayer()
    {
        Vector3 projectedPosition = player.getProjectedPosition();

        // Player has been caught if we are within a certain range
        if (Vector3.Distance(transform.position, projectedPosition) < stoppingDistance)
        {
            // TODO: Take desired action. For now, just stop moving but keep chasing
            navAgent.SetDestination(transform.position);
        }
        else
        {
            navAgent.SetDestination(projectedPosition);
        }
    }
}
