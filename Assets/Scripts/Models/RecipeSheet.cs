using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeSheet : MonoBehaviour
{
    public GameObject[] ComponentsLists;
    public GameObject Image,button,SideRecipes;

    private void Awake()
    {
        DisplayKnownRecipes();
    }

    public void ChooseSideRecipe()
    {

        for (int i = 0; i < SideRecipes.transform.childCount; i++)
        {
            if (SideRecipes.transform.GetChild(i).gameObject == EventSystem.current.currentSelectedGameObject)
            {
                Debug.Log(SideRecipes.transform.childCount);

                RecipeListView.currentMainWatch = RecipeListView.recipeLists[i];
                DisplayRecipe();
                break;
            }
        }
    }

    public void CleanRecipe()
    {
        foreach (var componentsList in ComponentsLists)
        {
            for (int i = 0; i < componentsList.transform.childCount; i++)
            {
                Destroy(componentsList.transform.GetChild(i).gameObject);
            }
        }
    }
    private void DisplayRecipe()
    {
       // Debug.Log(RecipeListView.currentMainWatch.WatchItem.itemID); 
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
    private void DisplayKnownRecipes()
    {
        if (RecipeListView.recipeLists.Count > 0)
        {
            for (int i = 0; i < RecipeListView.recipeLists.Count; i++)
            {
                if (i+1 > SideRecipes.transform.childCount)
                {
                    GameObject _quickRecipe = Instantiate(button, Vector3.zero, Quaternion.identity, SideRecipes.transform);
                    _quickRecipe.GetComponent<Image>().sprite = RecipeListView.recipeLists[i].WatchItem.itemImages[0];
                }
                else
                {
                    SideRecipes.transform.GetChild(i).GetComponent<Image>().sprite =
                        RecipeListView.recipeLists[i].WatchItem.itemImages[0];
                }
            }
        }
    }
    public void CloseSheet()
    {
        RecipeListView.UnloadRecipeView();
    }
}
