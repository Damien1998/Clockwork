using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeSheet : MonoBehaviour
{
    public GameObject[] ComponentsLists;
    public GameObject Image;
    private void Awake()
    {
        DisplayRecipe();
    }

    private void DisplayRecipe()
    {
        var WatchItem = RecipeListView.currentMainWatch.WatchItem;
        for (int i = WatchItem.itemImages.Count-1; i > 0; i--)
        {
            var tmpGO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[0].transform);
            tmpGO.GetComponent<Image>().sprite = WatchItem.itemImages[i];
            tmpGO.transform.position = ComponentsLists[0].transform.position;
        }
        for (int i = 0; i < WatchItem.components.Count; i++)
        {
            var GO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[1].transform);
            GO.GetComponent<Image>().sprite = WatchItem.components[i].itemImages[0];
            if (WatchItem.components[i].components.Count > 0)
            {
                for (int j = 0; j < WatchItem.components[i].components.Count - 1; j++)
                {
                    var GO2 = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[2].transform);
                    GO2.GetComponent<Image>().sprite = WatchItem.components[i].components[j].itemImages[0];
                    // for (int g = 0; g < WatchItem.components[j].components.Count - 1; g++)
                    // {
                    //     var GO3 = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[3].transform);
                    //     GO3.GetComponent<Image>().sprite =
                    //         WatchItem.components[i].components[j].components[g].itemImages[0];
                    // }
                }
            }
        }
    }
    public void CloseSheet()
    {
        RecipeListView.UnloadRecipeView();
        Player.CanInteract = true;
    }
}
