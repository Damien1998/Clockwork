using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrophyTable : MonoBehaviour
{
    private bool _playerInRange;
    [SerializeField] private Text _trophiesAmount;
    [SerializeField] private GameObject _trophyCanvas,_trophiesPanelDisplay,_trophyDisplay;

    private void Update()
    {
        if (_playerInRange && Input.GetButtonDown("Pickup1") && !_trophyCanvas.activeSelf)
        {
            DisplayTrophies();
        }
    }
    private void DisplayTrophies()
    {
        _trophyCanvas.SetActive(true);
        var amountOfTrophiesToDisplay = 0;
        for (int i = 0; i < _trophiesPanelDisplay.transform.childCount; i++)
        {
          Destroy(_trophiesPanelDisplay.transform.GetChild(i).gameObject);
        }
        foreach (var trophy in SaveController.trophies)
        {
            if (trophy.completed)
            {
                Instantiate(_trophyDisplay, new Vector3(0, 0, 0), Quaternion.identity, _trophiesPanelDisplay.transform);
                amountOfTrophiesToDisplay++;
            }
        }
        _trophiesAmount.text = $"Trophies :{amountOfTrophiesToDisplay.ToString()}";
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
