using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private ProgressWheel progressWheel;

    public int timerValue, maxTimerValue;
    private IEnumerator timer;


    private void Awake()
    {
        UIManager.instance.levelTimer = this;
        timerValue = 0;
        maxTimerValue = GameManager.instance.currentLevelParams.time;
        timer = StartLevelTimer();
        StartTimer();
    }

    public void StopTimer()
    {
        StopCoroutine(timer);
    }
    public void StartTimer()
    {
        StartCoroutine(timer);
    }
    private void SetValue(float value)
    {
        var clampedValue = Mathf.Clamp01(value);

        float angle = -360 * clampedValue;
        clockHand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        progressWheel.SetValue(clampedValue);
    }
    private IEnumerator StartLevelTimer()
    {
        while(timerValue < maxTimerValue)
        {
            timerValue++;
            SetValue((float) timerValue / (float) maxTimerValue);
            yield return new WaitForSeconds(1);
        }
        UIManager.instance.ShowLevelFailure();
    }
}
