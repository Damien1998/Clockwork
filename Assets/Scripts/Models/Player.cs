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

    public static bool CanInteract = false;
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

    public bool lockMovement;

    //We wanted to handle interaction from the side of the player 
    //having a bool for every type of interactable object in the workshop feels clunky
    //Especially if some more of them will be there
    //Idk what to do 
    // +1
    public bool isByWorkbench;
    public bool isByLevelStart;
    public bool isByWarpHole;
    private Workbench nearbyWorkbench;
    private LevelStart nearbyLevelStart;
    private WarpHole nearbyWarpHole;

    [SerializeField]
    private float dashDuration;

    private Vector2 movementInput, lastDirection;

    [SerializeField]
    private ParticleSystem itemDropParticles, footstepParticles;

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
        CanInteract = true;
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public static void SetPlayer(bool on)
    {
        CanInteract = on;
    }
    private Collider2D[] nearbyItems = null;
    void PickUp()
    {
        //Reworked item pick up mechanic
        //First checks for input and Locks movement then Searches for items in Range and sorts them accordingly to their position
        if (Input.GetButtonDown("Pickup" + playerNumber))
        {
            lockMovement = true;
            nearbyItems = Physics2D.OverlapCircleAll(transform.position, pickupRange, LayerMask.GetMask("Item"));
            nearbyItems = nearbyItems.OrderBy(item => item.transform.position.y).ToArray();
            Array.Reverse(nearbyItems);
            if (nearbyItems.Length > 0)
            {
                if (nearbyItems[itemToPickUpID].TryGetComponent(out Watch watch))
                {
                    nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
                }
            }
        }
        //After the first action if player is performing secondary action ie right click it highlights the item in sorted list
        if (nearbyItems != null && nearbyItems.Length > 0&&Input.GetButtonDown("Action" + playerNumber))
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
        //At the end checks if player is not holding a button anymore and picksup  highlighted item
        if(nearbyItems != null && Input.GetButtonUp("Pickup" + playerNumber))
        {
            if (itemToPickUpID < nearbyItems.Length  )
            {
                if (!nearbyItems[itemToPickUpID].GetComponent<Watch>().isPlacedOnWorkbench &&
                    !nearbyItems[itemToPickUpID].GetComponent<Watch>().isRecipe)
                {
                    PickUpItem(nearbyItems[itemToPickUpID].gameObject);
                }
                else if(nearbyItems[itemToPickUpID].GetComponent<Watch>().isRecipe)
                {
                    RecipeListView.currentMainWatch = nearbyItems[itemToPickUpID].GetComponent<Watch>();
                    RecipeListView.LoadRecipeView();
                }
            }
            
            itemToPickUpID = 0;
            nearbyItems = null;
            lockMovement = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (CanInteract)
        {
            if (HeldWatch != null)
            {
                HeldWatch.transform.position = transform.position + ItemPosition.localPosition;
            }

            if (HeldWatch == null)
            {
                PickUp();
            }
            else if (Input.GetButtonDown("Pickup" + playerNumber) && HeldWatch != null)
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

            if (isByLevelStart && Input.GetButtonDown("Action" + playerNumber))
            {
                nearbyLevelStart.StartLevel();
                nearbyLevelStart = null;
                isByLevelStart = false;
            }

            if (isByWorkbench && Input.GetButton("Action" + playerNumber))
            {
                nearbyWorkbench.isOperated = true;
            }
            else if (isByWorkbench)
            {
                nearbyWorkbench.isOperated = false;
            }

            if (isByWarpHole && Input.GetButtonDown("Action" + playerNumber) && !lockMovement && !isByWorkbench)
            {
                if (HeldWatch != null)
                {
                    DropItem();
                }

                nearbyWarpHole.Teleport(this);
                nearbyWarpHole = null;
                isByWarpHole = false;
            }
            Dashing();
        }
    }

    private void Dashing()
    {
        //Dash - only in 4 directions, quick burst of speed
        //This only handles dash input, velocity is managed in fixedupdate
        //For some reason this has to be here

        float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
        float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
        movementInput = new Vector2(moveX, moveY).normalized;

        if (movementInput != Vector2.zero)
        {
            animator.SetBool("walking", true);
            animator.SetFloat("moveX", movementInput.x);
            animator.SetFloat("moveY", movementInput.y);

            PlayFootstepFX();

            if (Input.GetButtonDown("Dash") && !isDashing)
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
        else
        {
            footstepParticles.Stop();
            animator.SetBool("walking", false);
        }
    }
    private void FixedUpdate()
    {
        //Player movement      
        if (CanInteract)
        {
            float moveX = Input.GetAxisRaw("Horizontal" + playerNumber);
            float moveY = Input.GetAxisRaw("Vertical" + playerNumber);
            movementInput = new Vector2(moveX, moveY).normalized;

            if (!lockMovement)
            {
                Vector2 xInput = new Vector2(moveX, 0);
                Vector2 yInput = new Vector2(0, moveY);

                if (movementInput != Vector2.zero)
                {
                    if (xInput.magnitude >= yInput.magnitude)
                    {
                        lastDirection = xInput;
                    }
                    else
                    {
                        lastDirection = yInput;
                    }

                    SoundManager.PlaySound(SoundManager.Sound.PlayerMove);
                }

                //Managing player speed
                if (movementInput != Vector2.zero && !isDashing)
                {
                    rigidBody.velocity = movementInput.normalized * moveSpeed;
                }
                else if (isDashing)
                {
                    rigidBody.velocity = movementInput.normalized * moveSpeed;
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

            if (Input.GetButton("Dash"))
            {
                dashReleased = false;
            }
            else
            {
                dashReleased = true;
            }
        }
    }

    public void PlayFootstepFX()
    {
        if(!footstepParticles.isEmitting)
        {
            footstepParticles.Play();
        }
        
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
        SoundManager.PlaySound(SoundManager.Sound.ItemPickUp);
        if (pickedupItem.TryGetComponent(out Rigidbody2D itemRigidbody))
        {
            itemRigidbody.velocity = Vector2.zero;
            pickedupItem.GetComponent<BoxCollider2D>().enabled = false;
        }
        HeldWatch = pickedupItem;
        pickedupItem.transform.position = ItemPosition.localPosition;
        HeldWatch.GetComponent<Watch>().isSelected = false;
        animator.SetBool("carriesItem", true);
    }

    private void DropItem()
    {
        SoundManager.PlaySound(SoundManager.Sound.ItemDrop);
        if (HeldWatch.TryGetComponent(out Rigidbody2D itemRigidbody))
        {
            itemRigidbody.velocity = Vector2.zero;
            HeldWatch.GetComponent<BoxCollider2D>().enabled = true;
        }
        HeldWatch.transform.position = transform.position + new Vector3(lastDirection.x, lastDirection.y);
        itemDropParticles.transform.position = HeldWatch.transform.position;
        itemDropParticles.Play();
        HeldWatch = null;
        animator.SetBool("carriesItem", false);

        
    }

    private void PlaceItemInWorkbench()
    {
        SoundManager.PlaySound(SoundManager.Sound.WorkBenchPut);
        if (HeldWatch.TryGetComponent(out Rigidbody2D itemRigidbody))
        {
            itemRigidbody.velocity = Vector2.zero;
            HeldWatch.GetComponent<BoxCollider2D>().enabled = true;
        }
        HeldWatch.GetComponent<Watch>().isSelected = false;
        nearbyWorkbench.PlaceItem(HeldWatch.GetComponent<Watch>());
        itemDropParticles.transform.position = HeldWatch.transform.position;
        itemDropParticles.Play();
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
        else if (collision.CompareTag("Warp"))
        {
            nearbyWarpHole = collision.GetComponent<WarpHole>();
            isByWarpHole = true;
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
        else if (collision.CompareTag("Warp"))
        {
            nearbyWarpHole = null;
            isByWarpHole = false;
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
