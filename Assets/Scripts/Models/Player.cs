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
    

    private int itemToPickUpID;
    //private Activator itemToPickUp;

    private bool lockMovement;
    public bool isByWorkbench;
    private Workbench nearbyWorkbench;

    [SerializeField]
    private float dashDuration;

    private Vector2 movementInput;

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
        Collider2D[] nearbyItems;
        if(Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch == null)
        {
            nearbyItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));
            //nearbyItems = nearbyItems.OrderBy(item => item.transform.parent.position.y).ToArray();
            Array.Reverse(nearbyItems);
        
            if(nearbyItems != null && nearbyItems.Length > 0)
            {
                itemToPickUpID = 0;
                PickUpItem(nearbyItems[0].gameObject);
                if (HeldWatch == null)
                {
                    lockMovement = true;
                    if(Input.GetButtonDown("Action" + playerNumber))
                    {
                        itemToPickUpID++;
                        if(itemToPickUpID >= nearbyItems.Length)
                        {
                            itemToPickUpID = 0;
                        }
                    }
                    lockMovement = false;
                }
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
            if (isByWorkbench)
            {
                PlaceItemInWorkbench();
            }
            else
            {
                DropItem();
            }
        }

        if(isByWorkbench && Input.GetButton("Action" + playerNumber))
        {
            nearbyWorkbench.isOperated = true;
        }
        else if(isByWorkbench)
        {
            nearbyWorkbench.isOperated = false;
        }

    

        //Dash - only in 4 directions, quick burst of speed
        //This only handles dash input, velocity is managed in fixedupdate
        //For some reason this has to be here

        float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
        float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput != Vector2.zero && Input.GetButtonDown("Dash") && !isDashing)
        {

            Vector2 xInput = new Vector2(moveX, 0);
            Vector2 yInput = new Vector2(0, moveY);

            Debug.Log("Dash");

            if (xInput.magnitude >= yInput.magnitude)
            {
                dashDirection = xInput.normalized;
            }
            else
            {
                dashDirection = yInput.normalized;
            }
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        //Player movement      
        float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
        float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
        movementInput = new Vector2(moveX, moveY).normalized;      

        if(!lockMovement)
        {          
            //Managing player speed
            if (movementInput != Vector2.zero && !isDashing)
            {
                rigidBody.velocity = new Vector2(movementInput.x * moveSpeed, movementInput.y * moveSpeed);
            }
            else if (isDashing)
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

        /*
        if(Input.GetButton("Dash"))
        {
            dashReleased = false;
        }
        else
        {
            dashReleased = true;
        }
        */
    }

    //This coroutine flags the dash bool as false after the specified duration
    IEnumerator Dash()
    {
        isDashing = true;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    private void PickUpItem(GameObject pickedupItem)
    {
        HeldWatch = pickedupItem;
        HeldWatch.GetComponent<Watch>().isSelected = true;
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

    private void PlaceItemInWorkbench()
    {
        nearbyWorkbench.PlaceItem(HeldWatch.GetComponent<Watch>());
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Workbench"))
        {
            nearbyWorkbench = collision.GetComponent<Workbench>();
            isByWorkbench = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Workbench"))
        {
            if(nearbyWorkbench != null)
            {
                nearbyWorkbench.isOperated = false;
            }
            
            nearbyWorkbench = null;
            isByWorkbench = false;
        }
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), collision.collider);
        }
    }
}
