using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupOpener : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private Popup myPopup;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ShowPopup);
    }

    void ShowPopup()
    {
        myPopup.SetPopupPosition(transform.position);
    }
}
