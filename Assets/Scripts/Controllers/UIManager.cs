using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject QuickSettingButton;
    [SerializeField] private int Points; 
    [SerializeField]private GameObject gameOverScreen;
    [SerializeField]private Slider timerDisplay;
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

    public void AddPoints(int sumOfPoints)
    {
        Points += sumOfPoints;
    }
    public void LevelStart()
    {
        if (GameManager.instance.levelID != 4)
        {
            timerDisplay.value = 0;
            timerDisplay.maxValue = GameManager.instance.currentLevelParams.time;
            timerDisplay.gameObject.SetActive(true);
            StartCoroutine(StartLevelTimer());
        }
    }
    private IEnumerator StartLevelTimer()
    {
        while(timerDisplay.value<timerDisplay.maxValue)
        {
            timerDisplay.value++;
            yield return new WaitForSeconds(1);
        }
        timerDisplay.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
    }
}
