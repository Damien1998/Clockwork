using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockLevel : MonoBehaviour
{
    public int thisLevelIndex = 0;

    private void Start()
    {
        SaveController.UnlockLevel(thisLevelIndex);
    }
}
