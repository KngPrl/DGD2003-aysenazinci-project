using UnityEngine;
using System.IO;

[System.Serializable]
public class PlayerPositionData
{
    public float x;
    public float y;
    public float z;
}

public class ClosetHiding : MonoBehaviour
{
    [Header("Nokta Ayarlari")]
    [SerializeField] private Transform closetPoint; // Dolabýn içi (Oyuncu buraya ýþýnlanacak)
    [SerializeField] private Transform exitPoint;   // Dolabýn dýþý (Oyuncu çýkýnca buraya ýþýnlanacak)

    [Header("UI Ayari (Opsiyonel)")]
    [SerializeField] private GameObject hidingPromptUI; // Ekranda belirecek "H tuþuna bas" yazýsý/görseli

    private bool isPlayerNearby = false;
    private bool isHiding = false;
    private GameObject playerObj = null;
    private CharacterController playerController = null;

    void Start()
    {
        // Oyun baþýnda UI yazýsý varsa gizle
        if (hidingPromptUI != null) hidingPromptUI.SetActive(false);
    }

    void Update()
    {
        // Oyuncu dolaba yakýnsa ve H tuþuna basarsa
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.H))
        {
            if (!isHiding)
            {
                HidePlayer();
            }
            else
            {
                ExitCloset();
            }
        }
    }

    private void HidePlayer()
    {
        isHiding = true;

        // UI simgesini kapat (Zaten dolabýn içine girdik)
        if (hidingPromptUI != null) hidingPromptUI.SetActive(false);

        // 1. JSON VE PLAYERPREFS: Eski konumu kaydet
        SaveLocationToJSON(playerObj.transform.position);

        // 2. HAREKETÝ DURDURMA: CharacterController'ý kapatýyoruz ki oyuncu hareket edemesin
        if (playerController != null) playerController.enabled = false;

        // 3. IÞINLAMA VE DÖNDÜRME: Dolap içine ýþýnla ve arkasýný döndür (Yüzü dýþarý baksýn)
        playerObj.transform.position = closetPoint.position;
        playerObj.transform.rotation = closetPoint.rotation * Quaternion.Euler(0, 180, 0);

        // 4. DÜÞMANDAN GÝZLENME: Karakterin etiketini deðiþtiriyoruz.
        // Düþman artýk onu "Player" olarak göremediði için kovalamayý býrakacak!
        playerObj.tag = "Untagged";
    }

    private void ExitCloset()
    {
        isHiding = false;

        // JSON'dan eski pozisyonu geri oku
        Vector3 oldPosition = LoadLocationFromJSON();

        // Eski konuma veya çýkýþ noktasýna geri ýþýnla
        if (oldPosition != Vector3.zero)
        {
            playerObj.transform.position = oldPosition;
        }
        else if (exitPoint != null)
        {
            playerObj.transform.position = exitPoint.position;
        }

        // Karakter kontrolcüsünü geri aç (Oyuncu tekrar hareket edebilir)
        if (playerController != null) playerController.enabled = true;

        // Etiketi eski haline getir ki düþman tekrar kovalamaya baþlayabilsin
        playerObj.tag = "Player";

        // Dolaptan çýkýnca UI simgesini tekrar göster (Hala tetikleyicinin içindeyiz)
        if (hidingPromptUI != null) hidingPromptUI.SetActive(true);
    }

    private void SaveLocationToJSON(Vector3 position)
    {
        PlayerPositionData data = new PlayerPositionData { x = position.x, y = position.y, z = position.z };
        string jsonString = JsonUtility.ToJson(data);
        PlayerPrefs.SetString("SavedPlayerPos", jsonString);
        PlayerPrefs.Save();
    }

    private Vector3 LoadLocationFromJSON()
    {
        if (PlayerPrefs.HasKey("SavedPlayerPos"))
        {
            string jsonString = PlayerPrefs.GetString("SavedPlayerPos");
            PlayerPositionData data = JsonUtility.FromJson<PlayerPositionData>(jsonString);
            return new Vector3(data.x, data.y, data.z);
        }
        return Vector3.zero;
    }

    // --- TETÝKLEYÝCÝ ALANINA GÝRÝÞ ÇIKIÞ ---
    private void OnTriggerEnter(Collider other)
    {
        // Gelen objenin katmaný veya etiketi Player ise
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerObj = other.gameObject;
            playerController = other.GetComponent<CharacterController>();

            // "H tuþuna bas" UI simgesini ekranda göster
            if (hidingPromptUI != null) hidingPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isHiding)
        {
            isPlayerNearby = false;
            playerObj = null;
            playerController = null;

            // Alandan çýkýnca UI simgesini gizle
            if (hidingPromptUI != null) hidingPromptUI.SetActive(false);
        }
    }
}