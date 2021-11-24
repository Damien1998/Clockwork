﻿using System.Collections;
using Polyglot;
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
    private bool walkableScene;

    [SerializeField]
    private AnimatedPanel levelEndPanel, levelFailPanel;

    public bool IsPaused { get; private set; }
    public bool mouseBlocked;

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
        _QuestItemName.text = $"{Localization.Get($"TROPHY_{myTrophy.trophyName}_NAME",Localization.Instance.SelectedLanguage)}";
        ItemFlavourText.text = $"{Localization.Get($"TROPHY_{myTrophy.trophyName}_DESCRIPTION",Localization.Instance.SelectedLanguage)}";
        trophyDisplay.sprite = myTrophy.trophyImage;

    }

    public void WentIntoAdvancedSettings()
    {
        AnalyticsController.SendAnalyticResult($"Went Into Advanced Settings");
    }

    public void ShowLevelEnd()
    {
        levelEndText.text = $"{Localization.Get("WORKSHOP_TIME_END_UI",Localization.Instance.SelectedLanguage)}: {levelTimer.timerValue.ToString()}";
        levelEndScren.SetActive(true);
        //levelEndPanel.Appear();
        levelEndScren.GetComponent<Animator>().SetTrigger("Open");

        StartCoroutine(WaitAndPause(true));

        ShowTrophyUI();
        timerDisplay.gameObject.SetActive(false);
    }

    IEnumerator WaitAndPause(bool pause)
    {
        yield return null;
        PauseGame(pause);
    }

    public void ShowLevelFailure()
    {
        //StopCoroutine(timer);
        levelFailureScren.SetActive(true);
        //levelFailPanel.Appear();
        levelFailureScren.GetComponent<Animator>().SetTrigger("Open");

        StartCoroutine(WaitAndPause(true));

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
            IsPaused = true;

            var conveyors = FindObjectsOfType<ConveyorBelt>();

            foreach(ConveyorBelt c in conveyors)
            {
                c.SetAnimationSpeed(0);
            }

            if(activePauseCoroutine != null)
            {
                StopCoroutine(activePauseCoroutine);
            }

            if(Player.CanInteract)
            {
                Player.CanInteract = false;
                walkableScene = true;
            }
            else
            {
                walkableScene = false;
            }

            levelTimer.StopTimer();

            activePauseCoroutine = StartPause(0.3f);
            StartCoroutine(activePauseCoroutine);
        }
        else
        {
            var conveyors = FindObjectsOfType<ConveyorBelt>();

            foreach (ConveyorBelt c in conveyors)
            {
                c.SetAnimationSpeed(1);
            }

            IsPaused = false;

            if (activePauseCoroutine != null)
            {
                StopCoroutine(activePauseCoroutine);
            }

            if(walkableScene)
            {
                Player.CanInteract = true;
            }

            levelTimer.StartTimer();

            activePauseCoroutine = StartUnpause(0.3f);
            StartCoroutine(activePauseCoroutine);
        }
    }

    public void PauseGameNoBlur(bool pause)
    {
        if (pause)
        {
            IsPaused = true;

            var conveyors = FindObjectsOfType<ConveyorBelt>();

            foreach (ConveyorBelt c in conveyors)
            {
                c.SetAnimationSpeed(0);
            }

            if (Player.CanInteract)
            {
                Player.CanInteract = false;
                walkableScene = true;
            }
            else
            {
                walkableScene = false;
            }
        }
        else
        {
            IsPaused = false;

            var conveyors = FindObjectsOfType<ConveyorBelt>();

            foreach (ConveyorBelt c in conveyors)
            {
                c.SetAnimationSpeed(1);
            }

            if (walkableScene)
            {
                Player.CanInteract = true;
            }

            activePauseCoroutine = StartUnpause(0.3f);
            StartCoroutine(activePauseCoroutine);
        }
    }

    public void SetMouseBlocked(bool block)
    {
        mouseBlocked = block;
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
