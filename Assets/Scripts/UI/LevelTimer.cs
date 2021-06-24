using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private ProgressWheel progressWheel;

    public void SetValue(float value)
    {
        var clampedValue = Mathf.Clamp01(value);

        float angle = -360 * clampedValue;
        clockHand.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        progressWheel.SetValue(clampedValue);
    }
}
