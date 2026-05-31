using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorController : MonoBehaviour
{
    [SerializeField] private Animator myDoor = null;

    private void OnTriggerEnter(Collider other)
    {
        // Gelen objenin oyuncu olduðundan emin oluyoruz
        if (other.CompareTag("Player"))
        {
            // 1. Kapýyý açma animasyonunu bir kez oynat
            if (myDoor != null)
            {
                myDoor.Play("DoorOpenNew", 0, 0.0f);
            }

            // 2. Bu tetikleyici kutuyu (Box Collider) tamamen yok et!
            // Böylece oyuncu içinden tekrar geçse bile kod asla bir daha tetiklenmez.
            Destroy(gameObject);
        }
    }
}