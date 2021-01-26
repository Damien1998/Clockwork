using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Refactoring is done! You may enter safely
public class CheckoutTable : Workbench
{
    private int watchIndex = 0;
    [SerializeField] private LevelParams workbenchLevelParams;

    [SerializeField] private POI endOfLevelDialogue;
    //public GameObject WatchTemplate;

    void Start()
    {
        //PS
        //<summary>
        //About the WatchList ScriptableObject script
        //</summary>
        //Since we will have more then 1 level expected at the moment of making this
        //It's more comfortable to have a quick and easy to use object that can specify what kind of watches will spawn in the order that is put in
        //This way we can make list of watches that will be used instantly
        //If you want to change the order which the watches spawn in go into "Assets/Prefabs/LevelParams/Level" directory
        workbenchLevelParams = (LevelParams)AssetDatabase.LoadAssetAtPath("Assets/Prefabs/LevelParams/Level " + GameManager.instance.levelID +".asset", typeof(LevelParams));       
    }
    /*
     * PS
     * <summary>
     * Function Checks for CorrectWatch and Spawns new One if it's given the repaired correct one
     * </summary>
     * Currently because of no easy way to access the and store the item that player holds i used the currentWatch variable as a storage
     * Possibly in the future it would be more efficient and comfortable if we had one method to handle putting watches in the workbenches
     */
     /**
    private void ReturningWatches(Watch returnedWatch)
    {
        returnedWatch.transform.position = transform.position;
        if (CheckWatch(returnedWatch) == true)
        {
                watchIndex++;
                ThrowNewWatch(workbenchWatchList.listOfWatches[watchIndex]);
                // GameManager.instance.AddPoints(1);      Since the game ends when the point is added as of now i'll just leave it commented 
        }
        
    }
    **/

    public void InitLevel()
    {
        ThrowNewWatch(workbenchLevelParams.listOfWatches[watchIndex]);
        StartCoroutine(CheckForQuests());
    }

    public override void PlaceItem(Watch itemToPlace)
    {
        
        if(CheckWatch(itemToPlace) == true)
        {
            //I'm not sure whether we really need that there:
            //base.PlaceItem(itemToPlace);
            watchIndex++;
            Destroy(itemToPlace.gameObject);
            if(workbenchLevelParams.listOfWatches.Count >  watchIndex)
            {
                ThrowNewWatch(workbenchLevelParams.listOfWatches[watchIndex]);
            }
            else
            {
                UIManager.instance.ShowLevelEnd();
                endOfLevelDialogue.StartDialogue();

                Time.timeScale = 0f;
            }
        }
    }

    protected override void DropItems()
    {
        
    }
    /**
    private void CheckForQuests()
    {
        if (GameManager.instance.sideQuestActive)
        {
            ThrowNewWatch(GameManager.instance.questItem);  
        }
    }
    **/

    //Checking for quests is done after a sort delay so that wathches don't go funky on converor belts
    IEnumerator CheckForQuests()
    {
        yield return new WaitForSeconds(0.5f);
        if (GameManager.instance.sideQuestActive)
        {
            ThrowNewWatch(GameManager.instance.questItem);
        }
    }

    /*
    PS
    <summary>
    This is a simple method to make new watch if there is none at the moment of creating 
    </summary> 
    Sadly as of now it does not pool the watch due to the nature of watch components and fact that the watch object constantly changes in scene
    But it can be noted that this might be changed to pooling later on in the production
    */
    private void ThrowNewWatch(Item itemParameters)
    {
        var newWatch = Instantiate(WatchTemplate, transform.position, Quaternion.identity);
        var newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetParameters(itemParameters);
        newWatch.GetComponent<Watch>().WatchItem = newItem;
        newWatch.GetComponent<Watch>().TrueState = ItemState.Broken;
    }

    //Placing items in done on the side of the player
    /**
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Watch watch)&&!watch.isSelected)
        {
            ReturningWatches(watch);
        }
    }
    **/
    /*
     PS
     <summary>
     The bool checks if the Item we placed matches the requirements of the CheckoutTable
     </summary>
     */
    private bool CheckWatch(Watch currentWatch)
    {
        if (currentWatch.WatchItem.State ==  ItemState.Repaired)
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
