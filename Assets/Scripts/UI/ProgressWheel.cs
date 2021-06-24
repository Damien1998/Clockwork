using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressWheel : MonoBehaviour
{
    [SerializeField] private Image progressBar;

    public void SetValue(float value)
    {
        var clampedValue = Mathf.Clamp01(value);

        progressBar.fillAmount = clampedValue;
    }
}
