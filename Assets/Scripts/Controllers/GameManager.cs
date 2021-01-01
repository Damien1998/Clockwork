using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Audio;


//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    //God help us
    //The only game manager allowed to stay in this world of flesh
    public static GameManager instance;
    public ItemStateDisplay itemStates;
    public List<Recipe> RecipesList = new List<Recipe>();
    public Item questItem;
    [SerializeField]private int _levelID;
    [SerializeField]private bool _sideQuest;
    private SaveController saveController;
    private int points;
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
    public int levelID 
    { 
        get => _levelID;
        set
        {
            _levelID = value;
            Debug.Log("VAR");
            currentLevelParams = (LevelParams)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelParams/Level " + _levelID + ".asset", typeof(LevelParams));
        } 
    }
    public bool sideQuestActive
    {
        get => _sideQuest;
        private set
        {
            _sideQuest = value;
            questItem = value == false ? null : saveController.GetCurrentSideQuest().itemToMake;
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
        saveController = new SaveController();
        saveController.InitializeSaveController(12,0,0,0);
    }

    public void SetSaveController(SaveController inSaveController)
    {
        inSaveController = saveController;
    }
    public void CompleteLevel()
    {
        // if(levelTimer < levels[levelID].completionTime || levels[levelID].completionTime == 0)
        // {
        //     levels[levelID] = new SaveData.Level(levels[levelID].name, true, true, levelTimer, levels[levelID].completionTimeSideQuest);
        // }
        saveController.UnlockLevel(levelID);
    }
    public void StartQuest(string questName,Item questItem)
    {
        if (sideQuestActive == false)
        {
            saveController.AddPOI(questName,questItem);
            sideQuestActive = true;
        }
    }
    public void CompleteQuest(string questName)
    {
        saveController.CompletePOI(questName);
    }
    public void Save(int saveID)
    {
        saveController.CreateSaveGame(saveID);
    }
    // Update is called once per frame
    // void Update()
    // {
    //     levelTimer += Time.deltaTime;
    //     float temp = levelTimer / levelTimerBase;
    //     timerDisplay.value = temp;
    //     if(points >= pointsToComplete && !levelCompletionCalled)
    //     {
    //         endDisplay.gameObject.SetActive(true);
    //         UIManager.instance.pointDisplayEnd.text = "Czas: " + levelTimer;
    //         CompleteLevel();
    //         SaveGame();
    //         levelCompletionCalled = true;
    //         Time.timeScale = 0;
    //     }
    // }
    
    // public void AddPoints(int pointAmount)
    // {
    //     points += pointAmount;
    //     UIManager.instance.pointDisplay.text = "Punkty: " + points;
    // }
    [System.Serializable]
    public class SoundAudioClip
    {
        public SoundManager.Sound sound;
        public AudioClip audioClip;
    }
}


