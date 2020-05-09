using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class WorkbenchExamine : MonoBehaviour
{
    public Activator item;
    public bool playerInRange;
    public Player[] interactingPlayer;

    public float timerBase;
    public float timer;
    public SpriteRenderer itemSlot;

    // Start is called before the first frame update
    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        if (timer <= 0 && timer > -1)
        {
            DropItems();
        }

        if (interactingPlayer[0] != null)
        {
            UseExamineWorkbench(0);
        }
        if (interactingPlayer[1] != null)
        {
            UseExamineWorkbench(1);
        }
    }

    private void UseExamineWorkbench(int playerID)
    {
        //TODO Examining a watch for a list of components;
        if (interactingPlayer[playerID] != null && timer <= -1)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) 
                && interactingPlayer[playerID].carriesItem 
                && interactingPlayer[playerID].freeToPickup)
            {
                //Is the item valid for examination
                if (!interactingPlayer[playerID].droppedItemActivator.child.knownState
                    && interactingPlayer[playerID].droppedItemActivator.child.itemID > 10)
                {
                    timer = timerBase;
                    item = interactingPlayer[playerID].droppedItemActivator;
                    itemSlot.sprite = item.child.itemImage;

                    interactingPlayer[playerID].ClearItem();

                    item.child.knownState = true;
                    if (item.child.broken && Random.Range(0, 2) == 0)
                    {
                        item.child.unfixable = true;
                    }                                     
                }
            }
        }       
    }

    private void DropItems()
    {
        if (item != null)
        {
            Vector3 direction = Vector3.zero;
            direction = new Vector3(Random.Range(1, 3), Random.Range(0, -1), 0);
            item.transform.position = transform.position + direction;
            item.SetChildState(true);
            item = null;
        }
        itemSlot.sprite = null;
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
