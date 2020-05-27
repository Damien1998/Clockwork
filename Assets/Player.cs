using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class Player : MonoBehaviour
{
    //Public properties
        //Which player controls the character - for co-op
    public int playerNumber;
        //Movement speed modifier
    public float moveSpeed;
        //For checking if the player is in range of a workbench
    public bool isByWorkbench;
        //Currently held item
    public Activator droppedItemActivator;
        //True if the player is holding an item
    public bool carriesItem;
        //Can the player pick up a new item. Mostly used to avoid the player dropping an item and instantly picking it up
    public bool freeToPickup = true;

        //Image of the currently held item
    public SpriteRenderer itemSprite;
        //Its state icon
    public SpriteRenderer itemStateSprite;


    //Private properties


    //Components
    private Animator animator;
    private Rigidbody2D rigidBody;

    //TODO
    //Making as much stuff private. Will need some refactoring of the dreaded workbench code:
    //Making droppedItemActivator private
    //Making itemSprite private
    //Making itemStateSprite private

    // Start is called before the first frame update
    void Start()
    {
        //Initialising - fetching needed components
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //Dropping items on input
        if (!isByWorkbench && carriesItem && freeToPickup && Input.GetButtonDown("Pickup" + playerNumber))
        {
            DropItem();
        }
        if(Input.GetButtonUp("Pickup" + playerNumber))
        {
            freeToPickup = true;
        }

        //Switching animations
        if(carriesItem)
        {
            animator.SetBool("carriesItem", true);
        }
        else
        {
            animator.SetBool("carriesItem", false);
        }
    }

    private void FixedUpdate()
    {
        //Player movement        
        float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
        float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
        Vector2 movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput != Vector2.zero)
        {
            rigidBody.velocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
        }
        else
        {
            rigidBody.velocity = Vector2.zero;
        }
    }

    //Drops the currently held item
    public void DropItem()
    {
        //Places the held item in-world
        droppedItemActivator.transform.position = transform.position;
        droppedItemActivator.SetChildState(true);

        //Resets the state of the player-held item  
        droppedItemActivator = null;
        itemSprite.sprite = null;
        itemStateSprite.sprite = null;
        carriesItem = false;
        freeToPickup = false;
    }

    //Clears the currently held item
    public void ClearItem()
    {
        //Resets the state of the player-held item  
        droppedItemActivator = null;
        itemSprite.sprite = null;
        itemStateSprite.sprite = null;
        carriesItem = false;
        freeToPickup = false;
    }

    //Picks up the specified item
    public void PickupItem(Sprite itemImage, Sprite itemState, Activator itemToPickup)
    {
        //Sets the state of the player-held item
        droppedItemActivator = itemToPickup;
        itemSprite.sprite = itemImage;
        itemStateSprite.sprite = itemState;
        carriesItem = true;
        freeToPickup = false;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
