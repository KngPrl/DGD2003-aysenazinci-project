using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyAttackNew : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameOverSceneName = "GameOver";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("The enemy caught the player!");

        // --- TEACHER'S PLAYERPREFS REQUIREMENT ---
        // Save the current active scene's name into a memory slot called "LastPlayedScene"
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedScene", currentSceneName);
        PlayerPrefs.Save(); // Tells the computer to physically write the data down immediately

        // Load the game over screen
        SceneManager.LoadScene(gameOverSceneName);
    }
}