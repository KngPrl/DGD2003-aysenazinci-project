using UnityEngine;
using UnityEngine.AddressableAssets; // Addressables için zorunlu kütüphane

public class TriggerLockedDoorController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    [Header("Anahtar Ayarlari")]
    [SerializeField] private Transform keySpawnPoint; // Anahtarýn nerede doðacaðýný belirleyen boþ bir GameObject

    private void Start()
    {
        // OYUN BAÞLADIÐINDA: Anahtarý Addressables üzerinden dinamik olarak çaðýrýyoruz
        // "SchoolKey" dikey olarak iþaretlediðimiz Addressable ismidir
        Addressables.InstantiateAsync("SchoolKey", keySpawnPoint.position, keySpawnPoint.rotation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Oyuncunun envanter koduna eriþiyoruz
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null && inventory.HasKey)
            {
                // 1. Eðer anahtarý varsa kapýyý aç
                if (myDoor != null)
                {
                    myDoor.Play("DoorOpenNew", 0, 0.0f);
                }

                // 2. Anahtarý kullandýðý için envanterden düþebiliriz (isteðe baðlý)
                inventory.HasKey = false;

                // 3. Kapý açýldý, tetikleyici kutuyu tamamen yok et
                Destroy(gameObject);
            }
            else
            {
                // Anahtarý yoksa konsola yazdýr (Buraya ekrana "Anahtar Gerekli" yazan bir UI da baðlayabilirsin)
                Debug.Log("Bu kapi kilitli! Once anahtari bulmalisin.");
            }
        }
    }
}