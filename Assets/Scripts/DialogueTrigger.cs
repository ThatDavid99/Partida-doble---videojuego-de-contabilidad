using System.Collections.Generic;
using UnityEngine;
 
[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}
 
[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}
 
[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}
 
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public GameObject dialoguePanel;

    [Header("Interaction")]
    [SerializeField] private GameObject interactionPrompt;
    private bool playerInRange = false;

    private void Start()
    {
        if (interactionPrompt != null)
        {
            GameObject instance = Instantiate(interactionPrompt, transform.position + new Vector3(-0.1f, 1.2f, 1f), Quaternion.identity);
            instance.SetActive(false);
            interactionPrompt = instance;
        }
    }
    
    public void TriggerDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        if (DialogueManager.Instance != null) DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TriggerDialogue();
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            if (interactionPrompt != null) interactionPrompt.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            if (DialogueManager.Instance != null) DialogueManager.Instance.EndDialogue();
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
            if (interactionPrompt != null) interactionPrompt.SetActive(false);
        }
    }
}