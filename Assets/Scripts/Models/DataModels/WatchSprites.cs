using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WatchType
{
    HandWatch,
    PocketWatch
}
[CreateAssetMenu(fileName = "New WatchSprites",menuName = "WatchSprites")]
public class WatchSprites : ScriptableObject
{
    public WatchType myType;
    public Sprite[] Belt,Box,Decoration,Glass,Mechanical,Housing,Face;
}
