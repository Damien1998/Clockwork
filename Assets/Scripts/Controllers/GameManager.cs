using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

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
    public SoundController SoundController;

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
        if (!SaveController._initialized)
        {
            SaveController.InitializeSaveController();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F4))
        {

            int c = SceneManager.sceneCount;
            for (int i = 0; i < c; i++) {
                Scene scene = SceneManager.GetSceneAt (i);
                SceneManager.UnloadSceneAsync (scene);
            }

            DialogueManager.instance.ExitDialogue();

            RecipeListView.ResetRecipesList();

            SceneManager.LoadScene("Level10-City");
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SoundManager.PlaySound(SoundManager.Sound.UIChange);
    }

    public void StartCityLevel(int mylevelID)
    {
        AnalyticsController.SendAnalyticDictionary("LevelStart","Level", mylevelID);
        SaveController.UnlockLevel(mylevelID);
        SceneManager.LoadSceneAsync($"Level{mylevelID}-City");
        StartCoroutine(WaitAndStartCityLevel(mylevelID));
    }

    IEnumerator WaitAndStartCityLevel(int mylevelID)
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeIn");
        yield return new WaitForSeconds(0.3f);
        DialogueManager.instance.StartDialogue($"lvl{mylevelID}_start");
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


