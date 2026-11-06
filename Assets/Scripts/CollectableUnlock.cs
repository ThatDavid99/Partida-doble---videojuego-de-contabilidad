using UnityEngine;

public class CollectableUnlock : MonoBehaviour
{
    [Header("Unlock Settings")]
    [SerializeField] private string requiredCollectableId = "MazeItems";
    [SerializeField] private GameObject objectToActivate;
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private GameObject unlockEffect;
    [SerializeField] private AudioClip unlockSound;

    private void OnEnable()
    {
        GameEvents.OnAllCollectablesGathered += OnAllCollectablesGathered;
        Debug.Log($"[CollectableUnlock] Suscrito a eventos de recolección");
    }

    private void OnDisable()
    {
        GameEvents.OnAllCollectablesGathered -= OnAllCollectablesGathered;
    }

    private void OnAllCollectablesGathered(string collectableId)
    {
        if (collectableId == requiredCollectableId)
        {
            Debug.Log($"[CollectableUnlock] ¡Todos los objetos recolectados! Activando...");
            Unlock();
        }
    }

    private void Unlock()
    {
        if (objectToActivate == null)
        {
            Debug.LogWarning("[CollectableUnlock] No hay objeto asignado para activar");
            return;
        }

        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, objectToActivate.transform.position, Quaternion.identity);
        }

        if (unlockSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, Camera.main.transform.position);
        }

        objectToActivate.SetActive(true);
        Debug.Log($"[CollectableUnlock] Objeto '{objectToActivate.name}' activado");
    }
}