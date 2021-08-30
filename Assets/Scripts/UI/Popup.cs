using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public GameObject buttonExplanation;
    public Text explanationText;

    public void SetPopupPosition(Vector2 worldPos)
    {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
        //rectTransform.position = Vector2.zero;
    }

    public void ShowExplanationText(string text)
    {
        buttonExplanation.SetActive(true);
        explanationText.text = text;
    }
}
