using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    public Slider timerDisplay;
    public Text pointDisplay;
    public Text pointDisplayEnd;
    
    public Canvas HUD;

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
        
        timerDisplay.gameObject.SetActive(false);
    }


}
