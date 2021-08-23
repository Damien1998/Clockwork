using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorkshopMenu : MonoBehaviour
{
    public static WorkshopMenu instance;
    public GameObject menu;

    public int levelID;

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
        AnalyticsController.SendAnalyticDictionary("StartedWorkshopLevel","Level", GameManager.instance.levelID);
        Time.timeScale = 1f;
        GameManager.instance.levelID = levelID;
        UIManager.instance.LevelStart();
        SceneManager.LoadScene($"Level{sceneID}");
    }

    public void GoIntoWorkshop(int sceneID)
    {
        AnalyticsController.SendAnalyticResult("WentIntoWorkshop");
        Time.timeScale = 1f;
        GameManager.instance.levelID = levelID;
        SceneManager.LoadScene($"Level{sceneID}-Workshop");
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
