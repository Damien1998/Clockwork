using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeSheet : MonoBehaviour
{
    public GameObject[] ComponentsLists;
    public GameObject Image,RecipeTemplate,RecipesList;
    [SerializeField] private GameObject NormalObject;
    public UILineRenderer line;
    private bool displayingRecipe = false;
    private float scaler = 135;

    private void Awake()
    {
        DisplayAllRecipes();
    }

    private void DisplayAllRecipes()
    {
        StartCoroutine(DisplayRecipes());
    }
    private void AddRecipe()
    {
        var tmpRecipe = Instantiate(RecipeTemplate, RecipesList.transform);
        var WatchItem = RecipeListView.currentMainWatch.WatchItem;

        DisplayItem(WatchItem, tmpRecipe.transform.GetChild(0));
        for (int i = 0; i < WatchItem.components.Count; i++)
        {
            var tmpGO = Instantiate(NormalObject, Vector3.zero, Quaternion.identity, tmpRecipe.transform.GetChild(1));
            DisplayItem(WatchItem.components[i], tmpGO.transform);
        }

        //DisplayRecipe();
    }


    public void CleanRecipe()
    {
        displayingRecipe = false;
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
    private void DisplayRecipeOLD()
    {
        var WatchItem = RecipeListView.currentMainWatch.WatchItem;
        for (int i = WatchItem.itemImages.Length-1; i >= 0; i--)
        {
            var tmpGO = Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[0].transform);
            tmpGO.GetComponent<Image>().sprite = WatchItem.itemImages[i];
            tmpGO.GetComponent<RectTransform>().anchoredPosition = ComponentsLists[0].GetComponent<RectTransform>().anchoredPosition ;
        }
        DisplayComponents(ComponentsLists[0],WatchItem,1);
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
                AddLine(childPos+new Vector2(0, 10));//new Vector2 here is the offset which the lines should be at based on the item positions
                                                     //: for example if we have 4 component lists and each of them is 40 height then we would have to divide 40/10 which gives us 10
                                                     //in case of changing the ui once again change this to ComponentsLists[i].transform.parent.GetComponent<RectTransform>().rect.height/numberOfComponentLists
                if (_componentListIndex == 0)
                {
                    DrawLines(_watchItem.components[i],_componentListIndex + 1,i,_linePos);
                }
                else
                {
                    Vector2 myPos = ComponentsLists[_componentListIndex].transform.GetChild(componentIndex).GetComponent<RectTransform>()
                        .anchoredPosition + ComponentsLists[_componentListIndex].GetComponent<RectTransform>().anchoredPosition;
                    DrawLines(_watchItem.components[i],_componentListIndex + 1,i,myPos);
                }
            }
        }
        if (_componentListIndex > 1)
        {
            AddLine(_linePos + new Vector2(0, 10));
        }
        else
        {
            AddLine(_linePos);
        }
    }

    private void AddLine(Vector2 linePos)
    {
     line._points.Add(linePos/scaler);
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
                DisplayItem(_item.components[i], GO.transform);
                DisplayComponents(GO,_item.components[i],_componentListIndex+1);
            }
        }
    }
    private void DisplayItem(Item _item,Transform _itemPosition)
    {
        for (int i = _item.itemImages.Length-1; i >= 0; i--)
        {
            if (_item.itemImages[i] != null)
            {
                var tmpGO = Instantiate(Image, _itemPosition);
                tmpGO.GetComponent<Image>().sprite = _item.itemImages[i];
                tmpGO.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

            }
        }
    }

    IEnumerator DisplayRecipes()
    {
        foreach (var t in RecipeListView.recipeLists)
        {
            RecipeListView.currentMainWatch = t;
            AddRecipe();
            yield return new WaitForSeconds(.05f);
        }
    }
    IEnumerator Wait1s(Item _watchItem)
    {
        var recipeParentRect = ComponentsLists[0].transform.parent.GetComponent<RectTransform>().rect;
        Vector2 firstPointPos =
            ComponentsLists[0].transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition +
            new Vector2(recipeParentRect.width / 2, 0);
        AddLine(firstPointPos);
        yield return new WaitForSeconds(.01f);
        DrawLines(_watchItem,0,0,firstPointPos);
        line.gameObject.SetActive(false);
        yield return new WaitForSeconds(.01f);
        line.gameObject.SetActive(true);
    }

    public void CloseSheet()
    {
        RecipeListView.UnloadRecipeView();
    }
}
