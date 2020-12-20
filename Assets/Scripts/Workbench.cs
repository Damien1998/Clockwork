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
    protected Transform dropLocation;

    [SerializeField]
    protected Slider timerDisplay;

    public Transform[] slotPositions;

    [SerializeField]
    //This is clunky. Ugh
    protected bool verticalSpread;

    public GameObject WatchTemplate;

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
                itemSlots[i] = itemToPlace;
                //itemToPlace.gameObject.SetActive(false);
                
                break;
            }
        }
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
        timerDisplay.value = value;

        workTimer -= Time.deltaTime;
    }

    //Dropping items
    protected virtual void DropItems()
    {
        int slotsFilled = 0;
        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] != null)
            {
                slotsFilled++;
            }
        }

        //Current way of handling offset is kind of clunky
        //I gotta think of a better algorithm
        var offset = 0f;
        if(slotsFilled > 1)
        {
            offset = 2f / (slotsFilled - 1);
        }

        for(int i = 0; i< slotsFilled; i++)
        {
            if (numberOfSlots > 1)
            {
                if (verticalSpread)
                {
                    itemSlots[i].transform.position = new Vector3(dropLocation.position.x, dropLocation.position.y - 1f + (offset * i), 0);
                }
                else
                {
                    itemSlots[i].transform.position = new Vector3(dropLocation.position.x - 1f + (offset * i), dropLocation.position.y, 0);
                }
            }
            else
            {
                itemSlots[i].transform.position = dropLocation.position;
            }

            itemSlots[i].gameObject.SetActive(true);
            itemSlots[i] = null;
        }
    }

    protected Watch GenerateItem(Item parameters)
    {
        var newItem = Instantiate(WatchTemplate, transform.position, Quaternion.identity);
        var newItemData = ScriptableObject.CreateInstance<Item>();
        newItemData.SetParameters(parameters);
        newItem.GetComponent<Watch>().WatchItem = newItemData;
        return newItem.GetComponent<Watch>();
    }

    protected void EmptySlot(int slot)
    {
        Debug.Log("Emptying slot: " + slot);
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
