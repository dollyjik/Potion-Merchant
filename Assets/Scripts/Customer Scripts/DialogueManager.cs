using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguePanel;
    
    public bool isDialogueOpen;
    
    private Queue<string> sentences = new Queue<string>();

    private void Awake()
    {
        Instance = this;
    }
    
    public void StartDialogue(string[] lines)
    {
        isDialogueOpen = true;
        dialoguePanel.SetActive(true);
        sentences.Clear();
        foreach (var line in lines)
        {
            sentences.Enqueue(line);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            dialoguePanel.SetActive(false);
            isDialogueOpen = false;
            return;
        }
        
        string sentence = sentences.Dequeue();
        dialogueText.text = sentence;
    }
}
