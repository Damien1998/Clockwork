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
    public UILineRenderer line;
    public float scaler=360;
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

                RecipeListView.currentMainWatch = RecipeListView.recipeLists[i];
                DisplayRecipe();
                break;
            }
        }
    }
    public void CleanRecipe()
    {
        line.ResetLines();
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
        DisplayComponents(ComponentsLists[0],WatchItem,1);
        line._points.Add((ComponentsLists[0].GetComponent<RectTransform>().anchoredPosition+ new Vector2(180,0))/scaler);
        StartCoroutine(Wait1s(WatchItem));
    }

    private void DrawLines(Item _watchItem,int _componentListIndex,int componentIndex,Vector2 _linePos)
    {
        if (CheckForComponents(_watchItem))
        {
            for (int i = 0; i < _watchItem.components.Count; i++)
            {
                Vector2 parentPos = ComponentsLists[_componentListIndex + 1].GetComponent<RectTransform>().anchoredPosition;
                Vector2 childPos =
                    ComponentsLists[_componentListIndex + 1].transform.GetChild(i).GetComponent<RectTransform>()
                        .anchoredPosition + parentPos;
                line._points.Add(childPos/scaler);
                if (_componentListIndex == 0)
                {
                    DrawLines(_watchItem.components[i],_componentListIndex + 1,i,_linePos);
                }
                else
                {
                    Vector2 myPos = ComponentsLists[_componentListIndex].transform.GetChild(componentIndex).GetComponent<RectTransform>()
                        .anchoredPosition + ComponentsLists[_componentListIndex].GetComponent<RectTransform>().anchoredPosition;
                    DrawLines(_watchItem.components[i],_componentListIndex + 1,i,myPos/scaler);
                }
            }
            line._points.Add(_linePos);
        }
        else
        {
            line._points.Add(_linePos);
        }
    }
    private void DisplayComponents(GameObject _itemImage,Item _item,int _componentListIndex)
    {
        if (CheckForComponents(_item))
        {
            for (int i = 0; i < _item.components.Count; i++)
            {
                var GO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[_componentListIndex].transform);
                GO.name = $"Component {i}";
                Destroy(GO.GetComponent<Image>());
                DisplayItem(_item.components[i], GO);
                DisplayComponents(GO,_item.components[i],_componentListIndex+1);
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

    IEnumerator Wait1s(Item _watchItem)
    {
        yield return new WaitForSeconds(1);
        DrawLines(_watchItem,0,0,(ComponentsLists[0].GetComponent<RectTransform>().anchoredPosition+ new Vector2(180,0))/scaler);
        line.gameObject.SetActive(false);
        yield return new WaitForSeconds(.01f);
        line.gameObject.SetActive(true);
    }
    
    public void CloseSheet()
    {
        RecipeListView.UnloadRecipeView();
    }
}
