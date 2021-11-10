using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class POIPopupOpener : PopupOpener
{
    [SerializeField]
    private POI myPOI;
    [SerializeField]
    private POIPopup popup;

    public void SetPOIToStart()
    {
        popup.myPOI = myPOI;
    }

    public override void ShowPopup()
    {
        SetPOIToStart();
        base.ShowPopup();
    }
}
