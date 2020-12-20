﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serialization is lkie black magic and I have absolutely no idea what I'm doing. Lol
[System.Serializable]
public class SaveData
{
    //A struct for saving everything the player interacts with within levels - trophies, POI, etc.
    /// <summary>
    /// It contains info on the name of the interaction (so that our little human brains don't get fried up when we have to figure out why does the POI nr 7556347865 not work) and its completion status.
    /// Basically, you set this as completed if the player did something.
    /// Interacting with the data type is clunky af, Unity doesn't let us edit arrays of this in the editor.
    /// Ghhh
    /// </summary>

    //TODO: find and extension that will let us edit it all in a humanely possible way.
    [System.Serializable]
    public struct Flag
    {
        public string name;
        public bool completed;

        //Use for creating new flags
        public Flag(string flagName)
        {
            name = flagName;
            completed = false;
        }

        //Use for setting flags as complete
        public Flag(string flagName, bool markComplete)
        {
            name = flagName;
            completed = markComplete;
        }
    }

    //We need to know if the player even found all the sidequests, so yay, another data type.
    [System.Serializable]
    public struct SideQuest
    {
        public string name;
        public bool completed;
        public bool found;
        public Item itemToMake;

        //Use for adding side quests
        public SideQuest(string questName)
        {
            name = questName;
            completed = false;
            found = false;
            itemToMake = null;
        }

        //Use for modifying data
        public SideQuest(string questName, bool questCompleted, bool questFound, Item ItemToMake)
        {
            name = questName;
            completed = questCompleted;
            found = questFound;
            itemToMake = ItemToMake;
        }
    }

    //The same as Flag, but for the levels themselves. 
    //Extra data - is the level unlocked and time highscores.
    //Dunno why, but it won't work without this
    [System.Serializable]
    public struct Level
    {
        public string name;
        public bool completed;
        public bool unlocked;
        public float completionTime;
        public float completionTimeSideQuest;

        //Use for adding new levels, except the tutorial
        public Level(string levelName)
        {
            name = levelName;
            completed = false;
            unlocked = false;
            completionTime = 0;
            completionTimeSideQuest = 0;
        }

        //Use for unlocking levels or completing them
        public Level(string levelName, bool levelCompleted, bool levelUnlocked, float time, float sideQuestTime)
        {
            name = levelName;
            completed = levelCompleted;
            unlocked = levelUnlocked;
            completionTime = time;
            completionTimeSideQuest = sideQuestTime;
        }
    }

    //All the stuff to save. Basically, this is all the player progress.
    //THIS IS OF UTMOST IMPORTANCE
    //You can't edit objects stored in such lists directly
    //If you want to, say, mark a level as complete
    //You have to replace the object with a new one
    //See the game manager script for examples
    public List<Level> levels = new List<Level>();
    public List<SideQuest> sideQuests = new List<SideQuest>();
    public List<Flag> pointsOfInterest = new List<Flag>();
    public List<Flag> trophies = new List<Flag>();
}
