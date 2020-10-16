using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSelect : MonoBehaviour
{
    private int timesClicked = 0;
    //<summary>
    //Check method for a single object that needs a GameObject that will pop out after clicking the button twice
    //<summary>
    public void OnSelect(GameObject BuildingMenu)
    {
        if (timesClicked < 2)
        {
            BuildingMenu.SetActive(true);
            timesClicked = 0;
        }
        timesClicked++;
    }
}
