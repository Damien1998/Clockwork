using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkshopMenu : MonoBehaviour
{
    public static WorkshopMenu instance;
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void ShowMenu()
    {
        menu.SetActive(true);
    }

    public void StartLevel(int sceneID)
    {
        Time.timeScale = 1f;
        UIManager.instance.HUD.gameObject.SetActive(true);
        SceneManager.LoadScene(sceneID);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel") && menu.activeInHierarchy)
        {
            menu.SetActive(false);
        }
    }
}
