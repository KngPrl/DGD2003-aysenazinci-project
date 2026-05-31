using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO; // Required for JSON saving/loading file paths

public class MainMenuController : MonoBehaviour
{
    [Header("Fallback Settings")]
    [Tooltip("Type the exact name of your main gameplay level scene here")]
    [SerializeField] private string fallbackSceneName = "MainScene";

    private string jsonSaveFilePath;

    private void Start()
    {
        // --- CURSOR UNLOCKING MECHANIC ---
        // This forces the mouse to reappear and unlock from the center of the screen
        // so the player can hover over and click UI buttons cleanly.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Sets up the path for the JSON save file
        jsonSaveFilePath = Path.Combine(Application.persistentDataPath, "gamesave.json");
    }

    // ====================================================================
    // 1. RESTART FROM GAME OVER (Assign to Game Over Scene Restart Button)
    // ====================================================================
    public void RestartFromGameOver()
    {
        // Check if PlayerPrefs has the saved scene name from when the enemy caught you
        if (PlayerPrefs.HasKey("LastPlayedScene"))
        {
            string sceneToLoad = PlayerPrefs.GetString("LastPlayedScene");
            Debug.Log("PlayerPrefs loaded successfully! Loading scene: " + sceneToLoad);

            // Tells your computer to dump old memory to make room for your heavy scene asset
            System.GC.Collect();

            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("PlayerPrefs key not found! Defaulting to fallback scene.");
            System.GC.Collect();
            SceneManager.LoadScene(fallbackSceneName);
        }
    }

    // ====================================================================
    // 2. RESTART FROM WIN SCENE (Assign to YouWin Scene Restart Button)
    // ====================================================================
    public void RestartGame()
    {
        // Check if the JSON save file exists
        if (File.Exists(jsonSaveFilePath))
        {
            string jsonString = File.ReadAllText(jsonSaveFilePath);
            GameSaveData loadedData = JsonUtility.FromJson<GameSaveData>(jsonString);

            Debug.Log("Loaded from JSON! Target scene: " + loadedData.lastTargetScene);

            // Tells your computer to dump old memory to make room for your heavy scene asset
            System.GC.Collect();

            SceneManager.LoadScene(loadedData.lastTargetScene);
        }
        else
        {
            Debug.LogWarning("JSON file not found! Using fallback scene.");
            System.GC.Collect();
            SceneManager.LoadScene(fallbackSceneName);
        }
    }

    // ====================================================================
    // 3. QUIT APPLICATION BUTTON
    // ====================================================================
    public void QuitGame()
    {
        Debug.Log("Quit button pressed!");
        Application.Quit();
    }
}