using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This place is an incomprehensible mess. Abandon all hope, ye who enter here.
public class Item : MonoBehaviour
{
    //State markers
    public Sprite itemImage;
    public Sprite repairedImage;
    public Sprite brokenImage;
    public Sprite unfixableImage;
    public Sprite unknownImage;
    public SpriteRenderer stateSprite;

    //public string itemName;
    public int itemID;
    public bool broken;
    public bool unfixable;
    public bool knownState;

    public bool playerCollided;
    public Player[] interactingPlayer;

    public Activator activator;

    // Start is called before the first frame update
    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;
        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();
    }

    // Update is called once per frame
    void Update()
    {        
        if (!knownState) stateSprite.sprite = unknownImage;
        else if (unfixable) stateSprite.sprite = unfixableImage;
        else if (broken) stateSprite.sprite = brokenImage;
        else stateSprite.sprite = repairedImage;

        //This used to be in OnTriggerStay2D, but Unity hates us
        if (playerCollided)
        {
            if(interactingPlayer[0] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[0].playerNumber) && !interactingPlayer[0].carriesItem && interactingPlayer[0].freeToPickup)
                {
                    interactingPlayer[0].PickupItem(itemImage, stateSprite.sprite, activator);
                    gameObject.SetActive(false);
                }
            }
            if(interactingPlayer[1] != null)
            {
                if (Input.GetButton("Pickup" + interactingPlayer[1].playerNumber) && !interactingPlayer[1].carriesItem && interactingPlayer[1].freeToPickup)
                {
                    interactingPlayer[1].PickupItem(itemImage, stateSprite.sprite, activator);
                    gameObject.SetActive(false);
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
        if(interactingPlayer[0] == collision.GetComponent<Player>())
        {
            interactingPlayer[0] = null;
        }
        else
        {
            interactingPlayer[1] = null;
        }
        Debug.Log("Item odkolidowuje");
    }

    private void OnTriggerStay2D(Collider2D other)
    {   //ITEM WCHODZI W GRACZA
        //Player player = other.GetComponent<Player>();
        
        
    }

}
