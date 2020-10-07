using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Workbench : MonoBehaviour
{
    //Note: Will we keep using sprites to show items on the bench, or will there be actual items placed there

    [SerializeField]
    protected int numberOfSlots;
    protected Watch[] itemSlots;

    [SerializeField]
    protected float workTimerBase;
    protected float workTimer;

    [SerializeField]
    private Transform dropLocation;

    //Use for filling slots with items
    //I'm not sure whether items will still be handled the same
    //For now I made the function deactivate items placed in slots
    public virtual void PlaceItem(Watch itemToPlace)
    {
        for(int i = 0; i < numberOfSlots; i++)
        {
            if(itemSlots[i] == null)
            {
                itemSlots[i] = itemToPlace;
                itemToPlace.gameObject.SetActive(false);
                break;
            }
        }
    }

    //Workbench functionality
    //This method is used only in inherited classes
    protected virtual void Work()
    {

    }

    //Dropping items
    protected virtual void DropItems()
    {
        for(int i = 0; i< numberOfSlots; i++)
        {
            if(itemSlots[i] != null)
            {
                itemSlots[i].transform.position = dropLocation.position;
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i] = null;
            }
        }
    }
}
