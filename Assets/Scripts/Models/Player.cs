using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFacing { DOWN, UP, LEFT, RIGHT}

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

    public bool isOnConveyor;

    public bool lockMovement;

    public Vector2 additionalVelocity;

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

    [SerializeField] private PickUpRange _pickUpScript;

    [SerializeField]
    private float dashDuration;

    private Vector2 movementInput, lastDirection;
    private PlayerFacing facing;

    [SerializeField]
    private ParticleSystem itemDropParticles, footstepParticles, dashParticles, itemDropGroundParticles;

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
    void PickUp()
    {
        if (Input.GetButtonDown("Action" + playerNumber))
        {
            _pickUpScript.ChangePickedUpObject();
        }
        if(Input.GetButtonDown("Pickup" + playerNumber) && _pickUpScript.GetPickedUpObject() != null)
        {
            lockMovement = true;
        }
        if (Input.GetButtonUp("Pickup" + playerNumber) && _pickUpScript.GetPickedUpObject() != null)
        {
            lockMovement = false;
            PickUpItem(_pickUpScript.GetPickedUpObject());
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (CanInteract)
        {
            if (HeldWatch != null)
            {
                HeldWatch.transform.position = /*transform.position +*/ ItemPosition.position;
            }

            if (HeldWatch == null)
            {
                PickUp();
            }
            else if (Input.GetButtonUp("Pickup" + playerNumber) && HeldWatch != null)
            {
                if (isByWorkbench)
                {
                    PlaceItemInWorkbench();
                    //if (nearbyWorkbench.requiredFacings.Contains(facing))
                    //{
                    //    PlaceItemInWorkbench();
                    //}
                    //else
                    //{
                    //    DropItem();
                    //}
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
                if(nearbyWorkbench != null)
                {
                    nearbyWorkbench.isOperated = true;
                }
                //nearbyWorkbench.isOperated = true;
            }
            else if (isByWorkbench)
            {
                if (nearbyWorkbench != null)
                {
                    nearbyWorkbench.isOperated = false;
                }
            }

            if (isByWarpHole && Input.GetButtonUp("Action" + playerNumber) && !lockMovement && !isByWorkbench)
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
                SoundManager.PlaySound(SoundManager.Sound.Dash);
                dashParticles.Play();

                Vector2 xInput = new Vector2(moveX, 0);
                Vector2 yInput = new Vector2(0, moveY);

                Debug.Log("Dash");


                //Diagonal dash
                if (xInput.magnitude > 0.3f && yInput.magnitude > 0.3f)
                {
                    if(xInput.x > 0)
                    {
                        if (yInput.y > 0)
                        {
                            dashDirection = new Vector2(1f, 1f).normalized;
                        }
                        else
                        {
                            dashDirection = new Vector2(1f, -1f).normalized;
                        }
                    }
                    else
                    {
                        if (yInput.y > 0)
                        {
                            dashDirection = new Vector2(-1f, 1f).normalized;
                        }
                        else
                        {
                            dashDirection = new Vector2(-1f, -1f).normalized;
                        }
                    }
                }
                else
                {
                    if (xInput.magnitude >= yInput.magnitude)
                    {
                        dashDirection = xInput.normalized;
                    }
                    else
                    {
                        dashDirection = yInput.normalized;
                    }
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
                        if(xInput.x > 0)
                        {
                            facing = PlayerFacing.RIGHT;
                        }
                        else
                        {
                            facing = PlayerFacing.LEFT;
                        }
                    }
                    else
                    {
                        lastDirection = yInput;
                        if (yInput.y > 0)
                        {
                            facing = PlayerFacing.UP;
                        }
                        else
                        {
                            facing = PlayerFacing.DOWN;
                        }
                    }

                    SoundManager.PlaySound(SoundManager.Sound.StepAnna);
                }

                //Managing player speed
                if (movementInput != Vector2.zero && !isDashing)
                {
                    //if (isOnConveyor)
                    //{
                    //    //rigidBody.AddForce(movementInput.normalized * moveSpeed*0.7f);
                    //    rigidBody.velocity = movementInput.normalized * moveSpeed + additionalVelocity;
                    //}
                    //else
                    //{
                    //    rigidBody.velocity = movementInput.normalized * moveSpeed + additionalVelocity;
                    //}
                    rigidBody.velocity = movementInput.normalized * moveSpeed + additionalVelocity;
                }
                else if (isDashing)
                {
                    //if (isOnConveyor)
                    //{
                    //    //rigidBody.AddForce(dashDirection * dashSpeed * 0.7f);
                    //    rigidBody.velocity = dashDirection * dashSpeed + additionalVelocity;
                    //}
                    //else
                    //{
                    //    rigidBody.velocity = dashDirection * dashSpeed + additionalVelocity;
                    //}
                    rigidBody.velocity = dashDirection * dashSpeed + additionalVelocity;
                }
                else /*if(!isOnConveyor)*/
                {
                    rigidBody.velocity = Vector2.zero + additionalVelocity;
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
        _pickUpScript.GetPickedUpObject().GetComponent<Watch>().isSelected = false;
        var heldWatch = pickedupItem.GetComponent<Watch>();
        if (!heldWatch.isPlacedOnWorkbench)
        {
            SoundManager.PlaySound(SoundManager.Sound.ItemPickUp);
            HeldWatch = pickedupItem;
            if (pickedupItem.TryGetComponent(out Rigidbody2D itemRigidbody))
            {
                itemRigidbody.velocity = Vector2.zero;
                pickedupItem.GetComponent<BoxCollider2D>().enabled = false;
            }

            _pickUpScript.HighLightItems = false;
            _pickUpScript.ResetID();
            heldWatch.isSelected = false;
            heldWatch.ChangeSortingLayer("ItemsHeld");
            StartCoroutine(LerpItemToPos(ItemPosition.position, 0.08f, 0));
            
            animator.SetBool("carriesItem", true);
        }
    }

    private void DropItem()
    {
        SoundManager.PlaySound(SoundManager.Sound.ItemDrop);
        if (HeldWatch.TryGetComponent(out Rigidbody2D itemRigidbody))
        {
            itemRigidbody.velocity = Vector2.zero;
            HeldWatch.GetComponent<BoxCollider2D>().enabled = true;
        }
        HeldWatch.GetComponent<Watch>().ChangeSortingLayer("Items");

        //HeldWatch.transform.position = transform.position + new Vector3(lastDirection.x, lastDirection.y);
        //HeldWatch.GetComponent<Rigidbody2D>().AddForce((new Vector2(lastDirection.x, lastDirection.y) * 4), ForceMode2D.Impulse);
        StartCoroutine(LerpItemToPos(transform.position + new Vector3(lastDirection.x, lastDirection.y), 0.08f, 1));
        //StartCoroutine(ThrowItemToPos(new Vector3(lastDirection.x, lastDirection.y), 0.08f));

        _pickUpScript.HighLightItems = true;
        animator.SetBool("carriesItem", false);
        _pickUpScript.RefreshItems();

    }

    private void PlaceItemInWorkbench()
    {
        SoundManager.PlaySound(SoundManager.Sound.ItemPlaced);
        if (HeldWatch.TryGetComponent(out Rigidbody2D itemRigidbody))
        {
            itemRigidbody.velocity = Vector2.zero;
            HeldWatch.GetComponent<BoxCollider2D>().enabled = true;
        }
        HeldWatch.GetComponent<Watch>().ChangeSortingLayer("ItemsWorkbench");
        HeldWatch.GetComponent<Watch>().isSelected = false;

        if(nearbyWorkbench.slotPositions.Length > 0)
        {
            StartCoroutine(LerpItemToPos(nearbyWorkbench.slotPositions[0].position, 0.08f, 2));
        }


        _pickUpScript.HighLightItems = true;
        animator.SetBool("carriesItem", false);


    }

    IEnumerator ThrowItemToPos(Vector2 targetPos, float duration)
    {
        Vector2 startPos = HeldWatch.transform.position;
        float time = 0;
        //if(!clearItem)
        //{
        //    lockMovement = true;
        //}

        //var dist = Vector2.Distance(startPos, targetPos);
        //var dir = (targetPos - startPos).normalized;
        var speed = 1 / duration;

        //HeldWatch.GetComponent<Rigidbody2D>().AddForce(targetPos * speed, ForceMode2D.Impulse);

        while (time < duration)
        {
            HeldWatch.GetComponent<Rigidbody2D>().velocity = targetPos * speed;

            time += Time.deltaTime;
            yield return null;
        }

        //yield return new WaitForSeconds(duration);

        //lockMovement = false;

        //HeldWatch.transform.position = targetPos;
        HeldWatch.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //HeldWatch.GetComponent<Rigidbody2D>().AddForce(targetPos * -800f, ForceMode2D.Impulse);

        itemDropGroundParticles.transform.position = HeldWatch.transform.position;
        itemDropGroundParticles.Play();
        HeldWatch = null;
    }

    IEnumerator LerpItemToPos(Vector2 targetPos, float duration, int interactionType)
    {
        float time = 0;
        Vector2 startPos = HeldWatch.transform.position;

        //if(!clearItem)
        //{
        //    lockMovement = true;
        //}


        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            HeldWatch.transform.position = Vector2.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        //lockMovement = false;

        HeldWatch.transform.position = targetPos;

        if(interactionType == 1)
        {
            itemDropGroundParticles.transform.position = HeldWatch.transform.position;
            itemDropGroundParticles.Play();
            HeldWatch = null;
        }
        else if(interactionType == 2)
        {
            nearbyWorkbench.PlaceItem(HeldWatch.GetComponent<Watch>());
            itemDropParticles.transform.position = HeldWatch.transform.position;
            itemDropParticles.Play();
            HeldWatch = null;
        }
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
