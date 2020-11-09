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

    public GameObject componentList;
    public List<ListButton> listButtons;
    public Image watchDisplay, casingDisplay, decorDisplay;
    public Image[] casingComponentDisplays, mechComponentDisplays;

    public GameObject buttonLayoutGroup;
    public Button listButtonTemplate;

    public Sprite nothingImage;

    public Watch listItem;

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
        
        //timerDisplay.gameObject.SetActive(false);
    }


}
