using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public static class SoundManager
{
    public enum Sound
    {
        NONE = 0,

        //UI
        ButtonClick = 1,
        PoiInteraction = 2,     // przy wejściu w interakcję z PoI lub QuestGiverem
        WorkshopInteraction = 3,
        SceneSwapOut = 4,          // odpalane przy zmianie sceny innej niż menu
        SceneSwapIn = 5,
        SceneSwapToMenu = 6,
        //wolne mijsce
        DefaultClick = 8,       //Jeśli gracz kliknie, ale w nic konkretnego
        SliderInteraction = 9,
        Typing = 10,


        //W warsztacie
            //> Koroki
        StepAnna= 11,
        StepEdward = 12,

            //> Workbenche
        WorkSimple = 31,
        WorkAdvanced = 32,
        WorkPostal = 33,
        WorkResearch = 34,
        WorkItemEject = 35,     //Wypluwanie itemu z workbencha

            //> Itemy
        ItemPickUp = 41,
        ItemDrop = 42,          //Item upuszcony
        ItemPlaced = 43,        //Item położony na stole
        ClockCompleted = 44,    //Przy oddaniu naprawionego zegarka

            //> Other
        Dash = 51,
        WinPopup = 52,
        LosePopup = 53,
        AlarmRing,


        //Dialogi
        NextDialog = 61,         // przy odpaleniu kolejnej lini dialogowej


        //SFX
        SFXSnow,
        SFXWood,
        SFXChristmasTree,
        SFXBrick,
        SFXPavement,
        SFXMetal,
        SFXBuilding,
        SFXWater,
        SFXGrass


    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;



    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.NONE] = 0f;
        soundTimerDictionary[Sound.StepAnna] = 0f;
        soundTimerDictionary[Sound.Dash] = 0f;
    }
    public static void PlaySound(Sound sound)
    {
        if (CanPlaySound(sound))
        {
            if (oneShotGameObject == null)
            {
                oneShotGameObject = new GameObject("One ShotSound");
                oneShotAudioSource = oneShotGameObject.AddComponent<AudioSource>();
                oneShotAudioSource.outputAudioMixerGroup = GameManager.instance.SFX;
            }
            oneShotAudioSource.PlayOneShot(GetAudioClip(sound));
        }
    }

    private static bool CanPlaySound(Sound sound)
    {
        switch (sound)
        {
            case Sound.StepAnna:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .4f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case Sound.Dash:
                if (soundTimerDictionary.ContainsKey(sound))
                {
                    float lastTimePlayed = soundTimerDictionary[sound];
                    float playerMoveTimerMax = .5f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        soundTimerDictionary[sound] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
        }
    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (SoundModel soundAudioClip in SoundController.soundList)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip[Random.Range(0, soundAudioClip.audioClip.Length)];
            }
        }
        return null;
    }
}
