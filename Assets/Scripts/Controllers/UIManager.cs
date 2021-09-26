using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SaveController;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public GameObject QuickSettingButton;
    [SerializeField] private int Points;
    [SerializeField] private Slider timerDisplay;
    [SerializeField] private GameObject levelEndScren;
    [SerializeField] private GameObject levelFailureScren;
    [SerializeField] private GameObject TrophyScreen;
    [SerializeField] private TextMeshProUGUI levelEndText;
    //[SerializeField] private Text levelFailureText;
    [SerializeField] private TextMeshProUGUI _QuestItemName, ItemFlavourText;
    [SerializeField] private Image trophyDisplay;

    public LevelTimer levelTimer;

    public Animator transitionScreen;
    public bool transitionFinished;

    private float currentBlurStrength;
    private IEnumerator activePauseCoroutine;
    [SerializeField]
    private Volume pauseFXVolume;
    private DepthOfField blur;


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

        pauseFXVolume.profile.TryGet(out blur);
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
        levelEndScren.GetComponent<AnimatedPanel>().Appear();

        PauseGame(true);

        ShowTrophyUI();

        timerDisplay.gameObject.SetActive(false);
    }

    public void ShowLevelFailure()
    {
        //StopCoroutine(timer);
        //levelFailureText.text = "Twój czas: " + timerDisplay.value.ToString();
        levelFailureScren.SetActive(true);
        levelFailureScren.GetComponent<AnimatedPanel>().Appear();

        PauseGame(true);

        //trophyDisplay.SetActive(GameManager.instance.localQuestDone);
        SoundManager.PlaySound(SoundManager.Sound.AlarmRing);
        AnalyticsController.SendAnalyticDictionary("LostLevel", "Level", GameManager.instance.levelID);

        timerDisplay.gameObject.SetActive(false);
    }

    public void AddPoints(int sumOfPoints)
    {
        Points += sumOfPoints;
    }

    public void PauseGame(bool pause)
    {
        if (pause)
        {
            if(activePauseCoroutine != null)
            {
                StopCoroutine(activePauseCoroutine);
            }
            activePauseCoroutine = StartPause(0.3f);
            StartCoroutine(activePauseCoroutine);
        }
        else
        {
            if (activePauseCoroutine != null)
            {
                StopCoroutine(activePauseCoroutine);
            }
            activePauseCoroutine = StartUnpause(0.3f);
            StartCoroutine(activePauseCoroutine);
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

    IEnumerator StartPause(float duration)
    {
        while(currentBlurStrength < 1)
        {
            currentBlurStrength += Time.deltaTime * (1 / duration);
            blur.focalLength.value = 1 + (39 * currentBlurStrength);
            yield return null;
        }

        currentBlurStrength = 1;
        blur.focalLength.value = 1 + (39 * currentBlurStrength);
        //Time.timeScale = 0f;
    }

    IEnumerator StartUnpause(float duration)
    {
        //Time.timeScale = 1f;

        while (currentBlurStrength > 0)
        {
            currentBlurStrength -= Time.deltaTime * (1 / duration);
            blur.focalLength.value = 1 + (39 * currentBlurStrength);
            yield return null;
        }

        currentBlurStrength = 0;
        blur.focalLength.value = 1 + (39 * currentBlurStrength);

    }

    public void StopAllUIPrograms()
    {
        StopAllCoroutines();
        timerDisplay.gameObject.SetActive(false);
        timerDisplay.value = 0;
    }
}
