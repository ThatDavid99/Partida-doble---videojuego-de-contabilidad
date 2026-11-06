using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Collectable Settings")]
    [SerializeField] private string collectableId = "MazeItems"; // ID del grupo de objetos
    
    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt; // Prefab del "Presiona E"
    
    [Header("Visual Feedback (Opcional)")]
    [SerializeField] private GameObject collectEffect; // Efecto al recoger
    [SerializeField] private AudioClip collectSound; // Sonido al recoger

    private bool playerInRange = false;
    private bool collected = false;
    private GameObject promptInstance;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            promptInstance = Instantiate(interactionPrompt, transform.position + new Vector3(-0.1f, 1.2f, 0f), Quaternion.identity);
            promptInstance.transform.SetParent(transform);
            promptInstance.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && !collected && Input.GetKeyDown(KeyCode.E))
        {
            Collect();
        }
    }

    private void Collect()
    {
        if (collected) return;

        collected = true;
        Debug.Log($"[CollectableItem] '{gameObject.name}' recogido");

        // Notificar al manager que se recogió un objeto
        CollectableManager manager = FindFirstObjectByType<CollectableManager>();
        if (manager != null)
        {
            manager.CollectItem(collectableId);
        }
        else
        {
            Debug.LogError("[CollectableItem] No se encontró CollectableManager en la escena!");
        }

        // Efectos visuales y sonoros
        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
        }

        if (collectSound != null && Camera.main != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, Camera.main.transform.position);
        }

        // Desactivar el objeto
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collected)
        {
            playerInRange = true;
            if (promptInstance != null)
            {
                promptInstance.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptInstance != null)
            {
                promptInstance.SetActive(false);
            }
        }
    }
}