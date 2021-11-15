using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

public class StartOfLevelDialogue : MonoBehaviour
{
    public string textName;
    public TextAsset textScript;

    private void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        if (!textName.IsEmpty())
        {
            DialogueManager.instance.StartDialogue(textName);
        }
        else
        {
            DialogueManager.instance.StartDialogue(textScript);
        }
    }
}
