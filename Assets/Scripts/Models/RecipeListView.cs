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
        SceneManager.LoadSceneAsync("RecipeSheet", LoadSceneMode.Additive);
        //Player.SetPlayer(false);
    }

    public static void UnloadRecipeView()
    {
        SceneManager.UnloadSceneAsync("RecipeSheet");
        //   Player.SetPlayer(true);
    }

    public static void ReloadRecipeView()
    {
        SceneManager.UnloadSceneAsync("RecipeSheet");
        SceneManager.LoadSceneAsync("RecipeSheet", LoadSceneMode.Additive);
    }

    public static void AddRecipeToList(Watch _watch)
    {
        if (!recipeLists.Contains(_watch))
        {
            recipeLists.Add(_watch);
            if (SceneManager.GetSceneByName("RecipeSheet").isLoaded)
            {
                ReloadRecipeView();
            }
            else
            {
                LoadRecipeView();
            }
        }
    }

    public static void RemoveCheckForRecipes(Watch watch)
    {
        for (int i = 0; i < recipeLists.Count; i++)
        {
            if (watch.WatchItem.itemID == recipeLists[i].WatchItem.itemID)
            {
                recipeLists.Remove(recipeLists[i]);
                ReloadRecipeView();
            }
        }
    }

    public static void ResetRecipesList()
    {
        recipeLists = new List<Watch>();
    }
}
