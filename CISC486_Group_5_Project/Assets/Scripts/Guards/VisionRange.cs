using UnityEngine;

/**
 * Implements a triangular detection field and reporting
 * when the player enters the range and is not blocked by 
 * an obstacle. Must be enabled with startWatching() and 
 * can be disabled with stopWatching().
 */
public class VisionRange : MonoBehaviour
{
    [Header("Vision")]
    [Tooltip("Half the width of the vision triangle (values >0).")]
    public float visionRadius;
    [Tooltip("The furthest distance at which a player can be noticed (values >0).")]
    public float visionDistance;

    [Header("Player Information")]
    [Tooltip("The movement component of the player, used to find its position.")]
    public PlayerMovement player;

    /** True only if the player is both in range and not behind an obstacle */
    private bool isPlayerVisible = false;
    /** Allows to enable and disable checking if player is visible */
    private bool watchingForPlayer = false;

    void FixedUpdate()
    {
        if (watchingForPlayer)
        {
            // Hidden players cannot be detected, even when inside the vision range.
            isPlayerVisible = !player.IsHidden && isPlayerInRange() && isPlayerInLineOfSight();
        }
    }

    /**
     * Checks if the player is currently within the vision triangle
     */
    private bool isPlayerInRange()
    {
        // Determine the vision triangle formed by the distance and radius
        Vector3 pointA = transform.position;
        Vector3 furthestPoint = pointA + transform.TransformDirection(Vector3.forward) * visionDistance;
        Vector3 pointB = furthestPoint + transform.TransformDirection(Vector3.left) * visionRadius;
        Vector3 pointC = furthestPoint + transform.TransformDirection(Vector3.right) * visionRadius;

        // Get vectors A->B, A->C and A->Player
        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        Vector3 pointAtoPlayer = player.getCurrentPosition() - pointA;

        // Calculate dot products for barycentric coordinates
        float pointAtoPlayerDotAB = Vector3.Dot(pointAtoPlayer, sideAB);
        float pointAtoPlayerDotAC = Vector3.Dot(pointAtoPlayer, sideAC);
        float sideABdotAB = Vector3.Dot(sideAB, sideAB);
        float sideABdotAC = Vector3.Dot(sideAB, sideAC);
        float sideACdotAB = Vector3.Dot(sideAC, sideAB);
        float sideACdotAC = Vector3.Dot(sideAC, sideAC);

        // Calculate the barycentric coordinates u and v where player = A + u * AB + v * AC
        float denom = sideABdotAB * sideACdotAC - sideABdotAC * sideACdotAB;
        if (denom == 0)
        {
            Debug.Log("Division by zero when detecting player handled gracefully.");
            return false;
        }

        float u = (pointAtoPlayerDotAB * sideACdotAC - pointAtoPlayerDotAC * sideACdotAB) / denom;
        float v = (pointAtoPlayerDotAC * sideABdotAB - pointAtoPlayerDotAB * sideABdotAC) / denom;

        // If u or v is less than 0, p is behind edge side AB or AC
        // If u + v is more than 1, then p is past BC
        return u >= 0 && v >= 0 && u + v < 1;
    }

    /**
     * Checks that the player is not obscured by an obstacle 
     */
    private bool isPlayerInLineOfSight()
    {
        // Calculate the direction and distance to the player
        Vector3 direction = (player.getCurrentPosition() - transform.position).normalized;
        float distance = Vector3.Distance(player.getCurrentPosition(), transform.position);

        // Does the guard have unobstructed line of sight to the player?
        bool obstacleInLineOfSight = Physics.Raycast(transform.position, direction, distance);
        return !obstacleInLineOfSight;
    }

    /**
     * A public getter to allow other scripts to check if the player is in sight
     */
    public bool playerSpotted()
    {
        return !player.IsHidden && isPlayerVisible;
    }
    
    /**
     * A public getter for a weakened condition of whether the player is outside
     * the circle formed by the vision distance. Useful to allow agents to "see"
     * around corners, through obstacles, and behind themselves. 
     */
    public bool playerOutsideDistance()
    {
        // Treat hiding as leaving the chase range so guards stop pursuing the player.
        if (player.IsHidden)
        {
            return true;
        }

        float distance = Vector3.Distance(player.getCurrentPosition(), transform.position);
        return distance > visionDistance;
    }


    /**
     * Activates player detection. It is disabled until this method is called.
     */
    public void startWatching()
    {
        watchingForPlayer = true;
    }

    /**
     * Allows disabling player detection. For example, we may want to turn it
     * off while the guard is chasing the player to reduce the workload. 
     */
    public void stopWatching()
    {
        watchingForPlayer = false;
    }

    /**
     * A debug method to draw the vision triangle using its three points
     */
    private void debugDrawTriangle(Vector3 pointA, Vector3 pointB, Vector3 pointC)
    {
        Debug.DrawLine(pointA, pointB, Color.red);
        Debug.DrawLine(pointA, pointC, Color.red);
        Debug.DrawLine(pointB, pointC, Color.red);
    }
}
