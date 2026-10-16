using UnityEngine;
using UnityEngine.AI;

public class Pathfollowing : MonoBehaviour
{
    public NavMeshAgent navAgent;
    public Pathforming path;
    public float nodeRadius;
    private Vector3 currentTarget = new Vector3(0,0,0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        moveToNextPointOnPath();
    }

    private int i = 0;

    void FixedUpdate()
    {
        if (hasArrivedAtTarget())
        {
            Debug.Log("Arrived at point: " + i);
            moveToNextPointOnPath();
        }
    }

    private bool hasArrivedAtTarget()
    {
        return Vector3.Distance(transform.position, currentTarget) <= nodeRadius;
    }

    private void moveToNextPointOnPath()
    {
        currentTarget = path.getNextNodeInPath();
        navAgent.SetDestination(currentTarget);
        Debug.Log("Destination set to: " + currentTarget + "; " + ++i);
    }
}
