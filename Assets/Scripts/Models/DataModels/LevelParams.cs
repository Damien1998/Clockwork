using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Level",menuName = "Level")]
public class LevelParams : ScriptableObject
{
    public string levelName;
    public int time;

    public QuestWatch questWatch;
    public Trophy levelTrophy;
    public bool timedLevel;

    public WatchSpawningTypes SpawningType;

    //New way of defining level watches
    public int watchAmount;
    public int pocketWatchWeight, wristWatchWeight;
    public int decorPercentChance;
    public bool brokenState, unfixableState;
    public int brokenPartMinPercentage, brokenPartMaxPercentage;
    public int unknownStatePercentChance;
    public bool  researchAllowed;
    public int mechMinParts, mechMaxParts;
    public float watchDispensingTime;

}

public enum WatchSpawningTypes {None, MechanismOrCasing, MechanismAndCasing, Mechanism, Casing}