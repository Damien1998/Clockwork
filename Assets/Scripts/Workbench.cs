﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Workbench : MonoBehaviour
{
    [SerializeField]
    protected int numberOfSlots;
    protected Watch[] itemSlots;

    [SerializeField]
    protected float workTimerBase;
    protected float workTimer;

    [SerializeField]
    public Transform dropLocation;

    [SerializeField]
    protected ProgressWheel timerDisplay;

    [SerializeField]
    public Transform[] slotPositions;

    public GameObject WatchTemplate;

    [SerializeField]
    protected ParticleSystem workParticles, endParticles;

    [SerializeField]
    private GameObject collisionLock;

    //This is only used in the basic and precise workbenches
    //But it has to be here so that other scripts can easily access it
    //Set this to true if the player is pushing the action button near a workbench
    public bool isOperated;

    //Use for filling slots with items
    //I'm not sure whether items will still be handled the same
    //For now I made the function deactivate items placed in slots
    public virtual void PlaceItem(Watch itemToPlace)
    {
        for(int i = 0; i < numberOfSlots; i++)
        {
            if(itemSlots[i] == null)
            {
                //TODO - actual places for items
                itemToPlace.transform.position = slotPositions[i].position;
                itemToPlace.isPlacedOnWorkbench = true;
                itemSlots[i] = itemToPlace;
                //if(collisionLock != null)
                //{
                //    StartCoroutine(LockItemCollision());
                //}
                //itemToPlace.gameObject.SetActive(false);

                break;
            }
        }
    }

    protected void KeepSlotsInPlace(bool yAxis)
    {
        for(int i = 0; i < itemSlots.Length; i++)
        {
            if(itemSlots[i] != null)
            {
                if(yAxis)
                {
                    itemSlots[i].transform.position = new Vector2(itemSlots[i].transform.position.x, slotPositions[i].position.y);
                }
                else
                {
                    itemSlots[i].transform.position = new Vector2(slotPositions[i].position.x, itemSlots[i].transform.position.y);
                }
            }
        }
    }

    IEnumerator LockItemCollision()
    {
        collisionLock.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        collisionLock.SetActive(false);
    }

    //Workbench functionality
    //There is no need to override it anywhere
    protected void Work()
    {
        if (workTimer == workTimerBase)
        {
            timerDisplay.gameObject.SetActive(true);
        }
        var value = (workTimerBase - workTimer) / workTimerBase;
        timerDisplay.SetValue(value);

        workTimer -= Time.deltaTime;
    }

    //Dropping items
    protected virtual void DropItems()
    {
        //StartCoroutine(WaitAndDrop(0.1f));
        SoundManager.PlaySound(SoundManager.Sound.WorkItemEject);
        for(int i = 0; i< numberOfSlots; i++)
        {
            if(itemSlots[i] != null)
            {
                //itemSlots[i].transform.position = dropLocation.position;
                //itemSlots[i].isPlacedOnWorkbench = false;
                itemSlots[i].gameObject.SetActive(true);
                //itemSlots[i].ChangeSortingLayer("Items");
                //itemSlots[i] = null;
                StartCoroutine(LerpItemToPos(dropLocation.position, 0.2f, i));
            }
        }
    }

    protected IEnumerator WaitAndDrop(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] != null)
            {
                //itemSlots[i].transform.position = dropLocation.position;
                //itemSlots[i].isPlacedOnWorkbench = false;
                itemSlots[i].gameObject.SetActive(true);
                //itemSlots[i].ChangeSortingLayer("Items");
                //itemSlots[i] = null;
                StartCoroutine(LerpItemToPos(dropLocation.position, 0.2f, i));
            }
        }
    }

    IEnumerator LerpItemToPos(Vector2 targetPos, float duration, int itemID)
    {
        float time = 0;
        Vector2 startPos = slotPositions[0].position;

        //if(!clearItem)
        //{
        //    lockMovement = true;
        //}


        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t);

            itemSlots[itemID].transform.position = Vector2.Lerp(startPos, targetPos, t);
            time += Time.deltaTime;
            yield return null;
        }

        //lockMovement = false;

        itemSlots[itemID].transform.position = targetPos;
        itemSlots[itemID].ChangeSortingLayer("Items");
        itemSlots[itemID].isPlacedOnWorkbench = false;
        itemSlots[itemID] = null;
    }

    protected Watch GenerateItem(Item parameters)
    {
        var newItem = Instantiate(WatchTemplate, transform.position, Quaternion.identity);
        var newItemData = new Item();
        newItemData.SetParameters(parameters);
        newItem.GetComponent<Watch>().WatchItem = newItemData;
        return newItem.GetComponent<Watch>();
    }

    protected void EmptySlot(int slot)
    {
        //Debug.Log("Emptying slot: " + slot);
        if(itemSlots[slot] != null)
        {
            Destroy(itemSlots[slot].gameObject);
            itemSlots[slot] = null;
        }
    }

    //Sorts items in the workbench by their ID
    //Empty slots are placed at the end
    protected void SortItems()
    {
        List<Watch> temp = new List<Watch>();
        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] != null)
            {
                temp.Add(itemSlots[i]);
            }
        }
        temp = temp.OrderBy(item => item.WatchItem.itemID).ToList();
        for (int i = 0; i < temp.Count; i++)
        {
            itemSlots[i] = temp[i];
        }
    }
}
