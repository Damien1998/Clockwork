using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    public void SetPopupPosition(Vector2 worldPos)
    {
        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
        //rectTransform.position = Vector2.zero;
    }
}
