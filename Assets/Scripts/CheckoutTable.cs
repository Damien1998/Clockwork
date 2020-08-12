using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class CheckoutTable : MonoBehaviour
{
    public Activator watch;
    public SpriteRenderer watchSlot;
    public bool playerCollided;
    public Player[] interactingPlayer;
    public bool pickedUp;
    public int watchID = -1;

    //Abandoned functionality
    //public ComponentList watchComponentList;

    // Start is called before the first frame update
    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        GenerateNewWatch();
    }

    // Update is called once per frame
    void Update()
    {
        //Generate watch
        /**
        if (watch == null && watchID == -1)
        {
            GenerateNewWatch();
        }
        **/
        if (interactingPlayer[0] != null)
        {
            UseCheckoutTable(0);           
        }
        if (interactingPlayer[1] != null)
        {
            UseCheckoutTable(1);
        }

        if(!watch.child.gameObject.activeInHierarchy)
        {
            watchSlot.sprite = watch.child.itemImage;
            watch.SetChildState(false);
            pickedUp = true;
        }
    }

    private void UseCheckoutTable(int playerID)
    {
        if (!pickedUp && watch != null 
            && Input.GetButtonUp("Pickup" + interactingPlayer[playerID].playerNumber) 
            && !interactingPlayer[playerID].carriesItem 
            && interactingPlayer[playerID].freeToPickup)
        {
            interactingPlayer[playerID].PickupItem(watch.child.itemImage, watch.child.stateSprite.sprite, watch);           
            watchSlot.sprite = watch.child.itemImage;
            watch.SetChildState(false);
            pickedUp = true;
        }

        //Return fixed watch
        if (pickedUp && watch != null 
            && Input.GetButtonUp("Pickup" + interactingPlayer[playerID].playerNumber) 
            && interactingPlayer[playerID].carriesItem)
        {
            Debug.Log("Oddawanko");
            if (interactingPlayer[playerID].droppedItemActivator.child.itemID == watchID 
                && !interactingPlayer[playerID].droppedItemActivator.child.broken)
            {
                interactingPlayer[playerID].ClearItem();
                watchSlot.sprite = null;
                GenerateNewWatch();
                GameManager.instance.AddPoints(1);
            }
        }
    }

    public void GenerateNewWatch()
    {
        Debug.Log("Generowanko");
        watchID = Random.Range(0, 5);
        watch = Instantiate(GameManager.instance.items[watchID], transform.position, transform.rotation);
        watchSlot.sprite = watch.child.itemImage;
        pickedUp = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerCollided = true;
        if (interactingPlayer[0] == null)
        {
            interactingPlayer[0] = collision.GetComponent<Player>();
        }
        else
        {
            interactingPlayer[1] = collision.GetComponent<Player>();
        }
        Debug.Log("Item koliduje z graczem");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerCollided = false;
        if (interactingPlayer[0] == collision.GetComponent<Player>())
        {
            interactingPlayer[0] = null;
        }
        else
        {
            interactingPlayer[1] = null;
        }
        //watchComponentList.Deactivate();
        Debug.Log("Item odkolidowuje");
    }
}
