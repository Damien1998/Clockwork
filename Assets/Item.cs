using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class Item : MonoBehaviour
{
    //Images: item and state icons
    public Sprite itemImage;    
    public SpriteRenderer stateSprite;

    //Item ID
    public int itemID;

    //State booleans
    public bool broken;
    public bool unfixable;
    public bool knownState;

    //For player interactions
    public bool playerInRange;
    public Player[] interactingPlayer;

    //Components:
    //Dropped item activator
    public Activator activator;

    // Start is called before the first frame update
    void Start()
    {
        //Multiplayer setup
        //The game needs to know which players are nearby
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;

        //Getting components
        itemImage = GetComponent<SpriteRenderer>().sprite;
        activator = GetComponentInParent<Activator>();
    }

    // Update is called once per frame
    void Update()
    {        
        //State sprites
        if (!knownState) stateSprite.sprite = GameManager.instance.unknownImage;
        else if (unfixable) stateSprite.sprite = GameManager.instance.unfixableImage;
        else if (broken) stateSprite.sprite = GameManager.instance.brokenImage;
        else stateSprite.sprite = GameManager.instance.repairedImage;

        //This used to be in OnTriggerStay2D, but Unity hates us
        //Picking up items
        if (playerInRange)
        {
            PickUp(0);

            PickUp(1);
        }
    }

    public void PickUp(int playerID)
    {
        //If the player inputs the pickup button, the specified player's PickupItem method is called and the item is disactivated
        if (interactingPlayer[playerID] != null)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) && !interactingPlayer[playerID].carriesItem && interactingPlayer[playerID].freeToPickup)
            {
                interactingPlayer[playerID].PickupItem(itemImage, stateSprite.sprite, activator);
                gameObject.SetActive(false);
            }
        }
    }

    //Checking for players in range
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerInRange = true;
        if (interactingPlayer[0] == null)
        {
            interactingPlayer[0] = collision.GetComponent<Player>();
        }
        else
        {
            interactingPlayer[1] = collision.GetComponent<Player>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerInRange = false;
        if(interactingPlayer[0] == collision.GetComponent<Player>())
        {
            interactingPlayer[0] = null;
        }
        else
        {
            interactingPlayer[1] = null;
        }
    }
}
