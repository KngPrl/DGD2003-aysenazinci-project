using UnityEngine;
using UnityEngine.SceneManagement; // Sahne geçiþleri için zorunlu ad alaný

public class GameOverManager : MonoBehaviour
{
    [Header("Sahne Isimleri")]
    [SerializeField] private string mainMenuSceneName = "MainMenu"; // Ana menü sahnenin tam adý

    void Start()
    {
        // Oyun bittiðinde farenin ekranda rahatça hareket edebilmesi için kilidini açýyoruz
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // RESTART BUTONU ÝÇÝN FONKSÝYON
    public void RestartGame()
    {
        // Þu an aktif olan (oynanan) sahneyi bul ve sýfýrdan tekrar yükle
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    // MAIN MENU BUTONU ÝÇÝN FONKSÝYON
    public void GoToMainMenu()
    {
        // Buraya yazdýðýn isimdeki sahneye (Ana Menüye) geçiþ yapar
        SceneManager.LoadScene(mainMenuSceneName);
    }
}