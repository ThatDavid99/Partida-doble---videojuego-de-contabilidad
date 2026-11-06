using UnityEngine;

public class ChestInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;
    
    private ChestAnswer currentAnswer;
    private bool playerInRange = false;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            GameObject instance = Instantiate(interactionPrompt, transform.position + new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            instance.transform.SetParent(transform);
            instance.SetActive(false);
            interactionPrompt = instance;
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    public void SetAnswer(ChestAnswer answer)
    {
        currentAnswer = answer;
    }

    public string GetAnswerText()
    {
        return currentAnswer != null ? currentAnswer.answerText : "Sin respuesta";
    }

    public bool IsStatementTrue()
    {
        return currentAnswer != null && currentAnswer.isCorrect;
    }
    
    public bool IsWinningAnswer()
    {
        return currentAnswer != null && currentAnswer.isWinningAnswer;
    }

    private void OpenChest()
    {
        if (ChallengeManager.Instance != null)
        {
            ChallengeManager.Instance.OpenChest(this);
        }
        else
        {
            Debug.LogError("ChallengeManager.Instance es null en ChestInteractable!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null)
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
        }
    }
}