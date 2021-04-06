using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RecipeListView
{
    public static Watch currentMainWatch;
    public static List<Watch> recipeLists = new List<Watch>();

    public static void LoadRecipeView()
    {
        SceneManager.LoadSceneAsync("RecipeSheet",LoadSceneMode.Additive);
        //Player.SetPlayer(false);
    }

    public static void UnloadRecipeView()
    {
        SceneManager.UnloadSceneAsync("RecipeSheet");
        currentMainWatch.isSelected = false;
     //   Player.SetPlayer(true);
    }

    public static void AddRecipeToList(Watch _watch)
    {
        if (recipeLists.Count > 0)
        {
            for (int i = 0; i < recipeLists.Count; i++)
            {
                if (recipeLists[i] == _watch)
                {
                    break;
                }
                else
                {
                    recipeLists.Add(_watch);
                    UnloadRecipeView();
                    LoadRecipeView();
                }
            }
        }
        else
        {
            recipeLists.Add(_watch);
        }
    }
    public static void ResetRecipesList()
    {
        recipeLists = new List<Watch>();
    }
}
