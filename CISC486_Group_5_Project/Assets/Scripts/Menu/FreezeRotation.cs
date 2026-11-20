using UnityEngine;

/**
 * Fixes the object's rotation in place regardless of to its parent
 */
public class FreezeRotation : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
}
