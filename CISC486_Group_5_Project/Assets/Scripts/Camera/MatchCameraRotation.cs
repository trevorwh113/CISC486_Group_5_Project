using UnityEngine;

[DefaultExecutionOrder(100)]
public class MatchCameraRotation : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
