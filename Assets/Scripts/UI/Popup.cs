using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public GameObject buttonExplanation;
    public TextMeshProUGUI explanationText;
    public Animator myAnimator;

    public ParticleSystem popupFX;

    [SerializeField]
    private GameObject otherPopup;

    public PopupOpener myOpener;

    public void SetPopupPosition(Vector2 worldPos)
    {
        otherPopup.SetActive(false);

        Vector2 viewportPoint = Camera.main.WorldToViewportPoint(worldPos);

        var rectTransform = GetComponent<RectTransform>();
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
        //rectTransform.position = Vector2.zero;

        var ps = Instantiate(popupFX, worldPos, Quaternion.identity);
        SoundManager.PlaySound(SoundManager.Sound.PoiInteraction);
        ps.Play();
        Destroy(ps, 3f);

        myAnimator.Play("PopupInctive");

        myAnimator.SetTrigger("Open");
    }

    public void ShowExplanationText(string text)
    {
        buttonExplanation.SetActive(true);
        explanationText.text = text;
        StartCoroutine(RebuildRectTransform((RectTransform)buttonExplanation.transform));
    }

    IEnumerator RebuildRectTransform(RectTransform rectTransform)
    {
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        yield return null;
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public void HidePopup()
    {
        myAnimator.SetTrigger("Close");
        if(myOpener != null)
        {
            myOpener.Unselect();
        }
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
