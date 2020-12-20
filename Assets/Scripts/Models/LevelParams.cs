using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Level",menuName = "Level")]
public class LevelParams : ScriptableObject
{
    public int time;
    public List<Item> listOfWatches = new List<Item>();
}
