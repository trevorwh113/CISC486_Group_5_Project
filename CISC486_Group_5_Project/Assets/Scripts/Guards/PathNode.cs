using UnityEngine;

/**
 * Represents a node along a path.
 */
public class PathNode : MonoBehaviour
{
    [Header("Arrival details")]
    [Tooltip("How far the agent must be from the node before moving to the next (value: >0).")]
    public float seekRadius = 1;

    /** 
     * Returns the position of the node as a Vector3 
     */
    public Vector3 getPosition()
    {
        return transform.position;
    }
}
