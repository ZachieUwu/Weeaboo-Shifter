using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;

    public void DisplayDialogue(string text)
    {
        if (dialogueText != null)
        {
            dialogueText.text = text;
        }
    }
}
