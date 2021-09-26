using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndMenu : MonoBehaviour
{
    public int mainMenuIndex;

    public void RestartLevel()
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeOut");
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevel()
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeOut");
        SceneManager.LoadSceneAsync(mainMenuIndex);
    }
}
