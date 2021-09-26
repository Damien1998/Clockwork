using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController instance;
    
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
    public void LoadMenu()
    {
        UIManager.instance.transitionScreen.SetTrigger("FadeOut");
        SceneManager.LoadSceneAsync("Menu");
        UIManager.instance.StopAllUIPrograms();
        
    }

}
