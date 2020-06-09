using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListItem : Item
{
    public Activator watch;
    public ComponentList watchComponentList;

    // Start is called before the first frame update
    void Start()
    {
        //Multiplayer setup
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();
        watchComponentList = FindObjectOfType<ComponentList>();
    }

    //When this object is enabled (i.e. the item is dropped) the script hides the component list
    private void OnEnable()
    {
        if(watchComponentList != null)
        {
            watchComponentList.Deactivate();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //State sprites
        stateSprite.sprite = null;

        if (playerInRange && !watchComponentList.componentList.activeInHierarchy)
        {
            PickUp(0);

            PickUp(1);
        }
    }

    //The inherited pick up method needs to be changed to account for the designer's programmer torture fantasies
    //Jk, this isn't that bad actually
    //But I'm.. let's say, not a mature individual
    public new void PickUp(int playerID)
    {
        //If the player inputs the pickup button, the specified player's PickupItem method is called and the item is deactivated
        if (interactingPlayer[playerID] != null)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) && !interactingPlayer[playerID].carriesItem && interactingPlayer[playerID].freeToPickup)
            {
                interactingPlayer[playerID].PickupItem(itemImage, stateSprite.sprite, activator);

                //See watch checklist
                if (watch != null && !watchComponentList.componentList.activeInHierarchy)
                {
                    watchComponentList.watch = watch.child.GetComponent<Watch>();
                    watchComponentList.Activate();
                }

                gameObject.SetActive(false);
            }
        }
    }
}
