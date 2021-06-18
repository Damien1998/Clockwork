
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serialization is lkie black magic and I have absolutely no idea what I'm doing. Lol
[System.Serializable]
public class SaveData
{
    
    public string saveName = "No Save";

    public int saveID = 0;
    //A struct for saving everything the player interacts with within levels - trophies, POI, etc.
    /// <summary>
    /// It contains info on the name of the interaction (so that our little human brains don't get fried up when we have to figure out why does the POI nr 7556347865 not work) and its completion status.
    /// Basically, you set this as completed if the player did something.
    /// Interacting with the data type is clunky af, Unity doesn't let us edit arrays of this in the editor.
    /// Ghhh
    /// </summary>
    
    //Save stuff
    //Al the level and POI names will have to be set up manually
    //At least until I find a better way to do it
    //*groan*
    //At the beginning of a game everything will be set to false, except for unlocking the tutorial level
    
    //We need to know if the player even found all the sidequests, so yay, another data type.
    [System.Serializable]
    public struct SideQuest
    {
        public string name;
        public bool completed;
        public int TrophyID;


        //Use for adding side quests
        public SideQuest(string questName)
        {
            name = questName;
            completed = false;
            TrophyID = 0;
        }

        //Use for modifying data
        public SideQuest(string questName,string itemName, bool questCompleted,int _trophyID)
        {
            name = questName;
            completed = questCompleted;
            TrophyID = _trophyID;
        }
    }

    //The same as Flag, but for the levels themselves. 
    //Extra data - is the level unlocked and time highscores.
    //Dunno why, but it won't work without this
    [System.Serializable]
    public struct Level
    {
        public int levelID;
        public string name;
        public bool completed;
        public SideQuest levelSideQuest;
        public float completionTime;
        public float completionTimeSideQuest;
        
        
        //Use for adding new levels, except the tutorial
        public Level(int _levelID)
        {
            levelID = _levelID;
            name = "levelName";
            completed = false;
            levelSideQuest = new SideQuest();
            completionTime = 0;
            completionTimeSideQuest = 0;
        }

        //Use for unlocking levels or completing them
        public Level(int _levelID,string levelName, bool levelCompleted, float time, float sideQuestTime)
        {
            levelID = _levelID;
            name = levelName;
            completed = levelCompleted;
            completionTime = time;
            completionTimeSideQuest = sideQuestTime;
            levelSideQuest = new SideQuest();
        }
    }
    [System.Serializable]
    public struct TrophyInfo
    {
        public Sprite trophyImage;
        public string trophyName;
        public string Description;
        

        public TrophyInfo(string _description,string _trophyName,Sprite _trophyImage)
        {
            trophyImage = _trophyImage;
            Description = _description;
            trophyName = _trophyName;
        }
        
    }

    //All the stuff to save. Basically, this is all the player progress.
    //THIS IS OF UTMOST IMPORTANCE
    //You can't edit objects stored in such lists directly
    //If you want to, say, mark a level as complete
    //You have to replace the object with a new one
    //See the game manager script for examples
    public List<Level> levels = new List<Level>();
    public List<SideQuest> completedSideQuests = new List<SideQuest>();
}

