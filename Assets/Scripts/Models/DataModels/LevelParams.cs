using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Level",menuName = "Level")]
public class LevelParams : ScriptableObject
{
    public int time;

    //Old way of defining level watches
    //public List<Item> listOfWatches = new List<Item>();
    //public List<Recipe> listOfRecipes = new List<Recipe>();

    public Item questItem;

    //New way of defining level watches
    public int watchAmount;
    public int pocketWatchWeight, wristWatchWeight;
    public bool timedLevel;
    public int decorPercentChance;
    public bool brokenState, unfixableState;
    public int brokenPartMinPercentage, brokenPartMaxPercentage;
    public int unknownStatePercentChance;
    public bool tierTwoUnknownState, researchAllowed;
    public bool casingComponents, mechanismComponents, eitherOfMechOrCasing;
    public int mechMinParts, mechMaxParts;
    public float watchDispensingTime;
    public Trophy levelTrophy;

}