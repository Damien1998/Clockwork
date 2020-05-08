using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorkbenchPrecise : MonoBehaviour
{
    public Activator[] item;
    public bool playerInRange;
    public Player[] interactingPlayer;

    public float timerBase;
    public float timer;
    private int itemSlotsNumber = 4;
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
            PreciseWorkbenchInput(0);
            PreciseWorkbenchOutput(0);
        }
        if (interactingPlayer[1] != null)
        {
            PreciseWorkbenchInput(1);
            PreciseWorkbenchOutput(1);
        }
    }

    private void PreciseWorkbenchInput(int playerID)
    {
        if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber)
            && interactingPlayer[playerID].carriesItem
            && interactingPlayer[playerID].freeToPickup)
        {
            timer = timerBase;

            //Is the item valid for precise workbench
            //1. Is there nothing on the table and the item is a broken component or an empty mechanism
            if (item[playerID] == null
                && ((interactingPlayer[playerID].droppedItemActivator.child.broken && !interactingPlayer[playerID].droppedItemActivator.child.unfixable && interactingPlayer[playerID].droppedItemActivator.child.itemID > 9)
                || (interactingPlayer[playerID].droppedItemActivator.child.itemID == 10 && interactingPlayer[playerID].droppedItemActivator.child.GetComponent<WatchComponent>().isEmpty)))
            {
                Debug.Log("Item Placed");
                item[0] = interactingPlayer[playerID].droppedItemActivator;
                itemSlots[0].sprite = item[0].child.itemImage;
                interactingPlayer[playerID].ClearItem();
                timer = timerBase;
            }

            //2. Is there an empty mechanism and is the item a repaired bit
            else if (item[0] != null && item[0].child.itemID == 10
                && item[0].child.GetComponent<WatchComponent>().isEmpty
                && interactingPlayer[playerID].droppedItemActivator.child.itemID >= 26
                && !interactingPlayer[playerID].droppedItemActivator.child.broken)
            {
                for (int i = 0; i < itemSlotsNumber; i++)
                {
                    //if the slot is empty place the item
                    if (item[i] == null)
                    {
                        Debug.Log("Item Placed2");
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

    private void PreciseWorkbenchOutput(int playerID)
    {
        if (item[0] != null)
        {
            //Managing timers
            if (Input.GetButton("Action" + interactingPlayer[0].playerNumber))
            {
                timer -= Time.deltaTime;
            }
            if (Input.GetButtonUp("Action" + interactingPlayer[0].playerNumber))
            {
                timer = timerBase;
            }

            //if the timer is almost up
            if (timer <= 0.3)
            {
                SortItems();

                //Is the item a broken mechanism
                if (item[0].child.itemID == 10 && item[0].child.broken)
                {
                    WatchComponent mech = item[0].child.GetComponent<WatchComponent>();
                    for (int i = 0; i < 3; i++)
                    {
                        if (mech.componentExists[i])
                        {
                            FillSlot(i + 1, mech.componentID[i], mech.componentBroken[i]);
                        }
                    }
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
                                    Debug.Log(item[j + 1].child.itemID + mech.componentID[i]);
                                    counter++;
                                }
                            }
                        }
                    }
                    if (counter >= mech.numberOfComponents)
                    {
                        FillSlot(0, 10, false);
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
        if (lastSlot <= itemSlotsNumber)
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
                direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-1, -3), 0);
                item[i].transform.position = transform.position + direction;
                item[i].SetChildState(true);
                item[i] = null;
            }
            itemSlots[i].sprite = null;
        }
        timer = -1;
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
