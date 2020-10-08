using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Refactoring is done! You may enter safely
public class Player : MonoBehaviour
{
    //Public properties
        //Position where item should be after picking it up
    public Transform ItemPosition;
        //Which player controls the character - for co-op
    public int playerNumber;
        //Movement speed modifier
    public float moveSpeed, dashSpeed;


    //Private properties
        //Currently held item
    private GameObject HeldWatch;
        //Running bool
    private bool isDashing;
        //Direction of the dash
    private Vector2 dashDirection;
        //For repeating dashes etc
    private bool dashReleased;

    [SerializeField]
    private float pickupRange;

    private Collider2D[] nearbyItems;
    

    private int itemToPickUpID;
    //private Activator itemToPickUp;

    private bool lockMovement;
    public bool isByWorkbench;

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

    void PickUp()
    {
        //Reworked item pick up mechanic
        if(Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch == null)
        {
            nearbyItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));
            //nearbyItems = nearbyItems.OrderBy(item => item.transform.parent.position.y).ToArray();
            Array.Reverse(nearbyItems);
        
            if(nearbyItems != null && nearbyItems.Length > 0)
            {
                itemToPickUpID = 0;
                nearbyItems[0].GetComponent<Watch>().isSelected = true;
                PickUpItem(nearbyItems[0].gameObject);
            }         
        } 
        if(Input.GetButton("Pickup" + playerNumber) && HeldWatch == null && nearbyItems.Length > 0)
        {
            lockMovement = true;
        
            if(Input.GetButtonDown("Action" + playerNumber))
            {
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = false;
                itemToPickUpID++;
                if(itemToPickUpID >= nearbyItems.Length)
                {
                    itemToPickUpID = 0;
                }
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }           
        }
        if(Input.GetButtonUp("Pickup" + playerNumber))
        {
            lockMovement = false;
            if(nearbyItems != null && nearbyItems.Length > 0 && HeldWatch == null)
            {
                for(int i = 0; i < nearbyItems.Length; i++)
                {
                    nearbyItems[i].GetComponent<Watch>().isSelected = false;
                }
                var item = nearbyItems[itemToPickUpID].GetComponent<Watch>().WatchItem;
                //PickupItem(item.itemImage, HeldWatch.stateRenderer.sprite, nearbyItems[itemToPickUpID].GetComponent<Watch>());
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch == null)
        {
            PickUp();
        }
        else if(Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch != null)
        {
            DropItem();
        }

    }

    private void FixedUpdate()
    {
        //Player movement        
        float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
        float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
        Vector2 movementInput = new Vector2(moveX, moveY).normalized;

        if(!lockMovement)
        {
            if (movementInput != Vector2.zero && Input.GetButton("Dash") && !isDashing && dashReleased)
            {
                isDashing = true;
                Vector2 xInput = new Vector2(moveX, 0);
                Vector2 yInput = new Vector2(0, moveY);

                Debug.Log(xInput.magnitude + " " + yInput.magnitude);

                if (xInput.magnitude >= yInput.magnitude)
                {
                    dashDirection = xInput.normalized;
                }
                else
                {
                    dashDirection = yInput.normalized;
                }
            }
            else if (!Input.GetButton("Dash"))
            {
                isDashing = false;
            }

            if (movementInput != Vector2.zero && !isDashing)
            {
                rigidBody.velocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
            }
            else if (movementInput != Vector2.zero)
            {
                rigidBody.velocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);
            }
            else
            {
                rigidBody.velocity = Vector2.zero;
            }
        }     
        else
        {
            rigidBody.velocity = Vector2.zero;
        }

        if(Input.GetButton("Dash"))
        {
            dashReleased = false;
        }
        else
        {
            dashReleased = true;
        }
    }

    private void PickUpItem(GameObject pickedupItem)
    {
        HeldWatch = pickedupItem;
        pickedupItem.transform.position = ItemPosition.position;
        pickedupItem.transform.SetParent(transform);
        animator.SetBool("carriesItem", true);
    }

    private void DropItem()
    {
        HeldWatch.transform.position = transform.position;
        HeldWatch.GetComponent<Watch>().isSelected = false;
        HeldWatch.transform.SetParent(null); 
        HeldWatch = null;
        animator.SetBool("carriesItem", false);
    }
    //Drops the currently held item
    // public void DropItem()
    // {
    //     //Places the held item in-world
    //     HeldWatch.transform.position = transform.position;
    //     HeldWatch.isSelected = false;
    //
    //     //Resets the state of the player-held item  
    //     //itemToPickUp = null;
    //     HeldWatch = null;
    //     itemSprite.sprite = null;
    //     itemStateSprite.sprite = null;
    //     carriesItem = false;
    //     freeToPickup = false;
    // }

    //Clears the currently held item
    // public void ClearItem()
    // {
    //     //Resets the state of the player-held item  
    //     HeldWatch = null;
    //     itemSprite.sprite = null;
    //     itemStateSprite.sprite = null;
    //     carriesItem = false;
    //     freeToPickup = false;
    // }
    //
    // //Picks up the specified item
    // public void PickupItem(Sprite itemImage, Sprite itemState, Watch itemToPickup)
    // {
    //     //Sets the state of the player-held item
    //     HeldWatch = itemToPickup;
    //     itemSprite.sprite = itemImage;
    //     itemStateSprite.sprite = itemState;
    //     carriesItem = true;
    //     freeToPickup = false;
    // }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
