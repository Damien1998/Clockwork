using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        
        DefaultClick = 8,       //Jeśli gracz kliknie, ale w nic konkretnego
        SliderInteraction = 9,
        Typing = 10,


        //W warsztacie
            //> Koroki
        StepAnna1 = 11,
        StepAnna2 = 12,
        StepAnna3 = 13,
        StepAnna4 = 14,
        StepAnna5 = 15,
        StepEdward1 = 16,
        StepEdward2 = 17,
        StepEdward3 = 18,
        StepEdward4 = 19,
        StepEdward5 = 20,

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


        //Dialogi
        NextDialog = 61,         // przy odpaleniu kolejnej lini dialogowej

    }

    private static Dictionary<Sound, float> soundTimerDictionary;
    private static GameObject oneShotGameObject;
    private static AudioSource oneShotAudioSource;

    public static void Initialize()
    {
        soundTimerDictionary = new Dictionary<Sound, float>();
        soundTimerDictionary[Sound.NONE] = 0f;
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
            default:
                return true;
            //case Sound.PlayerMove:
            //    if (soundTimerDictionary.ContainsKey(sound))
            //    {
            //        float lastTimePlayed = soundTimerDictionary[sound];
            //        float playerMoveTimerMax = .4f;
            //        if (lastTimePlayed + playerMoveTimerMax < Time.time)
            //        {
            //            soundTimerDictionary[sound] = Time.time;
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //case Sound.Jump:
            //    if (soundTimerDictionary.ContainsKey(sound))
            //    {
            //        float lastTimePlayed = soundTimerDictionary[sound];
            //        float playerMoveTimerMax = .5f;
            //        if (lastTimePlayed + playerMoveTimerMax < Time.time)
            //        {
            //            soundTimerDictionary[sound] = Time.time;
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return true;
            //    }
        }
    }
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameManager.SoundAudioClip soundAudioClip in GameManager.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip[Random.Range(0,soundAudioClip.audioClip.Length)];
            }
        }
        return null;
    }
}
