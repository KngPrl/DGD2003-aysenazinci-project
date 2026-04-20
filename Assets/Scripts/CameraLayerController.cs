using UnityEngine;
using Unity.Cinemachine;

public class PlayerBodyVisibility : MonoBehaviour
{
    public CinemachineCamera cam1;
    public CinemachineCamera cam2;
    public CinemachineCamera cam3;

    private Camera mainCam;
    private int playerLayerMask;

    void Start()
    {
        mainCam = Camera.main;
        playerLayerMask = 1 << LayerMask.NameToLayer("PlayerBody");
    }

    void Update()
    {
        if (cam1 == null || cam2 == null || cam3 == null) return;

        // Detect active camera via PRIORITY (Unity 6 safe method)
        if (cam1.Priority > cam2.Priority && cam1.Priority > cam3.Priority)
        {
            HideBody();
        }
        else
        {
            ShowBody();
        }
    }

    void HideBody()
    {
        mainCam.cullingMask &= ~playerLayerMask;
    }

    void ShowBody()
    {
        mainCam.cullingMask |= playerLayerMask;
    }
}