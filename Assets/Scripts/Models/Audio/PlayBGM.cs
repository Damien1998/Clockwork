using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    public AudioClip _BGM;

    private void Awake()
    {
        GameManager.instance.BGM.clip = _BGM;
        GameManager.instance.BGM.Play();
    }
}
