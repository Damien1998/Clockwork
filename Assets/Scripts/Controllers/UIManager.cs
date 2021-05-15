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

    private IEnumerator timer;

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

    public void StopTimer()
    {
        StopCoroutine(timer);
    }

    private void ShowTrophyUI()
    {
        var levelQuest = ReturnLevel(GameManager.instance.levelID).Value.levelSideQuest;
        TrophyScreen.SetActive(levelQuest.completed);
        _QuestItemName.text = levelQuest.questTrophy.trophyName;
        ItemFlavourText.text = levelQuest.questTrophy.Description;
        trophyDisplay.sprite = levelQuest.questTrophy.trophyImage;

    }

    public void ShowLevelEnd()
    {
        levelEndText.text = "Your Time: " + timerDisplay.value.ToString();
        levelEndScren.SetActive(true);

        ShowTrophyUI();
        
        timerDisplay.gameObject.SetActive(false);
    }

    public void ShowLevelFailure()
    {
        //StopCoroutine(timer);
        levelFailureText.text = "Your Time: " + timerDisplay.value.ToString();
        levelFailureScren.SetActive(true);

        //trophyDisplay.SetActive(GameManager.instance.localQuestDone);

        timerDisplay.gameObject.SetActive(false);
    }

    public void AddPoints(int sumOfPoints)
    {
        Points += sumOfPoints;
    }

    public void LevelStart()
    {
        if (GameManager.instance.levelID != 4)
        {
            Time.timeScale = 1f;
            timerDisplay.value = 0;
            timerDisplay.maxValue = GameManager.instance.currentLevelParams.time;
            timerDisplay.gameObject.SetActive(true);
            timer = StartLevelTimer();
            StartCoroutine(timer);
        }
    }

    public void StopAllUIPrograms()
    {
        StopAllCoroutines();
        timerDisplay.gameObject.SetActive(false);
        timerDisplay.value = 0;
    }
    private IEnumerator StartLevelTimer()
    {
        while(timerDisplay.value<timerDisplay.maxValue)
        {
            timerDisplay.value++;
            yield return new WaitForSeconds(1);
        }
        timerDisplay.gameObject.SetActive(false);
        timerDisplay.value = 0;
        ShowLevelFailure();
        //gameOverScreen.gameObject.SetActive(true);
    }
}
