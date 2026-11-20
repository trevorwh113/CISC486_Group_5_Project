using UnityEngine;
using UnityEngine.AI;

/**
 * Makes the agent wander within a given radius.
 */
public class Wander : MonoBehaviour
{
    [Header("Navigation")]
    [Tooltip("The NavMeshAgent component of this game object.")]
    public NavMeshAgent navAgent;

    [Header("Range")]
    [Tooltip("The radius representing the area in which the agent can wander.")]
    public float wanderRadius;

    [Header("Stopping")]
    [Tooltip("How close to the last known position the enemy gets before stopping (range: 0+).")]
    public float stoppingDistance;

    [Header("Movement")]
    [Tooltip("The speed factor of base speed while whandering. Usually (0,1] to slow the agent down.")]
    public float wanderRate;

    /** The original speed of the agent while walking */
    private float walkSpeed;

    /** The point around which wander behaviour is anchored. */
    private Vector3 anchorPoint;

    /** The current seek target around the anchor point */
    private Vector3 currentTarget;

    /** Indicates whether wander is activated  */
    private bool isWandering = false;


    void FixedUpdate()
    {
        // drawDebugWhiskers();

        if (isWandering && hasArrivedAtTarget())
        {
            // Set the target to another point once we've arrived
            moveToRandomPointInCircle();
        }
    }

    /**
     * Returns true if the agent is within the arrival radius of the current target
     */
    private bool hasArrivedAtTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
        return distanceToTarget <= stoppingDistance;
    }

    /**
     * Updates the currentTarget to a random point ahead of the agent and 
     * starts seeking to it. If the agent is current too far, returns to a 
     * point within the wander circle.
     */
    private void moveToRandomPointInCircle()
    {
        // Use the full agent's speed only on the first wander step, so that
        // it can get back to the circle quickly if it is far 
        if (isWandering)
        {
            navAgent.speed = walkSpeed * wanderRate;
        }
        isWandering = true;

        // Pick a spot on the circle to seek towards
        Vector2 variation = Random.onUnitCircle * wanderRadius;
        currentTarget = anchorPoint + new Vector3(variation.x, 0, variation.y);
        navAgent.SetDestination(currentTarget);
    }

    /**
     * Begins wandering by saving the anchor point then picking a target around it
     */
    public void startWandering()
    {
        // Uses the agent's current location as their permanent anchor point
        anchorPoint = transform.position;
        walkSpeed = navAgent.speed;

        // Start wandering around the anchor point
        isWandering = true;
        resumeWandering();
    }

    /**
     * Resumes wandering by seeking towards a point around the anchor
     */
    public void resumeWandering()
    {
        // Resume wandering by returning to a point around the anchor point
        moveToRandomPointInCircle();
    }

    /**
     * Stops all wandering behaviour on the agent
     */
    public void stopWandering()
    {
        navAgent.speed = walkSpeed;
        isWandering = false;
    }

    /**
     * Draws whiskers around the anchor point for debugging the wander radius
     */
    private void drawDebugWhiskers()
    {
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(0,0,1) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(0.71f,0,0.71f) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(1,0,0) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(0.71f,0,-0.71f) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(0,0,-1) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(-0.71f,0,-0.71f) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(-1,0,0) * wanderRadius, Color.red, 20f);
        Debug.DrawLine(anchorPoint, anchorPoint + new Vector3(-0.71f,0,0.71f) * wanderRadius, Color.red, 20f);
    }
}
