using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListButton : MonoBehaviour
{
    public Watch examinedWatch;

    //Is there a way to handle all this without replacing sprites every time?
    public void ToggleList()
    {
        if(!UIManager.instance.componentList.activeInHierarchy || UIManager.instance.listItem != examinedWatch)
        {
            Debug.Log("Activating list");
            UIManager.instance.componentList.SetActive(true);
            UIManager.instance.listItem = examinedWatch;
            UIManager.instance.watchDisplay.sprite = examinedWatch.WatchItem.itemImage;
            UIManager.instance.casingDisplay.sprite = examinedWatch.WatchItem.components[0].itemImage;
            if (examinedWatch.WatchItem.components.Count > 2)
            {
                UIManager.instance.decorDisplay.sprite = examinedWatch.WatchItem.components[2].itemImage;
            }
            else
            {
                UIManager.instance.decorDisplay.sprite = UIManager.instance.nothingImage;
            }

            for (int i = 0; i < UIManager.instance.casingComponentDisplays.Length; i++)
            {
                if (examinedWatch.WatchItem.components[0].components.Count > i)
                {
                    Debug.Log(examinedWatch.WatchItem.components[0].components[i].name);
                    UIManager.instance.casingComponentDisplays[i].sprite = examinedWatch.WatchItem.components[0].components[i].itemImage;
                }
                else
                {
                    UIManager.instance.casingComponentDisplays[i].sprite = UIManager.instance.nothingImage;
                }
            }

            for (int i = 0; i < UIManager.instance.mechComponentDisplays.Length; i++)
            {
                if (examinedWatch.WatchItem.components[1].components.Count > i)
                {
                    UIManager.instance.mechComponentDisplays[i].sprite = examinedWatch.WatchItem.components[1].components[i].itemImage;
                }
                else
                {
                    UIManager.instance.mechComponentDisplays[i].sprite = UIManager.instance.nothingImage;
                }
            }
        }
        else if(UIManager.instance.componentList.activeInHierarchy)
        {
            Debug.Log("Deactivating list");
            UIManager.instance.componentList.SetActive(false);
        }
    }
}
