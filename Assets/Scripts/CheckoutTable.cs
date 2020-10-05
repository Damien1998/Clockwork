using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Refactoring is done! You may enter safely
public class CheckoutTable : MonoBehaviour
{
    private bool playerCollided;
    private Player[] interactingPlayer;
    private int watchIndex = 0;
    private WatchList workbenchWatchList;

    void Start()
    {
        interactingPlayer = new Player[2];
        interactingPlayer[0] = null;
        interactingPlayer[1] = null;
        //PS
        //<summary>
        //About the WatchList ScriptableObject script
        //</summary>
        //Since we will have more then 1 level expected at the moment of making this
        //It's more comfortable to have a quick and easy to use object that can specify what kind of watches will spawn in the order that is put in
        //This way we can make list of watches that will be used instantly
        //If you want to change the order which the watches spawn in go into "Assets/Prefabs/WatchOrderLists/Level" directory
        workbenchWatchList = (WatchList)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/WatchOrderLists/Level" + GameManager.instance.levelID +".asset", typeof(WatchList));
        ThrowNewWatch(null);
    }
    void Update()
    {
        if (interactingPlayer[0] != null)
        {
            ReturningWatches(0);           
        }
        if (interactingPlayer[1] != null)
        {
            ReturningWatches(1);
        }
    }
    /*
     * PS
     * <summary>
     * Function Checks for CorrectWatch and Spawns new One if it's given the repaired correct one
     * </summary>
     * Currently because of no easy way to access the and store the item that player holds i used the currentWatch variable as a storage
     * Possibly in the future it would be more efficient and comfortable if we had one method to handle putting watches in the workbenches
     */
    private Activator currentWatch;
    private void ReturningWatches(int playerID)
    {
        if (playerCollided
            &&interactingPlayer[playerID].droppedItemActivator != null)
        {
            currentWatch = interactingPlayer[playerID].droppedItemActivator;
        }
        if (Input.GetButtonUp("Pickup" + interactingPlayer[playerID].playerNumber)&&currentWatch!=null)
        {
            currentWatch.transform.position = transform.position;
            if (CheckWatch(currentWatch.child) == true)
            {
                watchIndex++;
                ThrowNewWatch(currentWatch.gameObject);
                // GameManager.instance.AddPoints(1);      Since the game ends when the point is added as of now i'll just leave it commented 
            }
        }
    }
    /*
    PS
    <summary>
    This is a simple method to make new watch if there is none at the moment of creating 
    It also replaces any watch that matches the requirements
    </summary> 
    Sadly as of now it does not pool the watch due to the nature of watch components and fact that the watch object constantly changes in scene
    But it can be noted that this might be changed to pooling later on in the production
    */
    private void ThrowNewWatch(GameObject mywatch)
    {
        if (mywatch == null)
        {
            Instantiate(workbenchWatchList.listOfWatches[watchIndex],transform.position,Quaternion.identity);
        }
        else
        {
            Destroy(mywatch);
            Instantiate(workbenchWatchList.listOfWatches[watchIndex],transform.position,Quaternion.identity);
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
        if (interactingPlayer[0] == collision.GetComponent<Player>())
        {
            interactingPlayer[0] = null;
        }
        else
        {
            interactingPlayer[1] = null;
        }
        Debug.Log("Item odkolidowuje");
    }
    /*
     PS
     <summary>
     The bool checks if the Item we placed matches the requirements of the CheckoutTable
     </summary>
     */
    private bool CheckWatch(Item currentWatch)
    {
        if (!currentWatch.broken&&currentWatch.itemID == workbenchWatchList.listOfWatches[watchIndex].GetComponent<Activator>().child.itemID)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
// I'll just leave it here if we need it in the future
// private void UseCheckoutTable(int playerID)
// {
//     if (!pickedUp && watch != null 
//         && Input.GetButtonUp("Pickup" + interactingPlayer[playerID].playerNumber) 
//         && !interactingPlayer[playerID].carriesItem 
//         && interactingPlayer[playerID].freeToPickup)
//     {
//         interactingPlayer[playerID].PickupItem(watch.child.itemImage, watch.child.stateSprite.sprite, watch);           
//         watchSlot.sprite = watch.child.itemImage;
//         watch.SetChildState(false);
//         pickedUp = true;
//     }
//
//     //Return fixed watch
//     if (pickedUp && watch != null 
//         && Input.GetButtonUp("Pickup" + interactingPlayer[playerID].playerNumber) 
//         && interactingPlayer[playerID].carriesItem)
//     {
//         Debug.Log("Oddawanko");
//         if (interactingPlayer[playerID].droppedItemActivator.child.itemID == _watchID 
//             && !interactingPlayer[playerID].droppedItemActivator.child.broken)
//         {
//             interactingPlayer[playerID].ClearItem();
//             watchSlot.sprite = null;
//             GameManager.instance.AddPoints(1);
//         }
//     }
// }
