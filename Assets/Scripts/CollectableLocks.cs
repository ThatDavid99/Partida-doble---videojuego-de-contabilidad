using UnityEngine;

public class CollectableLock : MonoBehaviour
{
    [Header("Lock Settings")]
    [SerializeField] private string requiredCollectableId = "MazeItems";
    [SerializeField] private bool destroyOnUnlock = false; // Si es true, destruye el objeto. Si es false, solo lo desactiva
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private GameObject unlockEffect; // Partículas o efecto visual opcional
    [SerializeField] private AudioClip unlockSound; // Sonido opcional

    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto se activa
        GameEvents.OnAllCollectablesGathered += OnAllCollectablesGathered;
        Debug.Log($"[CollectableLock] '{gameObject.name}' suscrito a eventos de recolección");
    }

    private void OnDisable()
    {
        // Desuscribirse cuando el objeto se desactiva
        GameEvents.OnAllCollectablesGathered -= OnAllCollectablesGathered;
        Debug.Log($"[CollectableLock] '{gameObject.name}' desuscrito de eventos");
    }

    private void OnAllCollectablesGathered(string collectableId)
    {
        // Verificar si el grupo recolectado es el que desbloquea este objeto
        if (collectableId == requiredCollectableId)
        {
            Debug.Log($"[CollectableLock] '{gameObject.name}' recibió notificación: ¡Todos los objetos recolectados! Desbloqueando...");
            Unlock();
        }
    }

    private void Unlock()
    {
        // Efecto visual opcional
        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, transform.position, Quaternion.identity);
        }

        // Sonido opcional
        if (unlockSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, Camera.main.transform.position);
        }

        // Desactivar o destruir el objeto
        if (destroyOnUnlock)
        {
            Debug.Log($"[CollectableLock] Destruyendo '{gameObject.name}'");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log($"[CollectableLock] Desactivando '{gameObject.name}'");
            gameObject.SetActive(false);
        }
    }
}