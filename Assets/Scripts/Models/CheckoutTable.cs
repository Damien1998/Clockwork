﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Refactoring is done! You may enter safely
public class CheckoutTable : Workbench
{
    private int watchIndex = 0;
    [SerializeField] private LevelParams workbenchLevelParams;

    [SerializeField] private TextAsset endOfLevelDialogue,questEndDialogue;
    
    [SerializeField] private Transform watchThrowPoint;
    [SerializeField] private ParticleSystem deliveryFX, retrievedFX;
    private int watchesFixed;
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
        //If you want to change the order which the watches spawn in go into "Resources/LevelParams/Level  directory
        workbenchLevelParams = Resources.Load<LevelParams>("LevelParams/Level " + GameManager.instance.levelID);
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
        GameManager.instance.levelID = GameManager.instance.levelID;
        GameManager.instance.localQuestDone = false;
        GameManager.instance.CreateRandomWatches();
        //ThrowRandomWatch();
        StartCoroutine(CheckForQuests());
        StartCoroutine(DispenseWatches());
    }

    public override void PlaceItem(Watch itemToPlace)
    {
        if(CheckQuestWatch(itemToPlace))
        {
            itemToPlace.transform.position = transform.position;
            Destroy(itemToPlace.gameObject);

            GameManager.instance.localQuestDone = true;

            //Idk how to end quests :/
        }
        if(CheckWatch(itemToPlace) == true)
        {
            retrievedFX.Play();
            //I'm not sure whether we really need that there:
            //base.PlaceItem(itemToPlace);
            //watchIndex++;
            itemToPlace.transform.position = transform.position;
            Destroy(itemToPlace.gameObject);
            if(workbenchLevelParams.watchAmount <= watchesFixed)
            {
                //ThrowNewWatch(workbenchLevelParams.listOfWatches[watchIndex]);

                //ThrowRandomWatch();
                Debug.Log("Last watch");

                if (GameManager.instance.localQuestDone)
                {
                    DialogueManager.instance.StartDialogue(questEndDialogue);
                }
                else
                {
                    DialogueManager.instance.StartDialogue(endOfLevelDialogue);
                }
                UIManager.instance.StopTimer();
            }
            //else
            //{
            //    //UIManager.instance.ShowLevelEnd();
                
            //}
        }
    }

    IEnumerator DispenseWatches()
    {
        while(watchIndex < workbenchLevelParams.watchAmount)
        {
            deliveryFX.Play();
            ThrowRandomWatch();
            yield return new WaitForSeconds(workbenchLevelParams.watchDispensingTime);
            watchIndex++;
        }
    }

    protected override void DropItems()
    {
        Debug.Log("VAR234");
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
        yield return new WaitForSeconds(1f);
        if (SaveController._sideQuest)
        {
            ThrowNewWatch(GameManager.instance.currentLevelParams.questItem);
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
        var pos = transform.position;
        if (watchThrowPoint != null)
        {
            pos = watchThrowPoint.position;
        }

        var newWatch = Instantiate(WatchTemplate, pos, Quaternion.identity);
        var newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetParameters(itemParameters);
        newWatch.GetComponent<Watch>().WatchItem = newItem;
        newWatch.GetComponent<Watch>().isCompleteWatch = true;
        newWatch.GetComponent<Watch>().TrueState = ItemState.Broken;
    }

    private void ThrowRandomWatch()
    {
        var pos = transform.position;
        if(watchThrowPoint != null)
        {
            pos = watchThrowPoint.position;
        }

        var newWatch = Instantiate(WatchTemplate, pos, Quaternion.identity);
        var newItem = ScriptableObject.CreateInstance<Item>();
        newItem.SetParameters(GameManager.instance.randomWatches[watchIndex]);
        newWatch.GetComponent<Watch>().WatchItem = newItem;
        newWatch.GetComponent<Watch>().isCompleteWatch = true;
        newWatch.GetComponent<Watch>().TrueState = ItemState.Broken;
    }
    /*
     PS
     <summary>
     The bool checks if the Item we placed matches the requirements of the CheckoutTable
     </summary>
     */
    private bool CheckWatch(Watch currentWatch)
    {
        if(currentWatch.WatchItem.State == ItemState.Repaired&&currentWatch.isCompleteWatch)
        {
            for (int i = 0; i < GameManager.instance.randomWatches.Count; i++)
            {
                if (currentWatch.WatchItem.itemID == GameManager.instance.randomWatches[i].itemID)
                {
                    watchesFixed++;
                    return true;
                }
            }
        }

        return false;
        
    }

    private bool CheckQuestWatch(Watch currentWatch)
    {
        if(SaveController._sideQuest)
        {
            if (currentWatch.WatchItem.State == ItemState.Repaired && currentWatch.WatchItem.itemID == SaveController.GetQuestItem(GameManager.instance.levelID).itemID)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
        
    }
}
