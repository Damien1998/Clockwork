using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEndMenu : MonoBehaviour
{
    public int mainMenuIndex;

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void EndLevel()
    {
        SceneManager.LoadScene(mainMenuIndex);
    }
}
