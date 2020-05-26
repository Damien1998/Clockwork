using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Refactoring is done! You may enter safely
public class WorkbenchBasic : MonoBehaviour
{
    public Activator[] item;
    public bool playerInRange;
    public Player[] interactingPlayer;

    public float timerBase;
    public float timer;
    private int itemSlotsNumber = 3;
    public SpriteRenderer[] itemSlots;

    // Start is called before the first frame update
    void Start()
    {
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
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) 
                && interactingPlayer[playerID].carriesItem 
                && interactingPlayer[playerID].freeToPickup 
                && interactingPlayer[playerID].droppedItemActivator.child.knownState)
            {
                //timer = timerBase;

                //Is the item valid for the basic workbench:

                //1. Is there nothing on the table and the item is a broken watch or casing
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

                //Is the item to be dissected
                if (item[0].child.broken)
                {
                    //if the time is almost up
                    if (timer <= 0.3)
                    {
                        SearchRecipesBreak();
                        DropItems();
                    }
                }
            
                //is the item to be combined (isn't broken)
                else if (item[1] != null)
                {                   
                    if (timer <= 0.3)
                    {
                        SearchRecipesCombine();
                        DropItems();
                    }
                }
            }
        }            
    }

    private void FillSlot(int slotID, int itemID, bool broken)
    {
        item[slotID] = Instantiate(GameManager.instance.items[itemID]);
        item[slotID].child.broken = broken;
        item[slotID].SetChildState(false);
    }

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

    private void SearchRecipesCombine()
    {
        GameManager gameManager = GameManager.instance;

        SortItems();

        for(int i = 0; i < gameManager.basicRecipes.Length; i++)
        {
            int counter = 0;
            for(int j = 0; j < 3; j++)
            {
                if ((item[j] != null && item[j].child.itemID == gameManager.basicRecipes[i].partID[j])
                    ||(item[j] == null && gameManager.basicRecipes[i].partID[j] == -1))
                {
                    counter++;
                }
            }

            if(counter == 3)
            {
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

    private void SearchRecipesBreak()
    {
        GameManager gameManager = GameManager.instance;

        bool hasBeenUsed = false;

        for (int i = 0; (i < gameManager.basicRecipes.Length && !hasBeenUsed); i++)
        {
            if(gameManager.basicRecipes[i].resultID == item[0].child.itemID)
            {
                hasBeenUsed = true;

                Activator tempItem = item[0];
                Watch watch = tempItem.child.GetComponent<Watch>();
                WatchComponent casing = tempItem.child.GetComponent<WatchComponent>();

                if(watch != null)
                {
                    FillSlot(0, gameManager.basicRecipes[i].partID[0], watch.casingBroken);
                    item[0].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[0], watch.componentBroken[1], watch.componentBroken[2] };
                    item[0].child.knownState = true;

                    FillSlot(1, gameManager.basicRecipes[i].partID[1], watch.mechanismBroken);
                    item[1].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[3], watch.componentBroken[4], watch.componentBroken[5] };
                    item[1].child.GetComponent<WatchComponent>().componentID = watch.mechComponentID;
                    item[1].child.GetComponent<WatchComponent>().componentExists = watch.hasMechComponent;
                    item[1].child.knownState = true;

                    if (gameManager.basicRecipes[i].partID[2] != -1)
                    {
                        FillSlot(2, gameManager.basicRecipes[i].partID[2], watch.componentBroken[6]);
                    }
                    else
                    {
                        item[2] = null;
                    }
                }
                else if(casing != null)
                {
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
