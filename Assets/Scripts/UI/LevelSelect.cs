using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public int selectedLevel = 1;
    public TextMeshProUGUI levelText;
    [SerializeField] private GameObject selectScreen;
    [SerializeField] private AnimatedPanel selectPanel;
    private bool interactable;


    private void Update()
    {
        if (interactable && Input.GetButtonDown("Pickup1"))
        {
            selectScreen.SetActive(true);
            selectPanel.Appear();
            levelText.text = "Wybrany poziom " + selectedLevel;
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
    public void NextLevelSelect()
    {
        selectedLevel++;
        levelText.text = "Wybrany poziom " + selectedLevel;
    }
    public void PreviousLevelSelect()
    {
        if (selectedLevel > 1)
        {
            selectedLevel--;
        }
        levelText.text = "Wybrany poziom " + selectedLevel;
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
