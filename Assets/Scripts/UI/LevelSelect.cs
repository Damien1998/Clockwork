﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public int selectedLevel = 1;
    public Text levelText;
    [SerializeField] private GameObject selectScreen;
    private bool interactable;

    private void Awake()
    {
        //DialogueManager.instance.StartDialogue("Test");
    }

    private void Update()
    {
        if (interactable && Input.GetButtonDown("Pickup1"))
        {
            selectScreen.SetActive(true);
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
        SceneManager.LoadSceneAsync(7);
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
