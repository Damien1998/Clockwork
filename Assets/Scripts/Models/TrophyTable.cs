﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrophyTable : MonoBehaviour
{
    private bool _playerInRange,opened;
    [SerializeField] private TextMeshProUGUI trophyDescription;
    [SerializeField] private TextMeshProUGUI trophyName;
    [SerializeField] private Animator bigTrophyAnimator; 
    [SerializeField] private Image TrophyImage;
    [SerializeField] private GameObject trophyDisplayTemplate;
    [SerializeField] private GameObject trophyDisplayList,trophyList,trophyDetails;
    [SerializeField] private AnimatedPanel trophyPanel;

    private void Update()
    {
        if (_playerInRange && Input.GetButtonDown("Pickup1")&&!opened)
        {
            DisplayTrophies();
        }
    }
    public void DisplayTrophies()
    {
        opened = true;
        var mySave = SaveController.currentSave;
        trophyList.SetActive(true);
        trophyPanel.Appear();
        for (int i = 0; i < trophyDisplayList.transform.childCount; i++)
        {
            Destroy(trophyDisplayList.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < mySave.completedSideQuests.Count; i++)
        {
            var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {mySave.completedSideQuests[i].TrophyID}");
            var trophyObject = Instantiate(trophyDisplayTemplate, trophyDisplayList.transform);
            trophyObject.transform.GetChild(0).GetComponent<Image>().sprite = myTrophy.trophyImage;
            trophyObject.GetComponent<TrophyTableDisplay>().CurrentTrophyTable = this;
            trophyObject.GetComponent<TrophyTableDisplay>().TrophyID = mySave.completedSideQuests[i].TrophyID;
        }
    }

    public void DisplayTrophiesNoAnimation()
    {
        opened = true;
        var mySave = SaveController.currentSave;
        trophyList.SetActive(true);
        trophyPanel.GetComponent<Animator>().Play("PanelActive");
        for (int i = 0; i < trophyDisplayList.transform.childCount; i++)
        {
            Destroy(trophyDisplayList.transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < mySave.completedSideQuests.Count; i++)
        {
            var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {mySave.completedSideQuests[i].TrophyID}");
            var trophyObject = Instantiate(trophyDisplayTemplate, trophyDisplayList.transform);
            trophyObject.transform.GetChild(0).GetComponent<Image>().sprite = myTrophy.trophyImage;
            trophyObject.GetComponent<TrophyTableDisplay>().CurrentTrophyTable = this;
            trophyObject.GetComponent<TrophyTableDisplay>().TrophyID = mySave.completedSideQuests[i].TrophyID;
        }
    }

    public void DisplayTrophy(int TrophyID)
    {
        var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {TrophyID}");
        TrophyImage.sprite = myTrophy.trophyImage;
        trophyDescription.text = myTrophy.Description;
        trophyName.text = myTrophy.trophyName;
        trophyDetails.SetActive(true);
        trophyList.SetActive(false);
        bigTrophyAnimator.SetTrigger("Open");
    }

    public void SetOpenToFalse()
    {
        opened = false;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
    }
}
