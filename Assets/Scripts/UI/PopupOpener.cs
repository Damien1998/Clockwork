using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupOpener : MonoBehaviour
{
    private Button button;
    [SerializeField]
    private Popup myPopup;
    [SerializeField]
    private Image myRenderer;
    [SerializeField]
    private Material selectedMat, unselectedMat;
   
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<Image>();
        //button = GetComponent<Button>();
        //button.onClick.AddListener(ShowPopup);
    }

    public virtual void ShowPopup()
    {
        if(!UIManager.instance.mouseBlocked && !UIManager.instance.IsPaused)
        {
            myRenderer.material = selectedMat;
            myPopup.SetPopupPosition(transform.position);
            myPopup.myOpener = this;
        }      
    }

    public void Unselect()
    {
        myRenderer.material = unselectedMat;
    }
}
