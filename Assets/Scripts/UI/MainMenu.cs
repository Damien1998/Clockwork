using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level10");
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
    public void LoadSaves(int saveID)
    {
        SaveController _saveController = new SaveController();
        if (_saveController.CheckForSaves(saveID))
        {
            SaveController.LoadGame(saveID);
        }
        GameManager.instance.SetSaveController(_saveController);
        StartLevel(3);
    }

    public void PlayButtonSound(string sound)
    {
        SoundManager.PlaySound((SoundManager.Sound)Enum.Parse(typeof(SoundManager.Sound),sound));
    }
}
