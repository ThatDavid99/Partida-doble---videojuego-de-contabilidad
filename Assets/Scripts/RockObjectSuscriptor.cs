using UnityEngine;

public class ChallengeLock : MonoBehaviour
{
    [Header("Lock Settings")]
    [SerializeField] private string requiredChallengeId = "Challenge1";
    [SerializeField] private bool destroyOnUnlock = false; // Si es true, destruye el objeto. Si es false, solo lo desactiva
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private GameObject unlockEffect; // Partículas o efecto visual opcional
    [SerializeField] private AudioClip unlockSound; // Sonido opcional

    private void OnEnable()
    {
        // Suscribirse al evento cuando el objeto se activa
        GameEvents.OnChallengeCompleted += OnChallengeCompleted;
        Debug.Log($"[ChallengeLock] '{gameObject.name}' suscrito a eventos de retos");
    }

    private void OnDisable()
    {
        // Desuscribirse cuando el objeto se desactiva (importante para evitar memory leaks)
        GameEvents.OnChallengeCompleted -= OnChallengeCompleted;
        Debug.Log($"[ChallengeLock] '{gameObject.name}' desuscrito de eventos");
    }

    private void OnChallengeCompleted(string challengeId)
    {
        // Verificar si el reto completado es el que desbloquea este objeto
        if (challengeId == requiredChallengeId)
        {
            Debug.Log($"[ChallengeLock] '{gameObject.name}' recibió notificación: Desbloqueando!");
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
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}