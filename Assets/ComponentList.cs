using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComponentList : MonoBehaviour
{
    public Watch watch;
    public Image bg;
    public GameObject componentList;
    public Image watchImg;
    public Image casingImg;
    public Image mechImg1;
    public Image decorImg;
    public Image mechImg2;
    public Image bitImg1;
    public Image bitImg2;
    public Image bitImg3;
    public Image boxImg;
    public Image beltImg;
    public Image glassImg;
    public Vector2 pos;
    public Sprite nullImg;
    // Start is called before the first frame update
    void Start()
    {
        pos = bg.gameObject.transform.position;
        bg.gameObject.transform.position = new Vector2(-500, pos.y);
    }

    public void Activate()
    {
        Debug.Log(watch.itemID);
        bg.gameObject.transform.position = pos;
        watchImg.sprite = GameManager.instance.items[watch.itemID].child.GetComponent<SpriteRenderer>().sprite;
        casingImg.sprite = GameManager.instance.items[watch.componentID[0]].child.GetComponent<SpriteRenderer>().sprite;
        mechImg1.sprite = GameManager.instance.items[10].child.GetComponent<SpriteRenderer>().sprite;
        if (watch.hasDecor) decorImg.sprite = GameManager.instance.items[watch.componentID[2]].child.GetComponent<SpriteRenderer>().sprite;
        else decorImg.sprite = nullImg;
        mechImg2.sprite = GameManager.instance.items[10].child.GetComponent<SpriteRenderer>().sprite;
        if (watch.hasMechComponent[0]) bitImg1.sprite = GameManager.instance.items[watch.mechComponentID[0]].child.GetComponent<SpriteRenderer>().sprite;
        else bitImg1.sprite = nullImg;
        if (watch.hasMechComponent[1]) bitImg2.sprite = GameManager.instance.items[watch.mechComponentID[1]].child.GetComponent<SpriteRenderer>().sprite;
        else bitImg2.sprite = nullImg;
        if (watch.hasMechComponent[2]) bitImg3.sprite = GameManager.instance.items[watch.mechComponentID[2]].child.GetComponent<SpriteRenderer>().sprite;
        else bitImg3.sprite = nullImg;
        boxImg.sprite = GameManager.instance.items[GameManager.instance.items[watch.componentID[0]].child.GetComponent<WatchComponent>().componentID[0]].child.GetComponent<SpriteRenderer>().sprite;
        beltImg.sprite = GameManager.instance.items[GameManager.instance.items[watch.componentID[0]].child.GetComponent<WatchComponent>().componentID[1]].child.GetComponent<SpriteRenderer>().sprite;
        glassImg.sprite = GameManager.instance.items[GameManager.instance.items[watch.componentID[0]].child.GetComponent<WatchComponent>().componentID[2]].child.GetComponent<SpriteRenderer>().sprite;

    }
    public void Deactivate()
    {
        bg.gameObject.transform.position = new Vector2(-500, pos.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
