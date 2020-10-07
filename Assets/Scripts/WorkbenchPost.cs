using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchPost : MonoBehaviour
{
   // public Activator item;
    public bool playerInRange;
    public Player[] interactingPlayer;

    public float timerBase;
    public float timer;
    public SpriteRenderer itemSlot;

    public Slider timerDisplay;

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
            if (!timerDisplay.gameObject.activeInHierarchy)
            {
                timerDisplay.gameObject.SetActive(true);
            }
            float temp = (timerBase - timer) / timerBase;
            timerDisplay.value = temp;

            timer -= Time.deltaTime;
        }
        if (timer <= 0 && timer > -1)
        {
            timerDisplay.gameObject.SetActive(false);
            DropItems();
        }

        if (interactingPlayer[0] != null)
        {
            UsePost(0);
        }
        if (interactingPlayer[1] != null)
        {
            UsePost(1);
        }
    }

    private void UsePost(int playerID)
    {
        //TODO Examining a watch for a list of components;
        if (interactingPlayer[playerID] != null && timer <= -1)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber)
                && interactingPlayer[playerID].carriesItem
                && interactingPlayer[playerID].freeToPickup)
            {
                //Is the item valid for examination
                // if (interactingPlayer[playerID].droppedItemActivator.child.knownState 
                //     && interactingPlayer[playerID].droppedItemActivator.child.unfixable
                //     && interactingPlayer[playerID].droppedItemActivator.child.itemID > 10)
                // {
                //     timer = timerBase;
                //     item = interactingPlayer[playerID].droppedItemActivator;
                //     //itemSlot.sprite = null;
                //
                //     interactingPlayer[playerID].ClearItem();
                //
                //     item.child.broken = false;
                //     item.child.unfixable = false;
                // }
            }
        }
    }

    private void DropItems()
    {
        // if (item != null)
        // {
        //     Vector3 direction = Vector3.zero;
        //     direction = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(1, 3), 0);
        //     item.transform.position = transform.position + direction;
        //     item.SetChildState(true);
        //     item = null;
        // }
        //itemSlot.sprite = null;
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
