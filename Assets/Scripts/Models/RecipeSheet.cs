using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[0].transform);
        for (int i = 0; i < RecipeListView.currentMainWatch.WatchItem.components.Count; i++)
        {
            Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[1].transform);
            for (int j = 0; j < RecipeListView.currentMainWatch.WatchItem.components[i].components.Count; j++)
            {
                Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[2].transform);
                for (int g = 0; g < RecipeListView.currentMainWatch.WatchItem.components[i].components[j].components.Count; g++)
                {
                    Instantiate(Image, Vector3.zero, Quaternion.identity, ComponentsLists[3].transform);
                } 
            } 
        } 

    }
    public void CloseSheet()
    {
        RecipeListView.UnloadRecipeView();
    }
}
