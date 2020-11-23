using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level");
    }
    public void StartLevel(int sceneID)
    {
        Time.timeScale = 1f;
        UIManager.instance.QuickSettingButton.SetActive(true);
        SceneManager.LoadScene(sceneID);
    }

    public void SetGameManagerLevel(int levelID)
    {
        Time.timeScale = 1f;
        GameManager.instance.levelID = levelID;
    }

    public void QuitGame ()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

}
