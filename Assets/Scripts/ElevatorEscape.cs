using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // Required for writing the JSON file to your computer

public class ElevatorEscape : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string winSceneName = "YouWin";
    [Tooltip("Type the exact name of your main gameplay level scene here")]
    [SerializeField] private string currentGameplaySceneName = "MainScene";

    private bool isElevatorUnlocked = false;
    private string saveFilePath;

    private void Start()
    {
        // Sets the save file path to a safe, permanent folder on your computer
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    // This is called by your PaperWorks key object when touched
    public void UnlockElevator()
    {
        isElevatorUnlocked = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player stepped into the elevator AND has the key
        if (other.CompareTag("Player") && isElevatorUnlocked)
        {
            SaveProgressWithJSON();
            SceneManager.LoadScene(winSceneName);
        }
    }

    private void SaveProgressWithJSON()
    {
        // 1. Create a new data container and fill it with our current scene name
        GameSaveData data = new GameSaveData();
        data.lastTargetScene = currentGameplaySceneName;
        data.isObjectiveComplete = true;

        // 2. Convert that data into a JSON text string
        string jsonString = JsonUtility.ToJson(data, true);

        // 3. Write that text string into a physical file
        File.WriteAllText(saveFilePath, jsonString);
        Debug.Log("Progress saved to JSON path: " + saveFilePath);
    }
}