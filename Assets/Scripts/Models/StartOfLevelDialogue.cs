using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartOfLevelDialogue : MonoBehaviour
{
    public string poiName;
    public TextAsset textScript;

    private void Start()
    {
        StartDialogue();
    }

    public void StartDialogue()
    {
        DialogueManager.instance.StartDialogue(textScript);
    }
}
