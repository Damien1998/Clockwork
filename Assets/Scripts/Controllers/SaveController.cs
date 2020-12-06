using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveController
{ 
    private string saveName;
   private List<SaveData.Level> levels;
   private List<SaveData.SideQuest> sideQuests;
   private List<SaveData.Flag> pointsOfInterest;
   private List<SaveData.Flag> trophies;

   public int CompletedLevels()
   {
       return levels.Count;
   }

   public string currentSaveName()
   {
       return saveName;
   }
    public bool CheckForSaves(int saveID)
    {
        if(File.Exists(Application.persistentDataPath + "/savefile"+saveID+".clk"))
        {
            return true;
        }
        return false;
    }
    
    public void InitializeSaveController(int numberOfLevels,int numberOfSideQuests,int numberOfPOI,int numberOfTrophies)
    { 
        levels = new List<SaveData.Level>();
       sideQuests = new List<SaveData.SideQuest>();
       pointsOfInterest = new List<SaveData.Flag>();
       trophies = new List<SaveData.Flag>();
       levels.Add(new SaveData.Level("Tutorial", false, true, 0f, 0f)); 
    }
    public void CreateSaveGame(int saveID)
    {
        //Creates a new SaveData containing the current state of everything
        SaveData saveData = SaveState();
        //Shoves it into a file
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/savefile"+saveID+".clk");
            formatter.Serialize(file, saveData);
            file.Close();
    }
    public void LoadGame(int saveID)
    {
        BinaryFormatter formatter = new BinaryFormatter();
            FileStream file;
            file = File.Open(Application.persistentDataPath + "/savefile"+saveID+".clk", FileMode.Open);
            SaveData saveData = (SaveData)formatter.Deserialize(file);
            file.Close();
            saveName = saveData.saveName;
            levels = saveData.levels;
            sideQuests = saveData.sideQuests;
            trophies = saveData.trophies;
            pointsOfInterest = saveData.pointsOfInterest;
    }

    public void DeleteSave(int saveID)
    {
        if (CheckForSaves(saveID))
        {
            File.Delete(Application.persistentDataPath + "/savefile"+saveID+".clk");
        }
    }
    //Saves everything needed from the game manager into a SaveData
    private SaveData SaveState()
    {
        SaveData saveData = new SaveData();
        
        saveData.levels = levels;
        saveData.sideQuests = sideQuests;
        saveData.trophies = trophies;
        saveData.pointsOfInterest = pointsOfInterest;

        return saveData;
    }
    public SaveData.SideQuest GetCurrentSideQuest()
    {
        return sideQuests[0];
    }
    public void AddPOI(string questName,Item questItem)
    {
        sideQuests.Add(new SaveData.SideQuest(questName, false, true,questItem)); 
    }

    public void UnlockLevel(int levelID)
    {
        if(levelID < levels.Count)
        {
            if(!levels[levelID + 1].unlocked)
            { 
                levels[levelID + 1] = new SaveData.Level(levels[levelID + 1].name, false, true, 0f, 0f);
            } 
        }
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
