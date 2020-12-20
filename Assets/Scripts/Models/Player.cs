﻿using System;
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
    

    private int itemToPickUpID = 0;
    //private Activator itemToPickUp;

    private bool lockMovement;

    //We wanted to handle interaction from the side of the player 
    //having a bool for every type of interactable object in the workshop feels clunky
    //Especially if some more of them will be there
    //Idk what to do
    public bool isByWorkbench;
    public bool isByLevelStart;
    private Workbench nearbyWorkbench;
    private LevelStart nearbyLevelStart;
    

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
    private Collider2D[] nearbyItems = null;
    void PickUp()
    {
        //Reworked item pick up mechanic
        //First checks for input and Locks movement then Searches for items in Range and sorts them accordingly to their position
        if (Input.GetButtonDown("Pickup" + playerNumber) && !MouseBlocker.mouseBlocked)
        {
            lockMovement = true;
            nearbyItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));
            nearbyItems = nearbyItems.OrderBy(item => item.transform.position.y).ToArray();
            Array.Reverse(nearbyItems);
            if (nearbyItems.Length > 0)
            {
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }
        }
        //After the first action if player is performing secondary action ie right click it highlights the item in sorted list
        if (nearbyItems != null && nearbyItems.Length > 0&&Input.GetButtonDown("Action" + playerNumber) && !MouseBlocker.mouseBlocked)
        {         
            if (itemToPickUpID < nearbyItems.Length - 1)
            {
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = false;
                itemToPickUpID++;
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }
            else
            {
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = false;
                itemToPickUpID = 0;
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }
        }
        //At the end checks if player is not holding a button anymore and picks up highlighted item
        if(nearbyItems != null && Input.GetButtonUp("Pickup" + playerNumber) && !MouseBlocker.mouseBlocked)
        {
            if (itemToPickUpID < nearbyItems.Length)
            {
                PickUpItem(nearbyItems[itemToPickUpID].gameObject);
            }
            itemToPickUpID = 0;
            nearbyItems = null;
            lockMovement = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (HeldWatch != null)
        {
            HeldWatch.transform.position = transform.position + ItemPosition.localPosition;
        }
        if (HeldWatch == null)
        {
            PickUp();
        }
        else if(Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch != null && !MouseBlocker.mouseBlocked)
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

        if(isByLevelStart && Input.GetButtonDown("Action" + playerNumber) && !MouseBlocker.mouseBlocked)
        {
            nearbyLevelStart.StartLevel();
            nearbyLevelStart = null;
            isByLevelStart = false;
        }

        if(isByWorkbench && Input.GetButton("Action" + playerNumber) && !MouseBlocker.mouseBlocked)
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
        pickedupItem.transform.position = ItemPosition.position;
        HeldWatch.GetComponent<Watch>().isSelected = false;
        animator.SetBool("carriesItem", true);
    }

    private void DropItem()
    {
        HeldWatch.transform.position = transform.position;
        HeldWatch = null;
        animator.SetBool("carriesItem", false);
    }

    private void PlaceItemInWorkbench()
    {       
        HeldWatch.GetComponent<Watch>().isSelected = false;
        nearbyWorkbench.PlaceItem(HeldWatch.GetComponent<Watch>());
        HeldWatch = null;
        animator.SetBool("carriesItem", false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Workbench"))
        {
            nearbyWorkbench = collision.GetComponent<Workbench>();
            isByWorkbench = true;
        }
        else if (collision.CompareTag("LevelStart"))
        {
            nearbyLevelStart = collision.GetComponent<LevelStart>();
            isByLevelStart = true;
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
        else if (collision.CompareTag("LevelStart"))
        {
            nearbyLevelStart = null;
            isByLevelStart = false;
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
