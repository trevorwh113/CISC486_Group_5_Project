using UnityEngine;
using System.Collections.Generic;

/**
 * Implements a circular queue to represent a path comprised of nodes. 
 * To build a path, attach this component to an empty object, and nest
 * objects with the PathNode component under it in the desired sequence.
 */
public class Pathforming : MonoBehaviour
{    
    /** A circular queue of nodes forming the path */
    private LinkedList<PathNode> nodes = new LinkedList<PathNode>();
    
    void Start()
    {
        // Forms the path from all PathNodes nested under the current object
        PathNode[] nodesArray = GetComponentsInChildren<PathNode>();
        foreach (PathNode node in nodesArray)
        {
            nodes.AddLast(node);
        }
    }

    /** 
     *Pops the next node in the queue and puts it at the end 
     */
    public PathNode getNextNodeInPath()
    {
        // The next node is the first in the queue
        PathNode nextNode = nodes.First.Value;

        // Circulate the node back to the end of the queue
        nodes.RemoveFirst();
        nodes.AddLast(nextNode);
    
        // Return the position of the next node
        return nextNode;
    }

    /** 
     * Finds the closest node to the position and circles the queue to put it at the end 
     */
    public PathNode getClosestNodeInPath(Vector3 position)
    {
        // Find the closest node by calculating distance to all nodes
        PathNode closestNode = nodes.First.Value;
        float closestDistance = Vector3.Distance(position, closestNode.getPosition());
        foreach (PathNode node in nodes)
        {
            float distanceToNode = Vector3.Distance(position, node.getPosition());
            if (distanceToNode <= closestDistance)
            {
                closestDistance = distanceToNode;
                closestNode = node;
            }
        }

        // Cycle the path until the desired node is next
        PathNode nextNode = getNextNodeInPath();
        while (nextNode != closestNode)
        {
            nextNode = getNextNodeInPath();
        }

        // Return the position of the desired node
        return closestNode;
    }
}
