using UnityEngine;
using UnityEngine.AI;

/**
 * Steers the given agent along a given path.
 */
public class Pathfollowing : MonoBehaviour
{
    [Header("Navigation")]
    [Tooltip("The NavMeshAgent component of this game object.")]
    public NavMeshAgent navAgent;
    [Tooltip("The Pathforming object to use as a path. Should be attached to another object.")]
    public Pathforming path;

    /** The current seek target on the path */
    private PathNode currentTarget;
    /** Indicates whether path following is activated */
    private bool isFollowingPath;

    void Start()
    {
        // Make sure to form the path before starting to use it
        path.constructPath();

        // Start following the path at the closest node
        resumeFollowingPath();
    }

    void FixedUpdate()
    {
        if (isFollowingPath && hasArrivedAtTarget())
        {
            // Set the target to the next point once we've arrived
            moveToNextPointOnPath();
        }
    }

    /**
     * Returns true if the agent is within the arrival radius of the current seek node
     */
    private bool hasArrivedAtTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.getPosition());
        return distanceToTarget <= currentTarget.seekRadius;
    }

    /**
     * Updates the currentTarget to the next node in the path and starts seeking to it
     */
    private void moveToNextPointOnPath()
    {
        isFollowingPath = true;
        // Use the next node in the path as the seek point
        currentTarget = path.getNextNodeInPath();
        navAgent.SetDestination(currentTarget.getPosition());
    }

    /**
     * Begins path following behaviour by seeking to the nearest node in the path
     */
    public void resumeFollowingPath()
    {
        isFollowingPath = true;
        // Use the closest node in the path as the seek point
        currentTarget = path.getClosestNodeInPath(transform.position);
        // If too close, skip it and go directly to the next point
        if (hasArrivedAtTarget())
        {
            currentTarget = path.getNextNodeInPath();
        }
        navAgent.SetDestination(currentTarget.getPosition());
    }

    /**
     * Stops all pathfollowing behaviour on the agent
     */
    public void stopFollowingPath()
    {
        isFollowingPath = false;
    }

    // TODO: Work this into a behaviour on the guard itself
    // IEnumerator stopAndWait()
    // {
    //     stopFollowingPath();
    //     yield return new WaitForSeconds(currentTarget.stopTime);
    //     moveToNextPointOnPath();
    // }
}
