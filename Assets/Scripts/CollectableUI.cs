using UnityEngine;
using TMPro;

public class CollectableUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject counterPanel; // Panel que contiene el contador
    [SerializeField] private TextMeshProUGUI counterText; // Texto que muestra "X/5"

    [Header("Settings")]
    [SerializeField] private string collectableId = "MazeItems"; // ID del grupo a mostrar

    private void Start()
    {
        // Desactivar el panel al inicio
        if (counterPanel != null)
        {
            counterPanel.SetActive(false);
        }

        // Suscribirse a eventos
        GameEvents.OnCollectablePickedUp += OnCollectablePickedUp;
    }

    private void OnDestroy()
    {
        // Desuscribirse
        GameEvents.OnCollectablePickedUp -= OnCollectablePickedUp;
    }

    private void OnCollectablePickedUp(string id, int currentCount, int totalRequired)
    {
        // Solo actualizar si es el grupo correcto
        if (id == collectableId)
        {
            UpdateCounter(currentCount, totalRequired);
        }
    }

    private void UpdateCounter(int current, int total)
    {
        if (counterText != null)
        {
            counterText.text = $"{current}/{total}";
        }
    }

    // Método público para activar/desactivar el panel (llamado por CollectableZone)
    public void ShowPanel(bool show)
    {
        if (counterPanel != null)
        {
            counterPanel.SetActive(show);

            // Si se muestra, actualizar con los valores actuales
            if (show && CollectableManager.Instance != null)
            {
                int current = CollectableManager.Instance.GetCollectedCount(collectableId);
                int total = CollectableManager.Instance.GetTotalRequired(collectableId);
                UpdateCounter(current, total);
            }
        }
    }
}