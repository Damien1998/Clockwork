using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
    public string poiName;
    public TextAsset textScript;

    private void Start()
    {
  //      DialogueManager.instance.StartDialogue("Test");
    }

    public void StartDialogue()
    {
        Debug.Log("Attempting to start dialogue");
        GameManager.instance.AddPoiEncounter(poiName);
        DialogueManager.instance.StartDialogue(textScript.name);
    }
}
