using UnityEngine;
using Unity.Cinemachine; // Standard for Unity 6

public class CameraSwitchernew : MonoBehaviour
{
    [Header("Drag your V-cams here")]
    public CinemachineVirtualCameraBase cam1; // First Person (Main)
    public CinemachineVirtualCameraBase cam2; // Third Person
    public CinemachineVirtualCameraBase cam3; // Top Down / Alternate
    public Camera mainCam;

    void Start()
    {
        // Force cam1 to be the active camera at the very start
        SwitchTo(1);
    }

    void Update()
    {
        // Use 1, 2, and 3 keys to switch
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchTo(2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchTo(3);
    }

    public void SwitchTo(int camNumber)
    {
        // Reset all to low priority
        cam1.Priority = 10;
        cam2.Priority = 10;
        cam3.Priority = 10;

        // Set the chosen one to high priority so the Brain slides to it
        if (camNumber == 1) cam1.Priority = 20;
        if (camNumber == 2) cam2.Priority = 20;
        if (camNumber == 3) cam3.Priority = 20;

        Debug.Log("Switched to Cam " + camNumber);
    }
}
