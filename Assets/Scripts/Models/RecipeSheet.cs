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
    private bool CheckForComponents(Item _itemToCheck)
    {
        if (_itemToCheck.components.Count > 0)
        {
            return true;
        }

        return false;
    }
    private void DisplayRecipe()
    {
        var WatchItem = RecipeListView.currentMainWatch.WatchItem;
        for (int i = WatchItem.itemImages.Count-1; i >= 0; i--)
        {
            var tmpGO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[0].transform);
            tmpGO.GetComponent<Image>().sprite = WatchItem.itemImages[i];
            tmpGO.transform.position = ComponentsLists[0].transform.position;
        }

        DisplayComponents(WatchItem,1);
    }

    private void DisplayComponents(Item _item,int _componentListIndex)
    {
        if (CheckForComponents(_item))
        {
            for (int i = 0; i < _item.components.Count; i++)
            {
                var GO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[_componentListIndex].transform);
                Destroy(GO.GetComponent<Image>());
                DisplayItem(_item.components[i], GO);
                DisplayComponents(_item.components[i],_componentListIndex+1);
            }   
        }
    }
    private void DisplayItem(Item _item,GameObject _itemPosition)
    {
        for (int i = _item.itemImages.Count-1; i >= 0; i--)
        {
            var tmpGO = Instantiate(Image, Vector3.zero, Quaternion.identity, _itemPosition.transform);
            tmpGO.GetComponent<Image>().sprite = _item.itemImages[i];
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
                    DisplayItem(RecipeListView.recipeLists[i].WatchItem, _quickRecipe);
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
