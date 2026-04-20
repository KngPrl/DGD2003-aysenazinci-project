using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 5f;
    public UnityEvent onHideAction;

    // We add this to track if we are already hidden
    [HideInInspector] public bool isCurrentlyHidden = false;

    void Update()
    {
        if (Keyboard.current.hKey.wasPressedThisFrame || Input.GetKeyDown(KeyCode.H))
        {
            // IF HIDDEN: Just exit immediately (No raycast needed!)
            if (isCurrentlyHidden)
            {
                onHideAction.Invoke();
            }
            // IF NOT HIDDEN: Use raycast to find the wardrobe
            else
            {
                PerformRaycast();
            }
        }
    }

    void PerformRaycast()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("Ignore Raycast");

        if (Physics.Raycast(ray.origin, ray.direction, out hit, interactionDistance, layerMask))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                onHideAction.Invoke();
                // We will set isCurrentlyHidden to true inside the Wardrobe script
            }
        }
    }
}