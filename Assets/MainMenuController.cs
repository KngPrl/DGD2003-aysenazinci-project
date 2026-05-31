using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // Required for reading the JSON file

public class MainMenuController : MonoBehaviour
{
    [Header("Fallback Settings")]
    [Tooltip("Used only if a JSON save file cannot be found")]
    [SerializeField] private string fallbackSceneName = "MainScene";

    private string saveFilePath;

    private void Start()
    {
        // Points to the exact same file path the elevator used
        saveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    // Assign this function to your UI Restart Button's OnClick() event
    public void RestartGame()
    {
        // Check if the JSON save file actually exists
        if (File.Exists(saveFilePath))
        {
            // 1. Read the raw text string out of the JSON file
            string jsonString = File.ReadAllText(saveFilePath);

            // 2. Turn the text data back into a usable C# object
            GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonString);

            Debug.Log("Loaded from JSON! Reloading scene: " + loadedData.lastTargetScene);

            // 3. Load the scene that was saved inside the JSON data
            SceneManager.LoadScene(loadedData.lastTargetScene);
        }
        else
        {
            // If something went wrong or file doesn't exist, load the fallback scene
            Debug.LogWarning("JSON file not found! Using fallback scene.");
            SceneManager.LoadScene(fallbackSceneName);
        }
    }

    // Assign this function to your UI Quit Button's OnClick() event
    public void QuitGame()
    {
        Debug.Log("Quit button pressed!");
        Application.Quit();
    }
}