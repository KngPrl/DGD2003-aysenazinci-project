using UnityEngine;

public class FlashlightCameraSync : MonoBehaviour
{
    public GameObject cam1; // Drag your First Person Camera here
    public GameObject flashlightModel; // Drag the Flashlight's Mesh/Light here

    void Update()
    {
        if (cam1 == null || flashlightModel == null) return;

        // If Cam 1 is active, show the flashlight. Otherwise, hide it.
        if (cam1.activeInHierarchy)
        {
            flashlightModel.SetActive(true);
        }
        else
        {
            flashlightModel.SetActive(false);
        }
    }
}