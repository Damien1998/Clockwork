using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class WatchComponent : Item
{
    public bool[] componentBroken;
    public bool[] componentExists;
    public int[] componentID;

    public bool isEmpty;
    public int numberOfComponents;

    public Sprite emptyImage;

    // Start is called before the first frame update
    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();

        for(int i = 0; i < 3; i++)
        {
            if (i < componentExists.Length && componentExists[i]) numberOfComponents++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!knownState) stateSprite.sprite = GameManager.instance.unknownImage;
        else if (unfixable) stateSprite.sprite = GameManager.instance.unfixableImage;
        else if (broken) stateSprite.sprite = GameManager.instance.complexBrokenImage;
        else if (isEmpty) stateSprite.sprite = emptyImage;
        else stateSprite.sprite = GameManager.instance.repairedImage;

        if (playerInRange)
        {
            PickUp(0);

            PickUp(1);
        }
    }
}
