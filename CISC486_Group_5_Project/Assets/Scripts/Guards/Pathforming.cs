using UnityEngine;
using System.Collections.Generic;

public class Pathforming : MonoBehaviour
{    
    private LinkedList<Transform> nodes = new LinkedList<Transform>();
    
    void Start()
    {
        foreach (Transform node in transform)
        {
            nodes.AddLast(node);
        }
    }

    // TODO: Remove this debug drawing of the path
    Vector3 prev = new Vector3(0,0,0);

    void FixedUpdate()
    {
        // TODO: Remove this debug drawing of the path
        Vector3 target = getNextNodeInPath();
        Debug.Log("Drawing line to: " + target);
        Debug.DrawLine(prev, target, Color.red, 60);            
        prev = target;
    }

    public Vector3 getNextNodeInPath()
    {
        // The next node is the first in the queue
        Transform nextNode = nodes.First.Value;

        // Circulate the node back to the end of the queue
        nodes.RemoveFirst();
        nodes.AddLast(nextNode);
    
        // Return the position of the next node
        return nextNode.position;
    }
}
