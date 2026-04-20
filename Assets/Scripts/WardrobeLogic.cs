using UnityEngine;

public class WardrobeLogic : MonoBehaviour
{
    [Header("Teleport Points")]
    public Transform hidePosition;
    public Transform exitPosition;

    [Header("Camera Management")]
    public GameObject cam1;
    public GameObject cam2;

    [Header("Equipment")]
    public GameObject flashlightObject;

    private GameObject player;
    private PlayerInteraction playerScript;
    private CharacterMovement moveScript; // Targets your NEW movement script
    private bool isHiding = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<PlayerInteraction>();
            // This finds the new script you just added
            moveScript = player.GetComponent<CharacterMovement>();
        }
    }

    public void ToggleHide()
    {
        if (player == null) return;

        if (!isHiding)
        {
            EnterWardrobe();
        }
        else
        {
            ExitWardrobe();
        }
    }

    void EnterWardrobe()
    {
        // 1. FREEZE PLAYER (Disables your new movement script)
        if (moveScript != null) moveScript.enabled = false;

        // 2. Teleport
        player.transform.position = hidePosition.position;
        player.transform.rotation = hidePosition.rotation;

        // 3. Camera & Flashlight Logic
        if (cam1 != null) cam1.SetActive(true);
        if (cam2 != null) cam2.SetActive(false);
        if (flashlightObject != null) flashlightObject.SetActive(false);

        isHiding = true;
        if (playerScript != null) playerScript.isCurrentlyHidden = true;
    }

    void ExitWardrobe()
    {
        // 1. Teleport back out
        player.transform.position = exitPosition.position;

        // 2. UNFREEZE PLAYER (Re-enables movement)
        if (moveScript != null) moveScript.enabled = true;

        // 3. Restore View
        if (cam2 != null) cam2.SetActive(true);
        if (flashlightObject != null) flashlightObject.SetActive(true);

        isHiding = false;
        if (playerScript != null) playerScript.isCurrentlyHidden = false;
    }
}