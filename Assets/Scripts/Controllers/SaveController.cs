using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveController
{ 
    private static string saveName;
    public static bool _sideQuest,_initialized = false;
    public static SaveData currentSave;
    
    private static List<SaveData.Level> levels;
    public static List<SaveData.SideQuest> sideQuests;
    private static List<SaveData.Flag> pointsOfInterest;
    public static List<SaveData.Flag> trophies;
    
   public static int CompletedLevels()
   {
       return levels.Count;
   }

   public static string currentSaveName()
   {
       return saveName;
   }

   public static Item GetQuestItem(int levelId)
   {
       return sideQuests[levelId-1].questItem;
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
    
    public static void InitializeSaveController(int numberOfLevels,int numberOfSideQuests,int numberOfPOI,int numberOfTrophies)
    {
        levels = new List<SaveData.Level>(new SaveData.Level[numberOfLevels]);
       sideQuests = new List<SaveData.SideQuest>(new SaveData.SideQuest[numberOfSideQuests]);
       pointsOfInterest = new List<SaveData.Flag>(new SaveData.Flag[numberOfPOI]);
       trophies = new List<SaveData.Flag>(new SaveData.Flag[numberOfTrophies]);
       for (int i = 1; i < numberOfSideQuests+1; i++)
       {
           var questItem = Resources.Load<LevelParams>($"LevelParams/Level {i}").questItem;
               if (questItem != null)
               {
                   var sideQuest = sideQuests[i-1];
                   sideQuest.questItem = questItem;
                   sideQuests[i-1] = sideQuest;
               }
       }

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
            sideQuests = saveData.sideQuests;
            trophies = saveData.trophies;
            pointsOfInterest = saveData.pointsOfInterest;
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
        saveData.sideQuests = sideQuests;
        saveData.trophies = trophies;
        saveData.pointsOfInterest = pointsOfInterest;

        return saveData;
    }
    public static SaveData.SideQuest GetSideQuest(int sideQuestID)
    {
        return sideQuests[sideQuestID];
    }
    public static void AddPOI(string questName,string questItemName)
    {
        sideQuests.Add(new SaveData.SideQuest(questName, questItemName, false,true,null)); 
    }

    public static void UnlockLevel(int levelID)
    {
        if(levelID < levels.Count)
        {
            if(!levels[levelID + 1].unlocked)
            { 
                levels[levelID + 1] = new SaveData.Level(levels[levelID + 1].name, false, true, 0f, 0f);
            } 
        }
    }
    public static void CompletePOI(string pOIName)
    {
        for(int i = 0; i < pointsOfInterest.Count; i++)
        {
            if(pointsOfInterest[i].name == pOIName)
            {
                pointsOfInterest[i] = new SaveData.Flag(pOIName, true);
            }
        }
    }
    //To do: Change To Int based Method where you check for current stage instead of quest name
    public static void CompleteQuest(string questName)
    {
        for (int i = 0; i < sideQuests.Count; i++)
        {
            if (sideQuests[i].name == questName)
            {
                sideQuests[i] = new SaveData.SideQuest(questName, sideQuests[i].questItemName, true,true,null);
            }
        }
    }
}
