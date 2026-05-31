using UnityEngine;

public class EscapeObjective : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Drag your Elevator GameObject here")]
    [SerializeField] private ElevatorEscape elevatorScript;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player touched the objective
        if (other.CompareTag("Player"))
        {
            if (elevatorScript != null)
            {
                // Unlock the elevator escape
                elevatorScript.UnlockElevator();
                Debug.Log("Objective complete! Go to the elevator.");
            }
            else
            {
                Debug.LogError("Assign the Elevator to this script in the Inspector!");
            }

            // Turn off this object's visual model and collider so it disappears cleanly
            gameObject.SetActive(false);
        }
    }
}