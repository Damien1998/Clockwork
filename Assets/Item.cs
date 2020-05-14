using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class Item : MonoBehaviour
{
    public Sprite itemImage;    
    public SpriteRenderer stateSprite;

    public int itemID;
    public bool broken;
    public bool unfixable;
    public bool knownState;

    public bool playerInRange;
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
        if (!knownState) stateSprite.sprite = GameManager.instance.unknownImage;
        else if (unfixable) stateSprite.sprite = GameManager.instance.unfixableImage;
        else if (broken) stateSprite.sprite = GameManager.instance.brokenImage;
        else stateSprite.sprite = GameManager.instance.repairedImage;

        //This used to be in OnTriggerStay2D, but Unity hates us
        if (playerInRange)
        {
            PickUp(0);

            PickUp(1);
        }
    }

    public void PickUp(int playerID)
    {
        if (interactingPlayer[playerID] != null)
        {
            if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) && !interactingPlayer[playerID].carriesItem && interactingPlayer[playerID].freeToPickup)
            {
                interactingPlayer[playerID].PickupItem(itemImage, stateSprite.sprite, activator);
                gameObject.SetActive(false);
            }
        }
    }

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
