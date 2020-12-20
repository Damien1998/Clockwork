using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

//Refactoring is done! You may enter safely
public class GameManager : MonoBehaviour
{
    //List of ALL avaliable items
    //God help us
    //public Activator[] items;

    //The only game manager allowed to stay in this world of flesh
    public static GameManager instance;

    public Material Selected,NonSelected;
    //All state sprites
    public Sprite repairedImage,brokenImage,unfixableImage,unknownImage,complexBrokenImage;

    //Points and timers
    private int points;

    //End of level screen
    public GameObject endDisplay;
    public List<Recipe> RecipesList = new List<Recipe>();
    //The timer of doom
    public float levelTimerBase;
    private float levelTimer;

    public int levelID;
    public int pointsToComplete;

    public bool levelCompletionCalled;

    public bool sideQuestActive;

    //Save stuff
    //Al the level and POI names will have to be set up manually
    //At least until I find a better way to do it
    //*groan*
    //At the beginning of a game everything will be set to false, except for unlocking the tutorial level
    public List<SaveData.Level> levels = new List<SaveData.Level>();
    public List<SaveData.SideQuest> sideQuests = new List<SaveData.SideQuest>();
    public List<SaveData.Flag> pointsOfInterest = new List<SaveData.Flag>();
    public List<SaveData.Flag> trophies = new List<SaveData.Flag>();

    //A data type for holding workbench recipies


    //ALL recipies for the "basic" workbench
    //They work both ways, according to the workbench's functionality
    public Recipe[] basicRecipes;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        Time.timeScale = 1f;
        //Keeping the population of game managers in check
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        levelTimer = 0;
        LoadGame();
        if(!File.Exists(Application.persistentDataPath + "/savefile.clk"))
        {
            LoadGameData();
        }
        for(int i = 0; i < levels.Count; i++)
        {
            Debug.Log(levels[i].name + ": " + levels[i].completed + ", " + levels[i].completionTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        levelTimer += Time.deltaTime;
        //float temp = levelTimer / levelTimerBase;
        //timerDisplay.value = temp;
        if(points >= pointsToComplete && !levelCompletionCalled)
        {
            endDisplay.gameObject.SetActive(true);
            UIManager.instance.pointDisplayEnd.text = "Czas: " + levelTimer;
            CompleteLevel();
            SaveGame();
            levelCompletionCalled = true;
            Time.timeScale = 0;
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("level " + levelID + " complete");
        if(levelTimer < levels[levelID].completionTime || levels[levelID].completionTime == 0)
        {
            levels[levelID] = new SaveData.Level(levels[levelID].name, true, true, levelTimer, levels[levelID].completionTimeSideQuest);
        }
        
        if(levelID < levels.Count)
        {
            if(!levels[levelID + 1].unlocked)
            {
                levels[levelID + 1] = new SaveData.Level(levels[levelID + 1].name, false, true, 0f, 0f);
            }
        }
        Debug.Log(levels[levelID].completed);
    }

    public void AddPoints(int pointAmount)
    {
        points += pointAmount;
        UIManager.instance.pointDisplay.text = "Punkty: " + points;
    }
    
    private void LoadGameData()
    {
        levels.Add(new SaveData.Level("Tutorial", false, true, 0f, 0f));
        for(int i = 0; i < 12; i++)
        {
            levels.Add(new SaveData.Level("Level " + (i + 1)));
        }
    }

    public void SaveGame()
    {
        //Creates a new SaveData containing the current state of everything
        SaveData saveData = CreateSaveState();

        //Shoves it into a file
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/savefile.clk");
        formatter.Serialize(file, saveData);
        file.Close();
    }

    public void LoadGame()
    {
        if(File.Exists(Application.persistentDataPath + "/savefile.clk"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savefile.clk", FileMode.Open);
            SaveData saveData = (SaveData)formatter.Deserialize(file);
            file.Close();

            levels = saveData.levels;
            sideQuests = saveData.sideQuests;
            trophies = saveData.trophies;
            pointsOfInterest = saveData.pointsOfInterest;
        }
        else
        {
            Debug.Log("No save file!");
        }
    }

    //Saves everything needed from the game manager into a SaveData
    private SaveData CreateSaveState()
    {
        SaveData saveData = new SaveData();

        for(int i = 0; i < levels.Count; i++)
        {
            saveData.levels.Add(levels[i]);
            Debug.Log(levels[i].name + ": " + levels[i].completed + ", " + levels[i].completionTime);
        }

        for (int i = 0; i < sideQuests.Count; i++)
        {
            saveData.sideQuests.Add(sideQuests[i]);
        }

        for (int i = 0; i < trophies.Count; i++)
        {
            saveData.trophies.Add(trophies[i]);
        }

        for (int i = 0; i < pointsOfInterest.Count; i++)
        {
            saveData.pointsOfInterest.Add(pointsOfInterest[i]);
        }

        return saveData;
    }

    public void CompletePOI(string pOIName)
    {
        for(int i = 0; i < pointsOfInterest.Count; i++)
        {
            if(pointsOfInterest[i].name == pOIName)
            {
                pointsOfInterest[i] = new SaveData.Flag(pOIName, true);
            }
        }
    }

    public void StartQuest(string questName,Item QuestItem)
    {
        if (sideQuestActive == false)
        {
            sideQuests.Add(new SaveData.SideQuest(questName, false, true,QuestItem));  
            sideQuestActive = true;
        }
    }

    public void CompleteQuest(string questName)
    {
        for (int i = 0; i < sideQuests.Count; i++)
        {
            if (sideQuests[i].name == questName)
            {
                sideQuests[i] = new SaveData.SideQuest(questName, true, true,null);
            }
        }
    }
}

