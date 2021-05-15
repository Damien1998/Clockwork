﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    [SerializeField]private string buttonSound;

    private void Update()
    {
        PlayButtonSound(buttonSound);
    }
    public void PlayGame ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level10");
    }
    public void StartGame(int levelID)
    {
        Time.timeScale = 1f;
        UIManager.instance.QuickSettingButton.SetActive(true);
        SaveController.UnlockLevel(levelID);
        SceneManager.LoadScene($"Level{levelID}-City");
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
        if (SaveController.CheckForSaves(saveID))
        {
            SaveController.LoadGame(saveID);
        }
        StartGame(3);
    }

    public void PlayButtonSound(string sound)
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            SoundManager.PlaySound((SoundManager.Sound)Enum.Parse(typeof(SoundManager.Sound),sound));
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

   
}
