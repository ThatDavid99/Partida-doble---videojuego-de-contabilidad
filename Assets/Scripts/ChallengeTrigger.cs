using UnityEngine;

public class ChallengeTrigger : MonoBehaviour
{
    [Header("Challenge")]
    [SerializeField] private ChallengeData challengeData;
    
    [Header("Obstacle")]
    [SerializeField] private GameObject obstacleToDestroy;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;
    private bool playerInRange = false;
    private bool challengeStarted = false;
    
    private PlayerMovement playerMovement;
    private Rigidbody2D playerRb;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            GameObject instance = Instantiate(interactionPrompt, transform.position + new Vector3(-0.1f, 1.2f, 1f), Quaternion.identity);
            instance.SetActive(false);
            interactionPrompt = instance;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !challengeStarted)
        {
            Debug.Log("Tecla E presionada, mostrando reto");
            ShowChallenge();
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    private void ShowChallenge()
    {
        if (challengeData == null)
        {
            Debug.LogError("ChallengeData no está asignado!");
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
        }

        if (ChallengeManager.Instance == null)
        {
            Debug.LogError("ChallengeManager.Instance es NULL! Buscando en la escena...");
            
            ChallengeManager manager = FindFirstObjectByType<ChallengeManager>();
            if (manager != null)
            {
                Debug.Log("ChallengeManager encontrado en: " + manager.gameObject.name);
                manager.ShowChallengePrompt(challengeData, this);
                challengeStarted = true;
            }
            else
            {
                Debug.LogError("No se encontró ningún ChallengeManager en la escena!");
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }
            }
        }
        else
        {
            Debug.Log("Mostrando panel de reto");
            ChallengeManager.Instance.ShowChallengePrompt(challengeData, this);
            challengeStarted = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entró en el trigger de: " + gameObject.name);
            playerInRange = true;
            playerMovement = collision.GetComponent<PlayerMovement>();
            playerRb = collision.GetComponent<Rigidbody2D>();
            
            if (playerMovement == null)
            {
                Debug.LogWarning("No se encontró PlayerMovement en el jugador!");
            }
            
            if (interactionPrompt != null && !challengeStarted) 
            {
                interactionPrompt.SetActive(true);
            }
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player salió del trigger de: " + gameObject.name);
            playerInRange = false;
            
            if (!challengeStarted)
            {
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(false);
                }
                
                if (playerMovement != null)
                {
                    playerMovement.enabled = true;
                }
            }
            
            playerMovement = null;
            playerRb = null;
        }
    }
    
    public void ResetChallenge()
    {
        Debug.Log("ResetChallenge llamado en: " + gameObject.name);
        challengeStarted = false;
        
        if (playerInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
    }
    
    public GameObject GetObstacle()
    {
        return obstacleToDestroy;
    }
}