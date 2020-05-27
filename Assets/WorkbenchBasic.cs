using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Refactoring is done! You may enter safely
public class WorkbenchBasic : MonoBehaviour
{
    //Item slots
    public Activator[] item;

    public bool playerInRange;
    public Player[] interactingPlayer;

    //Workbench timer
    public float timerBase;
    public float timer;

    //Item slot images
    public SpriteRenderer[] itemSlots;
    private int itemSlotsNumber = 3;

    // Start is called before the first frame update
    void Start()
    {
        //Multiplayer stuff
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        item = new Activator[itemSlotsNumber];
    }

    // Update is called once per frame
    void Update()
    {
        if (interactingPlayer[0] != null)
        {
            BasicWorkbenchInput(0);
            BasicWorkbenchOutput(0);
        }
        if(interactingPlayer[1] != null)
        {
            BasicWorkbenchInput(1);
            BasicWorkbenchOutput(1);
        }
    }

    //Basic workbench item input functionality
    private void BasicWorkbenchInput(int playerID)
    {
        if (interactingPlayer[playerID] != null)
        {
            //If the player has an item in hand..
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) 
                && interactingPlayer[playerID].carriesItem 
                && interactingPlayer[playerID].freeToPickup 
                && interactingPlayer[playerID].droppedItemActivator.child.knownState)
            {

                //Is the item valid for the basic workbench:

                //1. Is there nothing on the table and the item is a broken watch or casing?
                if (item[0] == null 
                    && interactingPlayer[playerID].droppedItemActivator.child.itemID < 10 
                    && interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    Debug.Log("Item Placed");
                    item[0] = interactingPlayer[playerID].droppedItemActivator;
                    itemSlots[0].sprite = item[0].child.itemImage;
                    interactingPlayer[playerID].ClearItem();                   
                    timer = timerBase;
                }
                //2. Is the item a fixed watch or casing component
                else if (!interactingPlayer[0].droppedItemActivator.child.broken 
                    && (interactingPlayer[0].droppedItemActivator.child.itemID < 26 && interactingPlayer[0].droppedItemActivator.child.itemID > 4) 
                    && !interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    for (int i = 0; i < itemSlotsNumber; i++)
                    {
                        //if the slot is empty place the item
                        if (item[i] == null)
                        {
                            Debug.Log("Item Placed");
                            item[i] = interactingPlayer[playerID].droppedItemActivator;
                            itemSlots[i].sprite = item[i].child.itemImage;
                            interactingPlayer[playerID].ClearItem();
                            timer = timerBase;
                            break;
                        }
                    }
                }
            }
        }                        
    }

    //Basic workbench item output functionality
    private void BasicWorkbenchOutput(int playerID)
    {
        if (interactingPlayer[playerID] != null)
        {
            //Is there anything on the table?
            if (item[0] != null)
            {
                //While the player holds the input, the timer ticks down
                if (Input.GetButton("Action" + interactingPlayer[playerID].playerNumber))
                {
                    timer -= Time.deltaTime;
                }
                if (Input.GetButtonUp("Action" + interactingPlayer[playerID].playerNumber))
                {
                    timer = timerBase;
                }

                //Is the item to be dissected?
                //All that needs to be checked is whether it is broken
                if (item[0].child.broken)
                {
                    //if the time is almost up
                    //This is probably not needed now that the workbenches are separated into different script files
                    //TODO - see if the world breaks down once I change the argument to "timer <= 0"
                    if (timer <= 0)
                    {
                        SearchRecipesBreak();
                        DropItems();
                    }
                }
            
                //Is the item to be combined (isn't broken)
                //Adding a check for more than item means this part of the script doesn't run when not needed - with single items
                else if (item[1] != null)
                {                   
                    if (timer <= 0)
                    {
                        SearchRecipesCombine();
                        DropItems();
                    }
                }
            }
        }            
    }

    //There maybe I should add a way to fill slots without specifying the state of the item inside
    private void FillSlot(int slotID, int itemID, bool broken)
    {
        item[slotID] = Instantiate(GameManager.instance.items[itemID]);
        item[slotID].child.broken = broken;
        item[slotID].SetChildState(false);
    }

    //Sorts ALL items in the workbench
    private void SortItems()
    {
        Debug.Log("Sort!");
        List<Activator> temp = new List<Activator>();
        for (int i = 0; i < itemSlotsNumber; i++)
        {
            if (item[i] != null)
            {
                temp.Add(item[i]);
            }
        }
        temp = temp.OrderBy(it => it.child.itemID).ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            item[i] = temp[i];
        }
    }

    //Sorts items up to a point - probably not needed
    private void SortItems(int lastSlot)
    {
        Debug.Log("Sort!");
        if(lastSlot <= itemSlotsNumber)
        {
            List<Activator> temp = new List<Activator>();
            for (int i = 0; i < lastSlot; i++)
            {
                if (item[i] != null)
                {
                    temp.Add(item[i]);
                }
            }
            temp = temp.OrderBy(it => it.child.itemID).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                item[i] = temp[i];
            }
        }      
    }

    //Drops everything. The area specified is implemented in an extremely clunky way by providing the direction in code
    //TODO - making a better algorithm
    private void DropItems()
    {
        for (int i = 0; i < itemSlotsNumber; i++)
        {
            if (item[i] != null)
            {
                Vector3 direction = Vector3.zero;
                direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), 0);
                item[i].transform.position = transform.position + direction;
                item[i].SetChildState(true);
                item[i] = null;
            }
            itemSlots[i].sprite = null;
        }
        timer = -1;
    }

    //Searches through all recipes to find one that combines all items on the workbench
    private void SearchRecipesCombine()
    {
        GameManager gameManager = GameManager.instance;

        SortItems();

        //A loop through all basic workbench recipes
        for(int i = 0; i < gameManager.basicRecipes.Length; i++)
        {
            int counter = 0;
            //A loop for all slots checking whether all of them match
            //This is the reason all recipes must be sorted
            //This makes the algorithm faster by 'a shitload'
            //It's a mathematical term
            //Cue flashbacks from algorithmics extra classes
            for(int j = 0; j < 3; j++)
            {
                if ((item[j] != null && item[j].child.itemID == gameManager.basicRecipes[i].partID[j])
                    ||(item[j] == null && gameManager.basicRecipes[i].partID[j] == -1))
                {
                    counter++;
                }
            }

            //If a matching recipe is found..
            if(counter == 3)
            {
                //Fills the first slot with the recipe result and empties all other slots
                item[0] = Instantiate(gameManager.items[gameManager.basicRecipes[i].resultID]);
                if (item[0].child.GetComponent<Watch>() != null)
                {
                    item[0].child.GetComponent<Watch>().ResetComponents();
                }
                if (item[0].child.GetComponent<WatchComponent>() != null)
                {
                    item[0].child.GetComponent<WatchComponent>().componentBroken = new bool[3];
                }
                item[0].child.broken = false;
                item[0].child.knownState = true;
                item[1] = null;
                item[2] = null;
            }
        }
    }

    //Searches through all recipes to find one that breaks down the item inside 
    private void SearchRecipesBreak()
    {
        GameManager gameManager = GameManager.instance;

        //For making sure an item doesn't get broken down again
        bool hasBeenUsed = false;

        //A loop that goes through all recipes until an item is broken down
        for (int i = 0; (i < gameManager.basicRecipes.Length && !hasBeenUsed); i++)
        {
            //Shearching for the recipe for the item inside the first slot
            if(gameManager.basicRecipes[i].resultID == item[0].child.itemID)
            {
                //Stops the loop from going after this iteration
                hasBeenUsed = true;

                //A temporary variable for storing the original item data
                Activator tempItem = item[0];
                //For ease of use
                Watch watch = tempItem.child.GetComponent<Watch>();
                WatchComponent casing = tempItem.child.GetComponent<WatchComponent>();

                //Is the item a watch?
                if(watch != null)
                {
                    //Fills the first slot with the casing and sets all part states
                    FillSlot(0, gameManager.basicRecipes[i].partID[0], watch.casingBroken);
                    item[0].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[0], watch.componentBroken[1], watch.componentBroken[2] };
                    item[0].child.knownState = true;

                    //Fills the second slot with the mechanism, sets all part states and data
                    FillSlot(1, gameManager.basicRecipes[i].partID[1], watch.mechanismBroken);
                    item[1].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[3], watch.componentBroken[4], watch.componentBroken[5] };
                    item[1].child.GetComponent<WatchComponent>().componentID = watch.mechComponentID;
                    item[1].child.GetComponent<WatchComponent>().componentExists = watch.hasMechComponent;
                    item[1].child.knownState = true;

                    //Fills the last slot with the decoration if needed
                    if (gameManager.basicRecipes[i].partID[2] != -1)
                    {
                        FillSlot(2, gameManager.basicRecipes[i].partID[2], watch.componentBroken[6]);
                    }
                    else
                    {
                        item[2] = null;
                    }
                }
                //Is the item a casing?
                else if(casing != null)
                {
                    //Fills all slots with casing components
                    for(int j = 0; j < 3; j++)
                    {
                        if(gameManager.basicRecipes[i].partID[j] != -1)
                        {
                            FillSlot(j, gameManager.basicRecipes[i].partID[j], casing.componentBroken[j]);
                        }
                        else
                        {
                            item[j] = null;
                        }
                    }                  
                }
            }
        }
    }

    //Searching for nearby players
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
