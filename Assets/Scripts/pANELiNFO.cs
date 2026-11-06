using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InformationBoard : MonoBehaviour
{
    [Header("Information")]
    [TextArea(4, 10)]
    [SerializeField] private string informationText;

    [Header("UI References")]
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private TextMeshProUGUI informationTextUI;
    [SerializeField] private Button closeButton;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;
    
    private bool playerInRange = false;
    private bool isPanelOpen = false;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            GameObject instance = Instantiate(interactionPrompt, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.SetActive(false);
            interactionPrompt = instance;
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        if (informationPanel != null)
        {
            informationPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isPanelOpen)
        {
            OpenPanel();
        }
    }

    private void OpenPanel()
    {
        isPanelOpen = true;

        if (informationPanel != null)
        {
            informationPanel.SetActive(true);
        }

        if (informationTextUI != null)
        {
            informationTextUI.text = informationText;
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Bloquear movimiento del jugador
        if (PlayerMovement.instance != null)
        {
            PlayerMovement.instance.enabled = false;
        }
    }

    private void ClosePanel()
    {
        isPanelOpen = false;

        if (informationPanel != null)
        {
            informationPanel.SetActive(false);
        }

        if (interactionPrompt != null && playerInRange)
        {
            interactionPrompt.SetActive(true);
        }

        // Reactivar movimiento del jugador
        if (PlayerMovement.instance != null)
        {
            PlayerMovement.instance.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null && !isPanelOpen)
            {
                interactionPrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            // Si el panel está abierto, cerrarlo
            if (isPanelOpen)
            {
                ClosePanel();
            }
        }
    }
}