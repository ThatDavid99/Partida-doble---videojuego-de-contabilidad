using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
 
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
 
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;
    public TextMeshProUGUI buttonText; // Añade esta referencia al texto del botón
    private GameObject dialoguePanel;
 
    private Queue<DialogueLine> lines;
    
    public bool isDialogueActive = false;
 
    public float typingSpeed = 0.2f;
 
    public Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
        dialoguePanel = this.gameObject;
    }
 
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
 
        lines.Clear();
 
        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }
 
        DisplayNextDialogueLine();
    }
 
    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }
    
        DialogueLine currentLine = lines.Dequeue();
    
        characterIcon.sprite = currentLine.character.icon;
        characterName.text = currentLine.character.name;
    
        StopAllCoroutines();
    
        StartCoroutine(TypeSentence(currentLine));

        if (lines.Count == 0)
        {

            buttonText.text = "Fin";
        }
        else
        {
            buttonText.text = "Continuar";
        }
        
        buttonText.fontSize = 15;
    }
 
    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        
    }
 
    public void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false); 
        buttonText.text = "Continuar"; 
    }
}