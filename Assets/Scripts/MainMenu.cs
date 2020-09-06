using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    public Button[] levelButtons;

    private void Start()
    {
        if(levelButtons.Length == GameManager.instance.levels.Count)
        {
            for(int i = 0; i < GameManager.instance.levels.Count; i++)
            {
                if(GameManager.instance.levels[i].unlocked)
                {
                    levelButtons[i].interactable = true;
                }
                else
                {
                    levelButtons[i].interactable = false;
                }
            }
        }
    }

    public void PlayGame ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level");
    }

    public void StartLevel(int sceneID)
    {
        Time.timeScale = 1f;
        GameManager.instance.HUD.gameObject.SetActive(true);
        GameManager.instance.endDisplay.gameObject.SetActive(false);
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
