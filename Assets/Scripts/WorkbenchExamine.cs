using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchExamine : Workbench
{
    private bool invalidItemInside;

    private bool generateList;

    [SerializeField]
    private ListItem listTemplate;

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
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
        }

        //I made a different check for this
        //There will be different particle fx for dropping valid and invalid items
        if (invalidItemInside && workTimer <= (workTimerBase / 10))
        {
            timerDisplay.gameObject.SetActive(false);
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
                Debug.Log(itemToPlace.WatchItem.State);
                //If the item has 2 layers of components within, it's a watch
                if (itemToPlace.WatchItem.State == ItemState.UnknownState
                    || itemToPlace.WatchItem.components[0].components[0] != null)
                {
                    invalidItemInside = false;
                    if(itemToPlace.WatchItem.components.Count == 0)
                    {
                        itemToPlace.WatchItem.State = itemToPlace.WatchItem.trueState;
                        Debug.Log(itemToPlace.WatchItem.State);
                    }
                    else if(itemToPlace.WatchItem.components[0].components[0] != null && !itemToPlace.listDone && itemToPlace.TrueState != ItemState.Repaired)
                    {
                        generateList = true;
                    }
                }
                else
                {
                    invalidItemInside = true;
                }

                itemSlots[i] = itemToPlace;
                //itemToPlace.gameObject.SetActive(false);
                break;
            }
        }
    }

    protected override void DropItems()
    {
        ListItem listItem = null;
        if(generateList)
        {
            listItem = GenerateComponentList();
        }

        if (generateList)
        {
            itemSlots[0].listDone = true;
            if (verticalSpread)
            {
                itemSlots[0].transform.position = new Vector3(dropLocation.position.x, dropLocation.position.y - 1f, 0);
                listItem.transform.position = new Vector3(dropLocation.position.x, dropLocation.position.y + 1f, 0);
            }
            else
            {
                itemSlots[0].transform.position = new Vector3(dropLocation.position.x - 1f, dropLocation.position.y, 0);
                listItem.transform.position = new Vector3(dropLocation.position.x + 1f, dropLocation.position.y, 0);
            }
        }
        else
        {
            itemSlots[0].transform.position = dropLocation.position;
        }
        
        itemSlots[0] = null;
        generateList = false;
    }

    private ListItem GenerateComponentList()
    {
        var listItem = Instantiate(listTemplate, transform.position, Quaternion.identity);
        //var listItemData = itemSlots[0].WatchItem;
        listItem.GetComponent<ListItem>().examinedItem = itemSlots[0];
        return listItem.GetComponent<ListItem>();
    }

    //I'm leaving this in in case I need it later

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

}
