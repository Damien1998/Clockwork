using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    [SerializeField]private string buttonSound;

    private void Start()
    {
        //Makes the UI appear
        GetComponent<Animator>().SetTrigger("Open");
    }

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
        AnalyticsController.SendAnalyticResult($"StartedLevel {levelID}");
        Time.timeScale = 1f;
        UIManager.instance.QuickSettingButton.SetActive(true);
        if (SaveController.currentSave == null)
        {
            SaveController.CreateSaveGame(0);
        }

        StartCoroutine(WaitAndStartGame());
    }

    IEnumerator WaitAndStartGame()
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeOut");
        yield return new WaitForSeconds(0.7f);
        GameManager.instance.StartCityLevel(10);
        
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
        StartGame(10);
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
