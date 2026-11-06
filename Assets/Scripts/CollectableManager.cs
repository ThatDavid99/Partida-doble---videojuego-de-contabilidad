using UnityEngine;
using System.Collections.Generic;

public class CollectableManager : MonoBehaviour
{
    public static CollectableManager Instance { get; private set; }

    [Header("Collectable Groups")]
    [SerializeField] private List<CollectableGroup> collectableGroups = new List<CollectableGroup>();

    private Dictionary<string, int> collectedCounts = new Dictionary<string, int>();
    private Dictionary<string, bool> completedGroups = new Dictionary<string, bool>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Inicializar contadores
        foreach (var group in collectableGroups)
        {
            collectedCounts[group.collectableId] = 0;
            completedGroups[group.collectableId] = false;
        }
    }

    public void CollectItem(string collectableId)
    {
        if (!collectedCounts.ContainsKey(collectableId))
        {
            Debug.LogWarning($"[CollectableManager] ID desconocido: {collectableId}");
            return;
        }

        if (completedGroups[collectableId])
        {
            Debug.LogWarning($"[CollectableManager] Grupo '{collectableId}' ya completado");
            return;
        }

        collectedCounts[collectableId]++;

        int totalRequired = GetTotalRequired(collectableId);
        int currentCount = collectedCounts[collectableId];

        Debug.Log($"[CollectableManager] {collectableId}: {currentCount}/{totalRequired}");

        // Publicar evento de objeto recogido
        GameEvents.CollectablePickedUp(collectableId, currentCount, totalRequired);

        // Verificar si se completó el grupo
        if (currentCount >= totalRequired)
        {
            completedGroups[collectableId] = true;
            Debug.Log($"[CollectableManager] ¡Grupo '{collectableId}' completado!");
            GameEvents.AllCollectablesGathered(collectableId);
        }
    }

    public int GetCollectedCount(string collectableId)
    {
        return collectedCounts.ContainsKey(collectableId) ? collectedCounts[collectableId] : 0;
    }

    public int GetTotalRequired(string collectableId)
    {
        foreach (var group in collectableGroups)
        {
            if (group.collectableId == collectableId)
            {
                return group.totalRequired;
            }
        }
        return 0;
    }

    public bool IsGroupCompleted(string collectableId)
    {
        return completedGroups.ContainsKey(collectableId) && completedGroups[collectableId];
    }
}

[System.Serializable]
public class CollectableGroup
{
    public string collectableId = "MazeItems";
    public int totalRequired = 5;
}