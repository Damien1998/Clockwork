using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class ComponentList : MonoBehaviour
{
    public Watch watch;
    public Image background;
    public GameObject componentList;

    public Image watchImg;
    public Image casingImg;
    public Image[] mechImg;
    public Image decorImg;
    public Image[] mechComponentImg;
    public Image[] casingComponentImg;

    public Sprite nullImg;


    // Start is called before the first frame update
    void Start()
    {
        background.gameObject.SetActive(false);
    }

    public void Activate()
    {
        background.gameObject.SetActive(true);

        //SetItemImage(watchImg, watch.itemID);
        SetItemImage(casingImg, watch.componentID[0]);
        SetItemImage(mechImg[0], 10);
        if (watch.hasDecor)
        {
            SetItemImage(decorImg, watch.componentID[2]);
        }
        else
        {
            decorImg.sprite = nullImg;
        }

        SetItemImage(mechImg[1], 10);
        for (int i = 0; i < 3; i++)
        {
            if (watch.hasMechComponent[i])
            {
                SetItemImage(mechComponentImg[i], watch.mechComponentID[i]);
            }
            else
            {
                mechComponentImg[i].sprite = nullImg;
            }
          //  SetItemImage(casingComponentImg[i], GameManager.instance.items[watch.componentID[0]].child.GetComponent<WatchComponent>().componentID[i]);
        }
    }

    private void SetItemImage(Image imgToChange, int itemID)
    {
        //imgToChange.sprite = GameManager.instance.items[itemID].child.GetComponent<SpriteRenderer>().sprite;
    }

    public void Deactivate()
    {
        background.gameObject.SetActive(false);
    }
}
