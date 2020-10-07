using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Broken,Unfixable,Repaired,UnknownState};

[CreateAssetMenu(fileName = "New Item",menuName = "Item")]
public class Item : ScriptableObject
{
    private ItemState state = ItemState.UnknownState;
    private Action<Item> itemStateChangeCb;
    public int itemID;
    public Sprite itemImage;
    public List<Item> components = new List<Item>();
    public List<ItemState> componentsStates = new List<ItemState>();
    public ItemState State
    {
        get => state;
        set
        {
            var oldState = state;
            state = value;
            if (itemStateChangeCb != null && oldState != state) itemStateChangeCb(this);
        }
    }
    public void SetParameters(Item templateitem)
    {
        this.state = templateitem.state;
        this.components = templateitem.components;
        this.itemID = templateitem.itemID;
        this.itemImage = templateitem.itemImage;
    }
    public void RegisterItemCallback(Action<Item> callback)
    {
        itemStateChangeCb += callback;
    }
    public void UnRegisterItemCallback(Action<Item> callback)
    {
        if (itemStateChangeCb != null) itemStateChangeCb -= callback;
    }
    //Images: item and state icons
    // public SpriteRenderer stateSprite;
    // protected SpriteRenderer itemSpriteRenderer;
    //Item ID
    
    

    
    // //State booleans
    // public bool broken;
    // public bool unfixable;
    //public bool knownState;

    //For player interactions
   // public bool playerInRange;
    //public Player[] interactingPlayer;


    //Components:
    //Dropped item activator
    //public Activator activator;

    // Start is called before the first frame update
    // void Start()
    // {
    //
    //     //Getting components
    //     itemSpriteRenderer = GetComponent<SpriteRenderer>();
    //     itemImage = itemSpriteRenderer.sprite;
    // }

    // Update is called once per frame
    // void Update()
    // {        
    //     //State sprites
    //     if (!knownState) stateSprite.sprite = GameManager.instance.unknownImage;
    //     else if (unfixable) stateSprite.sprite = GameManager.instance.unfixableImage;
    //     else if (broken) stateSprite.sprite = GameManager.instance.brokenImage;
    //     else stateSprite.sprite = GameManager.instance.repairedImage;
    //
    //     //This used to be in OnTriggerStay2D, but Unity hates us
    //     //Picking up items
    //     if (playerInRange)
    //     {
    //         //PickUp(0);
    //
    //        // PickUp(1);
    //     }
    //
    //     if(isSelected)
    //     {
    //         itemSpriteRenderer.sortingOrder = 2;
    //         itemSpriteRenderer.color = Color.black;
    //     }
    //     else
    //     {
    //         itemSpriteRenderer.sortingOrder = 1;
    //         itemSpriteRenderer.color = Color.white;
    //     }
    // }
    //
    // public void PickUp(int playerID)
    // {
    //     //If the player inputs the pickup button, the specified player's PickupItem method is called and the item is disactivated
    //     if (interactingPlayer[playerID] != null)
    //     {
    //         if (Input.GetButton("Pickup" + interactingPlayer[playerID].playerNumber) && !interactingPlayer[playerID].carriesItem && interactingPlayer[playerID].freeToPickup)
    //         {
    //             interactingPlayer[playerID].PickupItem(itemImage, stateSprite.sprite, activator);
    //             gameObject.SetActive(false);
    //         }
    //     }
    // }
    //
    // //Checking for players in range
    // private void OnTriggerEnter2D(Collider2D collision)
    // {
    //     playerInRange = true;
    //     if (interactingPlayer[0] == null)
    //     {
    //         interactingPlayer[0] = collision.GetComponent<Player>();
    //         interactingPlayer[0].itemsInRange = true;
    //     }
    //     else
    //     {
    //         interactingPlayer[1] = collision.GetComponent<Player>();
    //     }
    // }
    //
    // private void OnTriggerExit2D(Collider2D collision)
    // {
    //     playerInRange = false;
    //     isSelected = false;
    //     if(interactingPlayer[0] == collision.GetComponent<Player>())
    //     {
    //         interactingPlayer[0].itemsInRange = false;
    //         interactingPlayer[0] = null;
    //     }
    //     else
    //     {
    //         interactingPlayer[1] = null;
    //     }
    // }
}
