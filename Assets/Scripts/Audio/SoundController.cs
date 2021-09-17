using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static List<SoundModel> soundList;
    private void Awake()
    {
        SoundManager.Initialize();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SoundManager.PlaySound(SoundManager.Sound.DefaultClick);
        }
    }

    public void ChangeBGM(AudioClip audioClip)
    {
        GetComponent<AudioSource>().clip = audioClip;
        GetComponent<AudioSource>().Play();
    }

    public void SetSounds(List<SoundModel> soundModels)
    {
        soundList = new List<SoundModel>();
        foreach (var sound in soundModels)
        {
            soundList.Add(sound);
        }
    }


}
