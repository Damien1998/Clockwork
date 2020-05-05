using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutTable : MonoBehaviour
{
    public Activator watch;
    public SpriteRenderer watchSlot;
    public bool playerCollided;
    public Player[] interactingPlayer;
    public bool pickedUp;
    public int watchID;
    public ComponentList cl;

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
        //Generate watch
        if (watch == null && Input.GetKeyDown("return"))
        {
            Debug.Log("fdsfs");
            watchID = Random.Range(0, 5);
            watch = Instantiate(GameManager.instance.items[watchID], transform.position, transform.rotation);
            //watch.SetChildState(false);
            pickedUp = false;
        }

        if (interactingPlayer[0] != null)
        {
            
            //Pickup watch
            if(!pickedUp && watch != null && Input.GetButtonDown("Pickup" + interactingPlayer[0].playerNumber) && !interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup)
            {
                interactingPlayer[0].droppedItemActivator = watch;
                interactingPlayer[0].carriesItem = true;
                interactingPlayer[0].freeToPickup = false;
                interactingPlayer[0].itemSprite.sprite = watch.child.itemImage;
                interactingPlayer[0].itemStateSprite.sprite = watch.child.stateSprite.sprite;
                watchSlot.sprite = watch.child.itemImage;
                watch.SetChildState(false);
                pickedUp = true;
            }
            //See watch checklist
            if (watch != null && Input.GetButtonDown("Action" + interactingPlayer[0].playerNumber) && cl.transform.position != new Vector3(cl.pos.x, cl.pos.y))
            {
                Debug.Log("hfugufgauf");
                cl.watch = watch.child.GetComponent<Watch>();
                cl.Activate();
            }
            if (watch != null && Input.GetButtonDown("Action" + interactingPlayer[0].playerNumber) && cl.transform.position == new Vector3(cl.pos.x, cl.pos.y))
            {
                cl.Deactivate();
            }

            //Return fixed watch
            if (pickedUp && watch != null && Input.GetButtonDown("Pickup" + interactingPlayer[0].playerNumber) && interactingPlayer[0].carriesItem)
            {
                if(interactingPlayer[0].droppedItemActivator.child.itemID == watchID && !interactingPlayer[0].droppedItemActivator.child.broken)
                {
                    interactingPlayer[0].droppedItemActivator = null;
                    interactingPlayer[0].itemSprite.sprite = null;
                    interactingPlayer[0].itemStateSprite.sprite = null;
                    interactingPlayer[0].carriesItem = false;
                    interactingPlayer[0].freeToPickup = false;
                    watchSlot.sprite = null;
                    watch = null;
                }
            }
        }
        if (interactingPlayer[1] != null)
        {

            //Pickup watch
            if (!pickedUp && watch != null && Input.GetButtonDown("Pickup" + interactingPlayer[1].playerNumber) && !interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup)
            {
                interactingPlayer[1].droppedItemActivator = watch;
                interactingPlayer[1].carriesItem = true;
                interactingPlayer[1].freeToPickup = false;
                interactingPlayer[1].itemSprite.sprite = watch.child.itemImage;
                interactingPlayer[1].itemStateSprite.sprite = watch.child.stateSprite.sprite;
                watchSlot.sprite = watch.child.itemImage;
                watch.SetChildState(false);
                pickedUp = true;
            }
            //See watch checklist

            //Return fixed watch
            if (pickedUp && watch != null && Input.GetButtonDown("Pickup" + interactingPlayer[1].playerNumber) && interactingPlayer[1].carriesItem)
            {
                if (interactingPlayer[1].droppedItemActivator.child.itemID == watchID && !interactingPlayer[1].droppedItemActivator.child.broken)
                {
                    interactingPlayer[1].droppedItemActivator = null;
                    interactingPlayer[1].itemSprite.sprite = null;
                    interactingPlayer[1].itemStateSprite.sprite = null;
                    interactingPlayer[1].carriesItem = false;
                    interactingPlayer[1].freeToPickup = false;
                    watchSlot.sprite = null;
                    watch = null;
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
        cl.Deactivate();
        Debug.Log("Item odkolidowuje");
    }
}
