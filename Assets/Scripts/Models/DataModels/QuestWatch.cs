﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Watch",menuName = "Quest Watch")]
public class QuestWatch : ScriptableObject
{
    private bool LastQuestWatch = false;
    public ItemState myState;
    public Sprite[] QuestWatchSprites;
    public QuestWatch[] Parts;
    
}
