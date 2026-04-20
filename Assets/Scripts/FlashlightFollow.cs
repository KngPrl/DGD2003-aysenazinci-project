using UnityEngine;

public class FlashlightFollow : MonoBehaviour
{
    public Transform targetCamera; // Drag your Main Camera here
    public Vector3 offset = new Vector3(0.3f, -0.2f, 0.5f); // Adjust to position it correctly

    void LateUpdate() // LateUpdate is smoother for following cameras
    {
        if (targetCamera == null) return;

        // Follow position
        transform.position = targetCamera.position + targetCamera.TransformDirection(offset);

        // Follow rotation
        transform.rotation = targetCamera.rotation;
    }
}