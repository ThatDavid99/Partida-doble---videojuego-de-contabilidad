using UnityEngine;

public class CollectableZone : MonoBehaviour
{
    [Header("Zone Settings")]
    [SerializeField] private CollectableUI collectableUI; // Referencia al UI

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("[CollectableZone] Player entró en la zona - Mostrando UI");
            if (collectableUI != null)
            {
                collectableUI.ShowPanel(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("[CollectableZone] Player salió de la zona - Ocultando UI");
            if (collectableUI != null)
            {
                collectableUI.ShowPanel(false);
            }
        }
    }
}