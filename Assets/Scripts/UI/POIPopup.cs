using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIPopup : Popup
{
    public POI myPOI;

    public void StartPOI()
    {
        myPOI.StartDialogue();
    }
}
