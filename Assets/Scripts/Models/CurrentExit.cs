using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CurrentExit : MonoBehaviour
{
    private bool interactable;



    private void Update()
    {
        if (interactable && Input.GetButtonDown("Pickup1"))
        {
            SceneManager.LoadScene($"Level{GameManager.instance.levelID}-City");
        }
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
