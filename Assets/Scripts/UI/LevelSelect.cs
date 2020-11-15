using System;
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
    private void Update()
    {
        if (interactable && Input.GetButtonDown("Pickup1"))
        {
            selectScreen.SetActive(true);
            levelText.text = "Wybrany poziom " + selectedLevel;
        }
    }

    public void StartSelectedLevel()
    {
        GameManager.instance.levelID = selectedLevel;
        SceneManager.LoadScene(1);
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
