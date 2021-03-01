using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RecipeListView
{
    public static Watch currentMainWatch;

    public static void LoadRecipeView()
    {
        SceneManager.LoadSceneAsync("RecipeSheet",LoadSceneMode.Additive);
        Player.SetPlayer(false);
    }

    public static void UnloadRecipeView()
    {
        SceneManager.UnloadSceneAsync("RecipeSheet");
        currentMainWatch.isSelected = false;
        Player.SetPlayer(true);
    }
}
