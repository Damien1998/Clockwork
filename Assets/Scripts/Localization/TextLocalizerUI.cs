using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

    [RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalizerUI : MonoBehaviour
{
    private TextMeshProUGUI textField;

    public LocalizedString localizedString;

    private void Start()
    {
        textField = GetComponent<TextMeshProUGUI>();
        string value = localizedString.value;
        textField.text = value;
    }

    public void ReInitialize()
    {
        string value = localizedString.value;
        textField.text = value;
    }
}
