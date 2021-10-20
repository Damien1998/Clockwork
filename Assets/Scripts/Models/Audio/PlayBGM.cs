﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayBGM : MonoBehaviour
{
    public AudioClip _BGM;

    private void Start()
    {
        GameManager.instance.SoundController.ChangeBGM(_BGM);
    }
}
