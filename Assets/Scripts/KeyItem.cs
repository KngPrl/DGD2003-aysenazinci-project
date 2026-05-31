using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Anahtara dokunan kiþi "Player" tag'ine sahipse
        if (other.CompareTag("Player"))
        {
            // Oyuncunun üzerindeki envanter sistemine "anahtarý aldýn" uyarýsý gönder
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.HasKey = true;
                Debug.Log("Anahtar toplandi!");

                // Anahtar objesini sahneden yok et
                Destroy(gameObject);
            }
        }
    }
}