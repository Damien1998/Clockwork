using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //TODO sorting component IDs
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;
        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();

        for(int i = 0; i < 3; i++)
        {
            if (componentExists[i]) numberOfComponents++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!knownState) stateSprite.sprite = unknownImage;
        else if (unfixable) stateSprite.sprite = unfixableImage;
        else if (broken) stateSprite.sprite = brokenImage;
        else if (isEmpty) stateSprite.sprite = emptyImage;
        else stateSprite.sprite = repairedImage;

        if (playerCollided)
        {
            if (interactingPlayer[0] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && !interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup)
                {
                    interactingPlayer[0].carriesItem = true;
                    interactingPlayer[0].freeToPickup = false;
                    interactingPlayer[0].itemSprite.sprite = itemImage;
                    interactingPlayer[0].itemStateSprite.sprite = stateSprite.sprite;
                    interactingPlayer[0].droppedItemActivator = activator;
                    gameObject.SetActive(false);
                }
            }
            if (interactingPlayer[1] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && !interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup)
                {
                    interactingPlayer[1].carriesItem = true;
                    interactingPlayer[1].freeToPickup = false;
                    interactingPlayer[1].itemSprite.sprite = itemImage;
                    interactingPlayer[1].itemStateSprite.sprite = stateSprite.sprite;
                    interactingPlayer[1].droppedItemActivator = activator;
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
