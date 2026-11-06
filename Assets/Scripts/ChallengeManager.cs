using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChallengeManager : MonoBehaviour
{
    public static ChallengeManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject acceptChallengePanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject answerPanel;
    [SerializeField] private GameObject resultPanel;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI challengeQuestionText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private TextMeshProUGUI resultText;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;
    [SerializeField] private Button trueButton;
    [SerializeField] private Button falseButton;
    [SerializeField] private Button resultOkButton;

    [Header("Chests")]
    [SerializeField] private ChestInteractable[] chests;

    private ChallengeData currentChallenge;
    private ChallengeTrigger currentTrigger;
    private float currentTime;
    private bool challengeActive = false;
    private bool challengeCompleted = false;
    private ChestInteractable currentOpenChest;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("✓ ChallengeManager Instance creado correctamente");
        }
        else
        {
            Debug.LogWarning("Ya existe un ChallengeManager, destruyendo duplicado");
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(AcceptChallenge);
        }
        
        if (rejectButton != null)
        {
            rejectButton.onClick.RemoveAllListeners();
            rejectButton.onClick.AddListener(RejectChallenge);
        }
        
        if (trueButton != null)
        {
            trueButton.onClick.RemoveAllListeners();
            trueButton.onClick.AddListener(() => CheckAnswer(true));
        }
        
        if (falseButton != null)
        {
            falseButton.onClick.RemoveAllListeners();
            falseButton.onClick.AddListener(() => CheckAnswer(false));
        }
        
        if (resultOkButton != null)
        {
            resultOkButton.onClick.RemoveAllListeners();
            resultOkButton.onClick.AddListener(CloseResultPanel);
        }

        DeactivateAllUI();
        
        Debug.Log("ChallengeManager inicializado correctamente");
    }

    private void Update()
    {
        if (challengeActive && !challengeCompleted)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0)
            {
                TimeUp();
            }
        }
    }

    public void ShowChallengePrompt(ChallengeData challenge, ChallengeTrigger trigger)
    {
        if (challenge == null)
        {
            Debug.LogError("ChallengeData es null!");
            return;
        }

        Debug.Log("Mostrando panel de reto: " + challenge.challengeQuestion);
        currentChallenge = challenge;
        currentTrigger = trigger;
        
        if (acceptChallengePanel != null)
        {
            acceptChallengePanel.SetActive(true);
        }
        
        if (challengeQuestionText != null)
        {
            challengeQuestionText.text = challenge.challengeQuestion;
        }
    }

    public void AcceptChallenge()
    {
        Debug.Log("Reto aceptado");
        
        if (acceptChallengePanel != null)
        {
            acceptChallengePanel.SetActive(false);
        }

        if (currentChallenge == null)
        {
            Debug.LogError("currentChallenge es null!");
            return;
        }

        challengeActive = true;
        challengeCompleted = false;
        currentTime = currentChallenge.timeLimit;

        ActivateChests();

        if (timerPanel != null)
        {
            timerPanel.SetActive(true);
        }
        
        if (PlayerMovement.instance != null)
        {
            PlayerMovement.instance.enabled = true;
            Debug.Log("Movimiento del jugador reactivado");
        }
    }

    public void RejectChallenge()
    {
        Debug.Log("Reto rechazado");
        
        if (acceptChallengePanel != null)
        {
            acceptChallengePanel.SetActive(false);
        }
        
        currentChallenge = null;
        currentTrigger = null;

        if (PlayerMovement.instance != null)
        {
            PlayerMovement.instance.enabled = true;
        }
        
        ChallengeTrigger trigger = FindFirstObjectByType<ChallengeTrigger>();
        if (trigger != null)
        {
            trigger.ResetChallenge();
        }
    }

    private void ActivateChests()
    {
        if (chests == null || chests.Length == 0)
        {
            Debug.LogError("No hay cofres asignados!");
            return;
        }

        if (currentChallenge.answers == null || currentChallenge.answers.Length == 0)
        {
            Debug.LogError("No hay respuestas en el ChallengeData!");
            return;
        }

        List<int> answerIndices = new List<int>();
        for (int i = 0; i < currentChallenge.answers.Length; i++)
        {
            answerIndices.Add(i);
        }

        for (int i = answerIndices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            int temp = answerIndices[i];
            answerIndices[i] = answerIndices[randomIndex];
            answerIndices[randomIndex] = temp;
        }

        int chestsToActivate = Mathf.Min(chests.Length, answerIndices.Count);
        for (int i = 0; i < chestsToActivate; i++)
        {
            int answerIndex = answerIndices[i];
            ChestAnswer answer = currentChallenge.answers[answerIndex];
            chests[i].SetAnswer(answer);
            chests[i].gameObject.SetActive(true);
            
            string winningText = answer.isWinningAnswer ? " [GANADOR]" : "";
            Debug.Log("Cofre " + i + " activado: \"" + answer.answerText + "\" (Es " + (answer.isCorrect ? "VERDADERO" : "FALSO") + ")" + winningText);
        }
    }

    public void OpenChest(ChestInteractable chest)
    {
        if (!challengeActive || challengeCompleted)
        {
            Debug.LogWarning("No se puede abrir cofre: reto no activo o ya completado");
            return;
        }

        Debug.Log("Cofre abierto");
        currentOpenChest = chest;
        
        if (answerPanel != null)
        {
            answerPanel.SetActive(true);
        }
        
        if (answerText != null)
        {
            answerText.text = chest.GetAnswerText();
        }
    }

    private void CheckAnswer(bool playerSelectedTrue)
    {
        Debug.Log("Player seleccionó: " + (playerSelectedTrue ? "VERDADERO" : "FALSO"));
        
        if (answerPanel != null)
        {
            answerPanel.SetActive(false);
        }

        if (currentOpenChest == null)
        {
            Debug.LogError("currentOpenChest es null!");
            return;
        }

        bool statementIsTrue = currentOpenChest.IsStatementTrue();
        bool answeredCorrectly = (statementIsTrue == playerSelectedTrue);
        bool isWinningChest = currentOpenChest.IsWinningAnswer();
        
        Debug.Log("Afirmación es " + (statementIsTrue ? "VERDADERA" : "FALSA"));
        Debug.Log("Player respondió " + (answeredCorrectly ? "CORRECTAMENTE" : "INCORRECTAMENTE"));
        Debug.Log("¿Es el cofre ganador? " + isWinningChest);

        if (isWinningChest)
        {
            if (answeredCorrectly)
            {
                Debug.Log("¡VICTORIA! Cofre ganador + respuesta correcta");
                ChallengeSuccess();
            }
            else
            {
                Debug.Log("Era el cofre ganador pero respondió mal - temblor");
                StartCoroutine(ShowIncorrectFeedback());
            }
        }
        else
        {
            if (answeredCorrectly)
            {
                Debug.Log("Respuesta correcta, pero no es el cofre ganador");
                StartCoroutine(ShowCorrectButNotWinningFeedback());
            }
            else
            {
                Debug.Log("Respuesta incorrecta - temblor");
                StartCoroutine(ShowIncorrectFeedback());
            }
        }

        currentOpenChest = null;
    }

    private void ChallengeSuccess()
    {
        challengeActive = false;
        challengeCompleted = true;

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (resultText != null)
        {
            resultText.text = "¡Felicidades!";
        }
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }

        DeactivateAllChests();
        
        if (currentTrigger != null)
        {
            GameObject obstacle = currentTrigger.GetObstacle();
            if (obstacle != null)
            {
                Debug.Log("Destruyendo obstáculo: " + obstacle.name);
                Destroy(obstacle);
            }
            else
            {
                Debug.LogWarning("No hay obstáculo asignado en el ChallengeTrigger");
            }
        }
    }

    private IEnumerator ShowCorrectButNotWinningFeedback()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (resultText != null)
        {
            resultText.text = "Correcto, pero sigue buscando...";
        }

        yield return new WaitForSeconds(1.5f);

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private IEnumerator ShowIncorrectFeedback()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (resultText != null)
        {
            resultText.text = "Respuesta Incorrecta";
        }

        if (Camera.main != null)
        {
            StartCoroutine(CameraShake(0.3f, 0.2f));
        }

        yield return new WaitForSeconds(1.5f);

        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    private void TimeUp()
    {
        Debug.Log("Tiempo agotado");
        challengeActive = false;
        challengeCompleted = true;

        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }
        
        if (resultText != null)
        {
            resultText.text = "Tiempo Agotado";
        }
        
        if (timerPanel != null)
        {
            timerPanel.SetActive(false);
        }

        DeactivateAllChests();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;
        
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void DeactivateAllChests()
    {
        if (chests == null) return;
        
        foreach (var chest in chests)
        {
            if (chest != null)
            {
                chest.gameObject.SetActive(false);
            }
        }
    }

    private void CloseResultPanel()
    {
        Debug.Log("Cerrando panel de resultado");
        
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        if (challengeCompleted)
        {
            ResetChallenge();
        }
    }

    private void ResetChallenge()
    {
        Debug.Log("Reseteando reto");
        challengeActive = false;
        challengeCompleted = false;
        currentChallenge = null;
        currentTrigger = null;

        if (PlayerMovement.instance != null)
        {
            PlayerMovement.instance.enabled = true;
        }
        
        ChallengeTrigger trigger = FindFirstObjectByType<ChallengeTrigger>();
        if (trigger != null)
        {
            trigger.ResetChallenge();
        }
    }

    private void DeactivateAllUI()
    {
        if (acceptChallengePanel != null)
            acceptChallengePanel.SetActive(false);
        
        if (timerPanel != null)
            timerPanel.SetActive(false);
        
        if (answerPanel != null)
            answerPanel.SetActive(false);
        
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }
}