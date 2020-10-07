using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchExamine : Workbench
{
    private bool invalidItemInside;

    // Start is called before the first frame update
    void Start()
    {
        //This workbench only has one slot
        //Use multiple if you want more at the same time
        numberOfSlots = 1;
        itemSlots = new Watch[1];

        workTimer = workTimerBase;
    }

    // Update is called once per frame
    void Update()
    {
        if (itemSlots[0] != null)
        {
            Work();
        }

        if (workTimer <= 0)
        {
            DropItems();
            workTimer = workTimerBase;
        }

        //I made a different check for this
        //There will be different particle fx for dropping valid and invalid items
        if (invalidItemInside && workTimer <= (workTimerBase / 10))
        {
            DropItems();
            workTimer = workTimerBase;
        }
    }

    //Fixing the item is done when the item is placed on the workbench
    public override void PlaceItem(Watch itemToPlace)
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] == null)
            {
                //I need to figure out a better way to check if an item is a watch
                if (itemToPlace.WatchItem.State == ItemState.UnknownState
                    || itemToPlace.WatchItem.itemID < 5)
                {
                    invalidItemInside = false;
                    //Here the function will set the state
                }
                else
                {
                    invalidItemInside = true;
                }

                itemSlots[i] = itemToPlace;
                itemToPlace.gameObject.SetActive(false);
                break;
            }
        }
    }

    protected override void Work()
    {
        workTimer -= Time.deltaTime;
    }

    protected override void DropItems()
    {
        //Here the function will generate the list of watch components
        base.DropItems();
    }

    // private bool IsItemValid(Activator item)
    // {
    //     // return (!item.child.GetComponent<Watch>() 
    //     //     && !item.child.GetComponent<WatchComponent>()
    //     //     && !item.child.GetComponent<ListItem>());
    //     return false;
    // }

    // private void UseExamineWorkbench(int playerID)
    // {
    //     //TODO Examining a watch for a list of components;
    //     if (interactingPlayer[playerID] != null && timer <= -1)
    //     {
    //         if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) 
    //             && interactingPlayer[playerID].carriesItem 
    //             && interactingPlayer[playerID].freeToPickup)
    //         {
    //             //Is the item valid for examination (i.e. is the item not a watch, list, casing or mechanism
    //             if (!interactingPlayer[playerID].droppedItemActivator.child.knownState
    //                 && IsItemValid(interactingPlayer[playerID].droppedItemActivator))
    //             {
    //                 timer = timerBase;
    //                 item = interactingPlayer[playerID].droppedItemActivator;
    //                 itemSlot.sprite = item.child.itemImage;
    //
    //                 interactingPlayer[playerID].ClearItem();
    //
    //                 item.child.knownState = true;
    //                 if (item.child.broken && Random.Range(0, 2) == 0)
    //                 {
    //                     item.child.unfixable = true;
    //                 }                                     
    //             }
    //
    //             if(interactingPlayer[playerID].droppedItemActivator != null && interactingPlayer[playerID].droppedItemActivator.child.GetComponent<Watch>() != null
    //                 && !interactingPlayer[playerID].droppedItemActivator.child.GetComponent<Watch>().examined)
    //             {
    //                 timer = timerBase;
    //                 item = interactingPlayer[playerID].droppedItemActivator;
    //                 itemSlot.sprite = item.child.itemImage;
    //                 item.child.GetComponent<Watch>().examined = true;
    //
    //                 interactingPlayer[playerID].ClearItem();
    //             }
    //         }
    //     }       
    // }

    // private void DropItems()
    // {
    //     if (item != null)
    //     {
    //         Vector3 direction = Vector3.zero;
    //         if(item.child.GetComponent<Watch>())
    //         {
    //             direction = new Vector3(Random.Range(1, 3), Random.Range(0, -1), 0);
    //             Activator temp = Instantiate(GameManager.instance.items[44], transform.position + direction, transform.rotation);
    //             temp.child.GetComponent<ListItem>().watch = item;
    //         }
    //         direction = new Vector3(Random.Range(1, 3), Random.Range(0, -1), 0);
    //         item.transform.position = transform.position + direction;
    //         item.SetChildState(true);
    //         item = null;
    //     }
    //     itemSlot.sprite = null;
    //     timer = -1;
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = true;
        if (interactingPlayer[0] == null)
        {
            interactingPlayer[0] = collision.GetComponent<Player>();
            interactingPlayer[0].isByWorkbench = true;
        }
        else
        {
            interactingPlayer[1] = collision.GetComponent<Player>();
            interactingPlayer[1].isByWorkbench = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
        if (interactingPlayer[0] == collision.GetComponent<Player>())
        {
            interactingPlayer[0].isByWorkbench = false;
            interactingPlayer[0] = null;
        }
        else
        {
            interactingPlayer[1].isByWorkbench = false;
            interactingPlayer[1] = null;
        }

    }
}
