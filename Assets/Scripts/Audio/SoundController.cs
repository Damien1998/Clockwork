using System;
using System.Collections;
using System.Collections.Generic;
using ModestTree;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static List<SoundModel> soundList;

    private AudioSource audioSource => GetComponent<AudioSource>();

    private void Awake()
    {
        Initialize();
        SoundManager.Initialize();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //SoundManager.PlaySound(SoundManager.Sound.DefaultClick);
        }
    }

    public void ChangeBGM(AudioClip audioClip)
    {
        audioSource.Stop();
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void SetSounds(List<SoundModel> soundModels)
    {
        soundList = new List<SoundModel>();
        foreach (var sound in soundModels)
        {
            soundList.Add(sound);
        }
    }

    private void Initialize()
    {
        var sounds = Resources.LoadAll("Sounds", typeof(SoundModel));

        List<SoundModel> soundsList = new List<SoundModel>();

        foreach (var sound in sounds)
        {
            soundsList.Add((SoundModel)sound);
        }

        SetSounds(soundsList);
    }

}
