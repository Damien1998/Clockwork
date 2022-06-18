using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueActor : MonoBehaviour
{
    public Image image;
    public Animator animator;
    public SoundManager.Sound sound;
    public FXManager.ParticleFX emoteFX;
    public string nameColor;

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
