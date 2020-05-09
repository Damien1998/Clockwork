using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Refactoring is done! You may enter safely
public class MainMenu : MonoBehaviour
{
    public void PlayGame ()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitGame ()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

}
