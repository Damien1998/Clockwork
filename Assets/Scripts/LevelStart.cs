using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStart : MonoBehaviour
{
    public int levelID = 1;

    // Start is called before the first frame update
    void Start()
    {
        levelID = GameManager.instance.levelID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //This will not work in the long run
    //How about making a consistent scene naming scheme?
    //WorkshopLevel0, WorkshopLevel1, etc.
    public void StartLevel()
    {
        SceneManager.LoadScene("Level");
    }
}
