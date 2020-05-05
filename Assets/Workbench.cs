using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Workbench : MonoBehaviour
{
    public int type;
    public Activator[] item;
    public bool playerCollided;
    public Player[] interactingPlayer;

    public float timerBaseShort, timerBaseLong;
    public float timer;
    public int itemSlotsNumber;

    public SpriteRenderer[] itemSlots;

    // Start is called before the first frame update
    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        if (type == 0)
        {
            itemSlotsNumber = 3;
        }
        else if(type == 1)
        {
            itemSlotsNumber = 4;
        }
        else
        {
            itemSlotsNumber = 1;
        }
        item = new Activator[itemSlotsNumber];

    }

    // Update is called once per frame
    void Update()
    {
        if(timer > 0 && (type == 2 || type == 3))
        {
            timer -= Time.deltaTime;
        }
        if(timer <= 0 && timer > -1)
        {
            DropItems();
        }

        if (type == 0)
        {
            UseBasicWorkbench();
        }
        if (type == 1)
        {
            UsePreciseWorkbench();
        }
        if (type == 2) 
        {
            UseExamineWorkbench();          
        }
        if (type == 3)
        {
            UsePostalWorkbench();
        }

    }

    private void DropItems()
    {
        for (int i = 0; i < itemSlotsNumber; i++)
        {
            if(item[i] != null)
            {
                Vector3 direction = Vector3.zero;
                switch(type)
                {
                    case 0:
                        direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), 0);
                        break;
                    case 1:
                        direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-1, -3), 0);
                        break;
                    case 2:
                        direction = new Vector3(Random.Range(1, 3), Random.Range(0, -1), 0);
                        break;
                    case 3:
                        direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), 0);
                        break;
                }
                item[i].transform.position = transform.position + direction;
                item[i].SetChildState(true);
                item[i] = null;
            }
            itemSlots[i].sprite = null;
        }
        timer = -1;
    }

    //Basic workbench functionality
    private void UseBasicWorkbench()
    {
        if (interactingPlayer[0] != null)
        {           
            //Input
            if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup && interactingPlayer[0].droppedItemActivator.child.knownState)
            {
                timer = timerBaseShort;
                //Is the item valid for basic workbench
                //1. Is there nothing on the table and the item is a broken watch or casing
                if (item[0] == null && interactingPlayer[0].droppedItemActivator.child.itemID < 10 && interactingPlayer[0].droppedItemActivator.child.broken && interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    Debug.Log("Item Placed");
                    item[0] = interactingPlayer[0].droppedItemActivator;
                    interactingPlayer[0].droppedItemActivator = null;
                    interactingPlayer[0].itemSprite.sprite = null;
                    interactingPlayer[0].itemStateSprite.sprite = null;
                    interactingPlayer[0].carriesItem = false;
                    interactingPlayer[0].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                    timer = timerBaseShort;
                }
                //2. Is the item a fixed watch or casing component
                else if(!interactingPlayer[0].droppedItemActivator.child.broken && (interactingPlayer[0].droppedItemActivator.child.itemID < 26 || interactingPlayer[0].droppedItemActivator.child.itemID > 4) && !interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    for(int i = 0; i < itemSlotsNumber; i++)
                    {
                        //if the slot is empty place the item
                        if(item[i] == null)
                        {
                            Debug.Log("Item Placed");
                            item[i] = interactingPlayer[0].droppedItemActivator;
                            interactingPlayer[0].droppedItemActivator = null;
                            interactingPlayer[0].itemSprite.sprite = null;
                            interactingPlayer[0].itemStateSprite.sprite = null;
                            interactingPlayer[0].carriesItem = false;
                            interactingPlayer[0].freeToPickup = false;
                            itemSlots[i].sprite = item[i].child.itemImage;
                            timer = timerBaseShort;
                            break;
                        }
                    }
                }
            }
        }
        if (interactingPlayer[1] != null)
        {
            Debug.Log("Player Valid");
            Debug.Log(interactingPlayer[1].playerNumber);
            if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup && interactingPlayer[1].droppedItemActivator.child.knownState)
            {
                timer = timerBaseShort;
                //Is the item valid for basic workbench
                //1. Is there nothing on the table and the item is a broken watch or casing
                if (item[0] == null && interactingPlayer[1].droppedItemActivator.child.itemID < 10 && interactingPlayer[1].droppedItemActivator.child.broken)
                {
                    Debug.Log("Item Placed");
                    item[0] = interactingPlayer[1].droppedItemActivator;
                    interactingPlayer[1].droppedItemActivator = null;
                    interactingPlayer[1].itemSprite.sprite = null;
                    interactingPlayer[1].itemStateSprite.sprite = null;
                    interactingPlayer[1].carriesItem = false;
                    interactingPlayer[1].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                    timer = timerBaseShort;
                }
                //2. Is the item a fixed watch or casing component
                else if (!interactingPlayer[1].droppedItemActivator.child.broken && (interactingPlayer[1].droppedItemActivator.child.itemID < 26 || interactingPlayer[1].droppedItemActivator.child.itemID > 4) && !interactingPlayer[1].droppedItemActivator.child.broken)
                {
                    for (int i = 0; i < itemSlotsNumber; i++)
                    {
                        //if the slot is empty place the item
                        if (item[i] == null)
                        {
                            Debug.Log("Item Placed");
                            item[i] = interactingPlayer[1].droppedItemActivator;
                            interactingPlayer[1].droppedItemActivator = null;
                            interactingPlayer[1].itemSprite.sprite = null;
                            interactingPlayer[1].itemStateSprite.sprite = null;
                            interactingPlayer[1].carriesItem = false;
                            interactingPlayer[1].freeToPickup = false;
                            itemSlots[i].sprite = item[i].child.itemImage;
                            timer = timerBaseShort;
                            break;
                        }
                    }
                }
            }
        }
        //Output
        if (interactingPlayer[0] != null && item[0] != null)
        {
            if(Input.GetButton("Action" + interactingPlayer[0].playerNumber))
            {
                timer -= Time.deltaTime;
            }
            if(Input.GetButtonUp("Action" + interactingPlayer[0].playerNumber))
            {
                timer = timerBaseShort;
            }
            //Is the item to be dissected
            if(item[0].child.broken)
            {
                //if the time is almost up
                if(timer <= 0.5)
                {
                    //Is the item a watch
                    if (item[0].child.GetComponent<Watch>() != null)
                    {                       
                        Watch watch = item[0].child.GetComponent<Watch>();                      
                        item[1] = Instantiate(GameManager.instance.items[watch.componentID[0]]);                       
                        item[1].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[0], watch.componentBroken[1], watch.componentBroken[2] };
                        item[1].child.broken = watch.casingBroken;
                        item[1].SetChildState(false);
                        item[2] = Instantiate(GameManager.instance.items[10]);
                        item[2].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[3], watch.componentBroken[4], watch.componentBroken[5] };
                        item[2].child.GetComponent<WatchComponent>().componentID = watch.mechComponentID;
                        item[2].child.GetComponent<WatchComponent>().componentExists = watch.hasMechComponent;
                        item[2].child.knownState = true;
                        item[1].child.knownState = true;
                        item[2].child.broken = watch.mechanismBroken;
                        item[2].SetChildState(false);
                        if (watch.hasDecor)
                        {
                            item[0] = Instantiate(GameManager.instance.items[watch.componentID[2]]);
                            item[0].child.broken = watch.componentBroken[6];
                            item[0].SetChildState(false);
                        }
                        else item[0] = null;
                        DropItems();
                    }
                    //Is the item a casing
                    if (item[0] != null && item[0].child.GetComponent<WatchComponent>() != null)
                    {
                        WatchComponent casing = item[0].child.GetComponent<WatchComponent>();
                        item[1] = Instantiate(GameManager.instance.items[casing.componentID[1]]);
                        item[1].child.broken = casing.componentBroken[1];
                        item[1].SetChildState(false);
                        item[2] = Instantiate(GameManager.instance.items[casing.componentID[2]]);
                        item[2].child.broken = casing.componentBroken[2];
                        item[2].SetChildState(false);
                        item[0] = Instantiate(GameManager.instance.items[casing.componentID[0]]);
                        item[0].child.broken = casing.componentBroken[0];
                        item[0].SetChildState(false);
                        DropItems();
                    }      
                }               
            }
            //is the item to be combined (isn't broken)
            else if(item[1] != null)
            {
                int outputID = -1;
                if (timer <= 0.5)
                {
                    //Is the first item a watch component
                    if (item[0].child.itemID < 13)
                    {
                        //cheching if all 5 watches can be assembled;
                        if (item[2] == null && ((item[0].child.GetComponent<WatchComponent>() != null && item[0].child.itemID == 10 && !item[0].child.GetComponent<WatchComponent>().isEmpty) || (item[1].child.GetComponent<WatchComponent>() != null && item[1].child.itemID == 10 && !item[1].child.GetComponent<WatchComponent>().isEmpty)))
                        {
                            
                            //Assemble silver watch
                            if (item[0].child.itemID == 5 || item[1].child.itemID == 5) outputID = 0;
                            //Assemble brass watch
                            else if (item[0].child.itemID == 7 || item[1].child.itemID == 7) outputID = 2;
                            //Assemble black watch
                            else if (item[0].child.itemID == 9 || item[1].child.itemID == 9) outputID = 4;
                        }
                        else if(item[2] != null)
                        {
                            //Sorting
                            item = item.OrderBy(it => it.child.itemID).ToArray();
                            //Assemble golden watch
                            if (item[0].child.itemID == 6 && item[1].child.itemID == 10 && item[2].child.itemID == 11) outputID = 1;
                            //Assemble blue watch
                            if (item[0].child.itemID == 8 && item[1].child.itemID == 10 && item[2].child.itemID == 12) outputID = 3;
                        }
                    }
                    //are all slots filled with casing components
                    else if (item[2] != null)
                    {
                        //Sorting
                        item = item.OrderBy(it => it.child.itemID).ToArray();
                        if (item[0].child.itemID == 13 && item[1].child.itemID == 14 && item[2].child.itemID == 17) outputID = 5;
                        else if (item[0].child.itemID == 15 && item[1].child.itemID == 16 && item[2].child.itemID == 17) outputID = 6;
                        else if (item[0].child.itemID == 18 && item[1].child.itemID == 19 && item[2].child.itemID == 20) outputID = 7;
                        else if (item[0].child.itemID == 21 && item[1].child.itemID == 22 && item[2].child.itemID == 24) outputID = 8;
                        else if (item[0].child.itemID == 23 && item[1].child.itemID == 24 && item[2].child.itemID == 25) outputID = 9;
                    }
                    else DropItems();
                    if (outputID != -1)
                    {
                        item[0] = Instantiate(GameManager.instance.items[outputID]);
                        if(item[0].child.GetComponent<Watch>() != null)
                        {
                            item[0].child.GetComponent<Watch>().ResetComponents();
                        }
                        if(item[0].child.GetComponent<WatchComponent>() != null)
                        {
                            item[0].child.GetComponent<WatchComponent>().componentBroken = new bool[3];
                        }
                        item[0].child.broken = false;
                        item[0].child.knownState = true;
                        item[1] = null;
                        item[2] = null;
                        DropItems();
                    }
                } 
            }
        }
        if (interactingPlayer[1] != null && item[0] != null)
        {
            if (Input.GetButton("Action" + interactingPlayer[1].playerNumber))
            {
                timer -= Time.deltaTime;
            }
            if (Input.GetButtonUp("Action" + interactingPlayer[1].playerNumber))
            {
                timer = timerBaseShort;
            }
            //Is the item to be dissected
            if (item[0].child.broken)
            {
                //if the time is almost up
                if (timer <= 0.5)
                {
                    //Is the item a watch
                    if (item[0].child.GetComponent<Watch>() != null)
                    {
                        Watch watch = item[0].child.GetComponent<Watch>();
                        item[1] = Instantiate(GameManager.instance.items[watch.componentID[0]]);
                        item[1].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[0], watch.componentBroken[1], watch.componentBroken[2] };
                        item[1].child.broken = watch.casingBroken;
                        item[1].SetChildState(false);
                        item[2] = Instantiate(GameManager.instance.items[10]);
                        item[2].child.GetComponent<WatchComponent>().componentBroken = new bool[] { watch.componentBroken[3], watch.componentBroken[4], watch.componentBroken[5] };
                        item[2].child.GetComponent<WatchComponent>().componentID = watch.mechComponentID;
                        item[2].child.GetComponent<WatchComponent>().componentExists = watch.hasMechComponent;
                        item[2].child.knownState = true;
                        item[1].child.knownState = true;
                        item[2].child.broken = watch.mechanismBroken;
                        item[2].SetChildState(false);
                        if (watch.hasDecor)
                        {
                            item[0] = Instantiate(GameManager.instance.items[watch.componentID[2]]);
                            item[0].child.broken = watch.componentBroken[6];
                            item[0].SetChildState(false);
                        }
                        else item[0] = null;
                        DropItems();
                    }
                    //Is the item a casing
                    if (item[0] != null && item[0].child.GetComponent<WatchComponent>() != null)
                    {
                        WatchComponent casing = item[0].child.GetComponent<WatchComponent>();
                        item[1] = Instantiate(GameManager.instance.items[casing.componentID[1]]);
                        item[1].child.broken = casing.componentBroken[1];
                        item[1].SetChildState(false);
                        item[2] = Instantiate(GameManager.instance.items[casing.componentID[2]]);
                        item[2].child.broken = casing.componentBroken[2];
                        item[2].SetChildState(false);
                        item[0] = Instantiate(GameManager.instance.items[casing.componentID[0]]);
                        item[0].child.broken = casing.componentBroken[0];
                        item[0].SetChildState(false);
                        DropItems();
                    }
                }
            }
            //is the item to be combined (isn't broken)
            else if (item[1] != null)
            {
                int outputID = -1;
                if (timer <= 0.5)
                {
                    //Is the first item a watch component
                    if (item[0].child.itemID < 13)
                    {
                        //cheching if all 5 watches can be assembled;
                        if (item[2] == null && ((item[0].child.GetComponent<WatchComponent>() != null && item[0].child.itemID == 10 && !item[0].child.GetComponent<WatchComponent>().isEmpty) || (item[1].child.GetComponent<WatchComponent>() != null && item[1].child.itemID == 10 && !item[1].child.GetComponent<WatchComponent>().isEmpty)))
                        {
                            //Assemble silver watch
                            if (item[0].child.itemID == 5 || item[1].child.itemID == 5) outputID = 0;
                            //Assemble brass watch
                            else if (item[0].child.itemID == 7 || item[1].child.itemID == 7) outputID = 2;
                            //Assemble black watch
                            else if (item[0].child.itemID == 9 || item[1].child.itemID == 9) outputID = 4;
                        }
                        else if(item[2] != null)
                        {
                            //Sorting
                            item = item.OrderBy(it => it.child.itemID).ToArray();
                            //Assemble golden watch
                            if (item[0].child.itemID == 6 && item[1].child.itemID == 10 && item[2].child.itemID == 11) outputID = 1;
                            //Assemble blue watch
                            if (item[0].child.itemID == 8 && item[1].child.itemID == 10 && item[2].child.itemID == 12) outputID = 3;
                        }
                    }
                    //are all slots filled with casing components
                    else if (item[2] != null)
                    {
                        //Sorting
                        item = item.OrderBy(it => it.child.itemID).ToArray();
                        if (item[0].child.itemID == 13 && item[1].child.itemID == 14 && item[2].child.itemID == 17) outputID = 5;
                        else if (item[0].child.itemID == 15 && item[1].child.itemID == 16 && item[2].child.itemID == 17) outputID = 6;
                        else if (item[0].child.itemID == 18 && item[1].child.itemID == 19 && item[2].child.itemID == 20) outputID = 7;
                        else if (item[0].child.itemID == 21 && item[1].child.itemID == 22 && item[2].child.itemID == 24) outputID = 8;
                        else if (item[0].child.itemID == 23 && item[1].child.itemID == 24 && item[2].child.itemID == 25) outputID = 9;
                    }
                    else DropItems();
                    if (outputID != -1)
                    {
                        item[0] = Instantiate(GameManager.instance.items[outputID]);
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
                        DropItems();
                    }
                }
            }
        }
    }

    //Examining workbench functionality
    private void UseExamineWorkbench()
    {
        //TODO Examining a watch for a list of components;
        if (interactingPlayer[0] != null && timer <= -1)
        {
            Debug.Log(interactingPlayer[0].playerNumber);
            if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup && interactingPlayer[0].droppedItemActivator.child.itemID > 10)
            {
                //Is the item valid for examination
                if(!interactingPlayer[0].droppedItemActivator.child.knownState)
                {
                    Debug.Log("Item Placed");
                    timer = timerBaseLong;
                    item[0] = interactingPlayer[0].droppedItemActivator;
                    interactingPlayer[0].droppedItemActivator = null;
                    item[0].child.knownState = true;
                    if (item[0].child.broken && Random.Range(0, 2) == 0) item[0].child.unfixable = true;
                    interactingPlayer[0].itemSprite.sprite = null;
                    interactingPlayer[0].itemStateSprite.sprite = null;
                    interactingPlayer[0].carriesItem = false;
                    interactingPlayer[0].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                }
            }
        }
        if (interactingPlayer[1] != null && timer <= -1)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup && interactingPlayer[1].droppedItemActivator.child.itemID > 10)
            {
                //Is the item valid for examination
                if (!interactingPlayer[1].droppedItemActivator.child.knownState)
                {
                    Debug.Log("Item Placed");
                    timer = timerBaseLong;
                    item[0] = interactingPlayer[1].droppedItemActivator;
                    interactingPlayer[1].droppedItemActivator = null;
                    item[0].child.knownState = true;
                    if (item[0].child.broken && Random.Range(0, 2) == 0) item[0].child.unfixable = true;
                    interactingPlayer[1].itemSprite.sprite = null;
                    interactingPlayer[1].itemStateSprite.sprite = null;
                    interactingPlayer[1].carriesItem = false;
                    interactingPlayer[1].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                }
            }
        }
    }

    //Postal Service Functionality
    public void UsePostalWorkbench()
    {
        if (interactingPlayer[0] != null && timer <= -1)
        {
            Debug.Log(interactingPlayer[0].playerNumber);
            if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup && interactingPlayer[0].droppedItemActivator.child.itemID > 10)
            {
                //Is the item beyond repair
                if (interactingPlayer[0].droppedItemActivator.child.knownState && interactingPlayer[0].droppedItemActivator.child.unfixable)
                {
                    Debug.Log("Item Placed");
                    timer = timerBaseLong;
                    item[0] = interactingPlayer[0].droppedItemActivator;
                    interactingPlayer[0].droppedItemActivator = null;
                    item[0].child.unfixable = false;
                    item[0].child.broken = false;
                    interactingPlayer[0].itemSprite.sprite = null;
                    interactingPlayer[0].itemStateSprite.sprite = null;
                    interactingPlayer[0].carriesItem = false;
                    interactingPlayer[0].freeToPickup = false;
                    itemSlots[0].sprite = null;
                }
            }
        }
        if (interactingPlayer[1] != null && timer <= -1)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup && interactingPlayer[1].droppedItemActivator.child.itemID > 10)
            {
                //Is the item beyond repair
                if (interactingPlayer[1].droppedItemActivator.child.knownState && interactingPlayer[1].droppedItemActivator.child.unfixable)
                {
                    Debug.Log("Item Placed");
                    timer = timerBaseLong;
                    item[0] = interactingPlayer[1].droppedItemActivator;
                    interactingPlayer[1].droppedItemActivator = null;
                    item[0].child.unfixable = false;
                    item[0].child.broken = false;
                    interactingPlayer[1].itemSprite.sprite = null;
                    interactingPlayer[1].itemStateSprite.sprite = null;
                    interactingPlayer[1].carriesItem = false;
                    interactingPlayer[1].freeToPickup = false;
                    itemSlots[0].sprite = null;
                }
            }
        }
    }

    public void SortItems()
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
        for(int i = 0; i < temp.Count; i++)
        {
            item[i] = temp[i];
        }
    }

    //Precise Workbench Functionality
    public void UsePreciseWorkbench()
    {
        if (interactingPlayer[0] != null)
        {
            //Input
            if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup)
            {
                timer = timerBaseShort;
                //Is the item valid for precise workbench
                //1. Is there nothing on the table and the item is a broken component or an empty mechanism
                if (item[0] == null && ((interactingPlayer[0].droppedItemActivator.child.broken && !interactingPlayer[0].droppedItemActivator.child.unfixable && interactingPlayer[0].droppedItemActivator.child.itemID > 9) || (interactingPlayer[0].droppedItemActivator.child.itemID == 10 && interactingPlayer[0].droppedItemActivator.child.GetComponent<WatchComponent>().isEmpty)))
                {
                    Debug.Log("Item Placed");
                    item[0] = interactingPlayer[0].droppedItemActivator;
                    interactingPlayer[0].droppedItemActivator = null;
                    interactingPlayer[0].itemSprite.sprite = null;
                    interactingPlayer[0].itemStateSprite.sprite = null;
                    interactingPlayer[0].carriesItem = false;
                    interactingPlayer[0].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                    timer = timerBaseShort;
                }
                //2. Is there an empty watch and is the item a repaired bit, 
                else if (item[0] != null && item[0].child.itemID == 10 && item[0].child.GetComponent<WatchComponent>().isEmpty && interactingPlayer[0].droppedItemActivator.child.itemID >= 26 && !interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    for (int i = 0; i < itemSlotsNumber; i++)
                    {
                        //if the slot is empty place the item
                        if (item[i] == null)
                        {
                            Debug.Log("Item Placed2");
                            item[i] = interactingPlayer[0].droppedItemActivator;
                            interactingPlayer[0].droppedItemActivator = null;
                            interactingPlayer[0].itemSprite.sprite = null;
                            interactingPlayer[0].itemStateSprite.sprite = null;
                            interactingPlayer[0].carriesItem = false;
                            interactingPlayer[0].freeToPickup = false;
                            itemSlots[i].sprite = item[i].child.itemImage;
                            timer = timerBaseShort;
                            break;
                        }
                    }
                }
            }
            //Output
            if(item[0] != null)
            {
                SortItems();
                if (Input.GetButton("Action" + interactingPlayer[0].playerNumber))
                {
                    timer -= Time.deltaTime;
                }
                if (Input.GetButtonUp("Action" + interactingPlayer[0].playerNumber))
                {
                    timer = timerBaseShort;
                }
                //if the timer is almost up
                if(timer <= 0.5)
                {
                    //Is the item a broken mechanism
                    if(item[0].child.itemID == 10 && item[0].child.broken)
                    {
                        WatchComponent mech = item[0].child.GetComponent<WatchComponent>();
                        for(int i = 0; i < 3; i++)
                        {
                            if(mech.componentExists[i])
                            {
                                item[i + 1] = Instantiate(GameManager.instance.items[mech.componentID[i]]);
                                item[i+1].child.broken = mech.componentBroken[i];
                                item[i+1].SetChildState(false);
                            }
                        }
                        Debug.Log("sgdhafda");
                        item[0].child.GetComponent<WatchComponent>().isEmpty = true;
                        item[0].child.GetComponent<WatchComponent>().broken = false;
                        item[0].child.GetComponent<WatchComponent>().knownState = true;
                    }
                    //Is the item a broken component
                    else if(item[0].child.itemID != 10 && item[0].child.broken)
                    {
                        Debug.Log("Repair!");
                        item[0].child.broken = false;
                    }
                    //Are there enough items to fix an empty mechanism
                    else if(item[0].child.itemID == 10 && !item[0].child.broken)
                    {
                        Debug.Log("Filling!");
                        WatchComponent mech = item[0].child.GetComponent<WatchComponent>();
                        bool[] used = new bool[] {false, false, false};
                        int counter = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (mech.componentExists[i])
                            {
                                //Searching through the slots
                                for (int j = 0; j < 3; j++)
                                {
                                    if(!used[j] && item[j+1] != null && item[j+1].child.itemID == mech.componentID[i])
                                    {
                                        used[j] = true;
                                        Debug.Log(item[j + 1].child.itemID + mech.componentID[i]);
                                        counter++;
                                    }
                                }
                            }
                        }
                        Debug.Log(counter);
                        if (counter >= mech.numberOfComponents)
                        {
                            item[0] = Instantiate(GameManager.instance.items[10]);
                            item[0].child.broken = false;
                            item[0].SetChildState(false);
                            item[0].child.knownState = true;
                            item[1] = null;
                            item[2] = null;
                            item[3] = null;
                        }
                    }
                    DropItems();
                }

            }

        }
        if (interactingPlayer[1] != null)
        {
            //Input
            if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup)
            {
                timer = timerBaseShort;
                //Is the item valid for precise workbench
                //1. Is there nothing on the table and the item is a broken component, a bit or an empty mechanism
                if (item[0] == null && ((interactingPlayer[1].droppedItemActivator.child.broken && !interactingPlayer[1].droppedItemActivator.child.unfixable && interactingPlayer[1].droppedItemActivator.child.itemID > 9) || interactingPlayer[1].droppedItemActivator.child.itemID >= 26 || (interactingPlayer[1].droppedItemActivator.child.itemID == 10 && interactingPlayer[1].droppedItemActivator.child.GetComponent<WatchComponent>().isEmpty)))
                {
                    Debug.Log("Item Placed");
                    item[0] = interactingPlayer[1].droppedItemActivator;
                    interactingPlayer[1].droppedItemActivator = null;
                    interactingPlayer[1].itemSprite.sprite = null;
                    interactingPlayer[1].itemStateSprite.sprite = null;
                    interactingPlayer[1].carriesItem = false;
                    interactingPlayer[1].freeToPickup = false;
                    itemSlots[0].sprite = item[0].child.itemImage;
                    timer = timerBaseShort;
                }
                //2. Is there an empty watch and is the item a repaired bit
                else if (item[0].child.itemID == 10 && item[0].child.GetComponent<WatchComponent>().isEmpty && interactingPlayer[1].droppedItemActivator.child.itemID >= 26 && !interactingPlayer[1].droppedItemActivator.child.broken)
                {
                    for (int i = 0; i < itemSlotsNumber; i++)
                    {
                        //if the slot is empty place the item
                        if (item[i] == null)
                        {
                            Debug.Log("Item Placed2");
                            item[i] = interactingPlayer[1].droppedItemActivator;
                            interactingPlayer[1].droppedItemActivator = null;
                            interactingPlayer[1].itemSprite.sprite = null;
                            interactingPlayer[1].itemStateSprite.sprite = null;
                            interactingPlayer[1].carriesItem = false;
                            interactingPlayer[1].freeToPickup = false;
                            itemSlots[i].sprite = item[i].child.itemImage;
                            timer = timerBaseShort;
                            break;
                        }
                    }
                }
            }
            //Output
            if (item[0] != null)
            {
                SortItems();
                if (Input.GetButton("Action" + interactingPlayer[1].playerNumber))
                {
                    timer -= Time.deltaTime;
                }
                if (Input.GetButtonUp("Action" + interactingPlayer[1].playerNumber))
                {
                    timer = timerBaseShort;
                }
                //if the timer is almost up
                if (timer <= 0.5)
                {
                    //Is the item a broken mechanism
                    if (item[0].child.itemID == 10 && item[0].child.broken)
                    {
                        WatchComponent mech = item[0].child.GetComponent<WatchComponent>();
                        for (int i = 0; i < 3; i++)
                        {
                            if (mech.componentExists[i])
                            {
                                item[i + 1] = Instantiate(GameManager.instance.items[mech.componentID[i]]);
                                item[i + 1].child.broken = mech.componentBroken[i];
                                item[i + 1].SetChildState(false);
                            }
                        }
                        Debug.Log("sgdhafda");
                        item[0].child.GetComponent<WatchComponent>().isEmpty = true;
                        item[0].child.GetComponent<WatchComponent>().broken = false;
                        item[0].child.GetComponent<WatchComponent>().knownState = true;
                    }
                    //Is the item a broken component
                    else if (item[0].child.itemID != 10 && item[0].child.broken)
                    {
                        item[0].child.broken = false;
                    }
                    //Are there enough items to fix an empty mechanism
                    else if (item[0].child.itemID == 10 && !item[0].child.broken)
                    {
                        WatchComponent mech = item[0].child.GetComponent<WatchComponent>();
                        bool[] used = new bool[] { false, false, false };
                        int counter = 0;
                        for (int i = 0; i < 3; i++)
                        {
                            if (mech.componentExists[i])
                            {
                                //Searching through the slots
                                for (int j = 0; j < 3; j++)
                                {
                                    if (!used[j] && item[j + 1] != null && item[j + 1].child.itemID == mech.componentID[i])
                                    {
                                        used[j] = true;
                                        counter++;
                                    }
                                }
                            }
                        }
                        if (counter >= mech.numberOfComponents)
                        {
                            item[0] = Instantiate(GameManager.instance.items[10]);
                            item[0].child.broken = false;
                            item[0].child.knownState = true;
                            item[0].SetChildState(false);
                            item[1] = null;
                            item[2] = null;
                            item[3] = null;
                        }
                    }
                    DropItems();
                }

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerCollided = true;
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
        playerCollided = false;
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
