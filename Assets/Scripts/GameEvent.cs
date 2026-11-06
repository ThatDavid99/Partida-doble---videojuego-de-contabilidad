using System;
using UnityEngine;

public static class GameEvents
{
    // Evento que se dispara cuando se completa un reto exitosamente
    public static event Action<string> OnChallengeCompleted;

    // NUEVOS EVENTOS - Sistema de recolección
    public static event Action<string, int, int> OnCollectablePickedUp; // (collectableId, currentCount, totalRequired)
    public static event Action<string> OnAllCollectablesGathered; // (collectableId)

    // Método para publicar que un reto se completó
    public static void ChallengeCompleted(string challengeId)
    {
        Debug.Log($"[GameEvents] Publicando: Reto '{challengeId}' completado");
        OnChallengeCompleted?.Invoke(challengeId);
    }

    // Método para publicar que se recogió un objeto
    public static void CollectablePickedUp(string collectableId, int currentCount, int totalRequired)
    {
        Debug.Log($"[GameEvents] Objeto recogido: {collectableId} ({currentCount}/{totalRequired})");
        OnCollectablePickedUp?.Invoke(collectableId, currentCount, totalRequired);
    }

    // Método para publicar que se recogieron todos los objetos
    public static void AllCollectablesGathered(string collectableId)
    {
        Debug.Log($"[GameEvents] ¡Todos los objetos '{collectableId}' recolectados!");
        OnAllCollectablesGathered?.Invoke(collectableId);
    }
}