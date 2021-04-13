using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchExamine : Workbench
{
    private bool invalidItemInside;

    [SerializeField] private ParticleSystem checkMark, crossMark;

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
            if (!workParticles.isEmitting)
            {
                Debug.Log("Workbench start");
                workParticles.Play();
            }
            Work();
        }

        if (workTimer <= 0)
        {
            workParticles.Stop();
            endParticles.Play();
            checkMark.Play();
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
        }

        //I made a different check for this
        //There will be different particle fx for dropping valid and invalid items
        if (invalidItemInside && workTimer <= (workTimerBase / 10))
        {
            workParticles.Stop();
            crossMark.Play();
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
                //I need to figure out a better way to check if an item is a watch
                if (itemToPlace.WatchItem.State == ItemState.UnknownState)
                {
                    invalidItemInside = false;
                    itemToPlace.WatchItem.State = itemToPlace.WatchItem.trueState;
                    Debug.Log(itemToPlace.WatchItem.State);
                    
                }
                else
                {
                    invalidItemInside = true;
                }
                if (itemToPlace.WatchItem.components.Count > 0)
                {
                    invalidItemInside = false;
                    GenerateComponentList(itemToPlace);
                }
                
                /**
                else if(itemToPlace.WatchItem.State != ItemState.UnknownState)
                {
                    invalidItemInside = true;
                }
                **/
                itemSlots[i] = itemToPlace;
                itemToPlace.gameObject.SetActive(false);
                break;
            }
        }
    }

    private void GenerateComponentList(Watch _watchItem)
    {
        RecipeListView.currentMainWatch = _watchItem;
        RecipeListView.AddRecipeToList(RecipeListView.currentMainWatch);
        RecipeListView.LoadRecipeView();
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
