using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class WatchComponent : MonoBehaviour
{
    public bool[] componentExists;
    public int numberOfComponents;
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < componentExists.Length && componentExists[i]) numberOfComponents++;
        }
    }
}

