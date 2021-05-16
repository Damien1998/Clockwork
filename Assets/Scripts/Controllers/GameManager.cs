using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    //God help us
    //The only game manager allowed to stay in this world of flesh
    public static GameManager instance;
    public ItemStateDisplay itemStates;
    [SerializeField]private int _levelID;

    public bool hasQuest;
    
    private int points;
    

    public List<WatchSprites> watchTypes;
    /*
    //PS
    //<summary>
    //A Variable that Hold Current Level Watch List and Time 
    //</summary>
    //It will only be changed when levelID is changed ,until then it is null
    //Which is changed when a level is selected
    */
    public LevelParams currentLevelParams;
    public SoundAudioClip[] soundAudioClipArray;
    public AudioMixerGroup SFX;
    public AudioSource BGM;

    public int levelID 
    { 
        get => _levelID;
        set
        {
            _levelID = value;

            currentLevelParams = Resources.Load<LevelParams>("LevelParams/Level " + _levelID);
        } 
    }
    private void Awake()
    {
        //Keeping the population of game managers in check
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        SoundManager.Initialize();
        if (!SaveController._initialized)
        {
            SaveController.InitializeSaveController();
        }
    }
    public void CompleteLevel()
    {
        SaveController.UnlockLevel(levelID);
    }
    public void Save(int saveID)
    {
        SaveController.CreateSaveGame(saveID);
    }
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip[] audioClip;
    }
}


