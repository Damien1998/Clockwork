using System;
using System.Collections;
using System.Collections.Generic;
using Polyglot;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public int selectedLevel = 1;
    public TextMeshProUGUI levelText;
    [SerializeField] private GameObject selectScreen, nextLevelButton, previousLevelButton;
    [SerializeField] private LocalizedTextMeshProUGUI levelNameText, dayText, namesText, descriptionText;
    [SerializeField] private AnimatedPanel selectPanel;
    private bool interactable;


    private void Update()
    {
        if (interactable && Input.GetButtonDown("Pickup1"))
        {
            selectScreen.SetActive(true);
            RenderLevelSelect();
            selectPanel.Appear();
            //levelText.text = "Poziom " + selectedLevel;
        }
    }

    //We need a proper system for this
    public void StartSelectedLevel()
    {
        GameManager.instance.levelID = selectedLevel;
        //LevelStart will be called at the end of a dialogue
        //UIManager.instance.LevelStart();
        //Scene transition
        UIManager.instance.transitionScreen.SetTrigger("FadeOut");
        //LoadSceneAsync for that sweet sweet loading screen
        Debug.Log(selectedLevel);
        SceneManager.LoadSceneAsync($"Level{selectedLevel}");
    }

    private void RenderLevelSelect()
    {
        var completedLevels = SaveController.CompletedLevels();
        levelText.text = "Wybrany poziom " + selectedLevel;

        if (completedLevels > selectedLevel)
        {
            nextLevelButton.SetActive(SaveController.levels[selectedLevel].completed);
        }
        else
        {
            nextLevelButton.SetActive(false);
        }
        if(1 < selectedLevel)
        {
            previousLevelButton.SetActive(SaveController.levels[selectedLevel - 1].completed);
        }
        else
        {
            previousLevelButton.SetActive(false);
        }
        
        var levelName = SaveController.levels[selectedLevel - 1].name;
        
        ChangeLocalization(levelNameText, $"WORKSHOP_{levelName}");
        ChangeLocalization(dayText, $"WORKSHOP_{levelName}_DAY");
        ChangeLocalization(namesText, $"WORKSHOP_{levelName}_CALENDAR_NAMES");
        ChangeLocalization(descriptionText, $"WORKSHOP_{levelName}_DESCRIPTION");
    }

    private void ChangeLocalization(LocalizedTextMeshProUGUI text, string key)
    {
        text.Key = key;
        text.OnLocalize();
    }
    
    public void NextLevelSelect()
    {
        selectedLevel++;
        RenderLevelSelect();
    }
    public void PreviousLevelSelect()
    {
        selectedLevel--;
        RenderLevelSelect();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            interactable = false;
        }
    }
}
