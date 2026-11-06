using UnityEngine;

public class ChallengeUnlock : MonoBehaviour
{
    [Header("Unlock Settings")]
    [SerializeField] private string requiredChallengeId = "Challenge1";
    [SerializeField] private GameObject objectToActivate; // El objeto que se activará
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private GameObject unlockEffect; // Partículas o efecto visual opcional
    [SerializeField] private AudioClip unlockSound; // Sonido opcional

    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto se activa
        GameEvents.OnChallengeCompleted += OnChallengeCompleted;
        Debug.Log($"[ChallengeUnlock] '{gameObject.name}' suscrito a eventos de retos");
    }

    private void OnDisable()
    {
        // Desuscribirse cuando el objeto se desactiva
        GameEvents.OnChallengeCompleted -= OnChallengeCompleted;
        Debug.Log($"[ChallengeUnlock] '{gameObject.name}' desuscrito de eventos");
    }

    private void OnChallengeCompleted(string challengeId)
    {
        // Verificar si el reto completado es el que activa este objeto
        if (challengeId == requiredChallengeId)
        {
            Debug.Log($"[ChallengeUnlock] '{gameObject.name}' recibió notificación: Activando objeto!");
            Unlock();
        }
    }

    private void Unlock()
    {
        if (objectToActivate == null)
        {
            Debug.LogWarning($"[ChallengeUnlock] No hay objeto asignado para activar en '{gameObject.name}'");
            return;
        }

        // Efecto visual opcional
        if (unlockEffect != null)
        {
            Instantiate(unlockEffect, objectToActivate.transform.position, Quaternion.identity);
        }

        // Sonido opcional
        if (unlockSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(unlockSound, Camera.main.transform.position);
        }

        // Activar el objeto
        objectToActivate.SetActive(true);
        Debug.Log($"[ChallengeUnlock] Objeto '{objectToActivate.name}' activado exitosamente");
    }
}