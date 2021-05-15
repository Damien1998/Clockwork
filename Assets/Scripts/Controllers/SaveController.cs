using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveController
{ 
    private static string saveName;
    public static bool _initialized = false,hasQuest;
    public static SaveData currentSave;
    
    private static List<SaveData.Level> levels;
    public static List<SaveData.SideQuest> completedSideQuests;

    public static int CompletedLevels()
   {
       return levels.Count;
   }

   public static string currentSaveName()
   {
       return saveName;
   }
   public static void ChangeSaveName(string name)
   {
       currentSave.saveName = name;
   }
    public static bool CheckForSaves(int saveID)
    {
        if(File.Exists(Application.persistentDataPath + "/savefile"+saveID+".clk"))
        {
            return true;
        }
        return false;
    }
    
    public static void InitializeSaveController()
    { 
        levels = new List<SaveData.Level>();
        completedSideQuests = new List<SaveData.SideQuest>();

        _initialized = true;
    }
    public static void CreateSaveGame(int saveID)
    {
        //Creates a new SaveData containing the current state of everything
        SaveData saveData = SaveState();
        //Shoves it into a file
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savefile"+saveID+".clk");
            formatter.Serialize(file, saveData);
            file.Close();
    }
    public static void LoadGame(int saveID)
    {
        BinaryFormatter formatter = new BinaryFormatter();
            FileStream file;
            file = File.Open(Application.persistentDataPath + "/savefile"+saveID+".clk", FileMode.Open);
            SaveData saveData = (SaveData)formatter.Deserialize(file);
            file.Close();
            currentSave = saveData;
            saveName = saveData.saveName;
            levels = saveData.levels;
    }

    public static void DeleteSave(int saveID)
    {
        if (CheckForSaves(saveID))
        {
            File.Delete(Application.persistentDataPath + "/savefile"+saveID+".clk");
        }
    }
    //Saves everything needed from the game manager into a SaveData
    private static SaveData SaveState()
    {
        SaveData saveData = new SaveData();
        
        saveData.levels = levels;

        return saveData;
    }
    public static SaveData.Level? ReturnLevel(int _levelID)
    {
        var levelListIndex = GetLevelListIndex(_levelID);
        
        return levels[levelListIndex];
    }
    public static void UnlockLevel(int levelID)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].levelID == levelID)
            {
                break;
            }
        }
        var tmpLevel = new SaveData.Level();
        tmpLevel.levelSideQuest.questTrophy = Resources.Load<LevelParams>($"LevelParams/Level {levelID}").levelTrophy;
        tmpLevel.levelID = levelID;
        levels.Add(tmpLevel);
        
    }
    public static void CompleteQuest(int levelID)
    {
        if (ReturnLevel(levelID).HasValue)
        {
            var levelListIndex = GetLevelListIndex(levelID);
            
            var level = levels[levelListIndex];
            level.levelSideQuest.completed = true;
            levels[levelListIndex] = level;
            
            completedSideQuests.Add(ReturnLevel(levelID).Value.levelSideQuest);
        }
    }

    static int GetLevelListIndex (int levelID)
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i].levelID == levelID)
            {
                return i;
            }
        }
        return 0;
    }
}
