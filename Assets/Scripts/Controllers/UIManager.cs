using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveController;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject QuickSettingButton;
    [SerializeField] private int Points;
    [SerializeField] private Slider timerDisplay;
    [SerializeField] private GameObject levelEndScren;
    [SerializeField] private GameObject levelFailureScren;
    [SerializeField] private GameObject TrophyScreen;
    [SerializeField] private Text levelEndText;
    [SerializeField] private Text levelFailureText;
    [SerializeField] private TextMeshProUGUI _QuestItemName, ItemFlavourText;
    [SerializeField] private Image trophyDisplay;

    public LevelTimer levelTimer;

    public Animator transitionScreen;
    public bool transitionFinished;


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

        transitionScreen.SetTrigger("FadeIn");
    }



    private void ShowTrophyUI()
    {
        var levelQuest = ReturnLevel(GameManager.instance.levelID).Value.levelSideQuest;
        TrophyScreen.SetActive(levelQuest.completed);
        var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {levelQuest.TrophyID}");
        _QuestItemName.text = myTrophy.trophyName;
        ItemFlavourText.text = myTrophy.Description;
        trophyDisplay.sprite = myTrophy.trophyImage;

    }

    public void ShowLevelEnd()
    {
        levelEndText.text = "Twój czas: " + timerDisplay.value.ToString();
        levelEndScren.SetActive(true);

        ShowTrophyUI();

        timerDisplay.gameObject.SetActive(false);
    }

    public void ShowLevelFailure()
    {
        //StopCoroutine(timer);
        levelFailureText.text = "Twój czas: " + timerDisplay.value.ToString();
        levelFailureScren.SetActive(true);

        //trophyDisplay.SetActive(GameManager.instance.localQuestDone);
        AnalyticsController.SendAnalyticDictionary("LostLevel", "Level", GameManager.instance.levelID);

        timerDisplay.gameObject.SetActive(false);
    }

    public void AddPoints(int sumOfPoints)
    {
        Points += sumOfPoints;
    }

    public void PauseGame(bool pause)
    {
        if(pause)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void LevelStart()
    {
        if (GameManager.instance.levelID != 4)
        {
            Time.timeScale = 1f;
            timerDisplay.value = 0;
            timerDisplay.maxValue = GameManager.instance.currentLevelParams.time;
            timerDisplay.gameObject.SetActive(true);

        }
    }

    public void StopAllUIPrograms()
    {
        StopAllCoroutines();
        timerDisplay.gameObject.SetActive(false);
        timerDisplay.value = 0;
    }
}
