using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpRange : MonoBehaviour
{
    private List<Collider2D> nearbyItems = new List<Collider2D>();
    private int itemToPickUpID = 0;


    public void ChangePickedUpObject()
    {
        if (nearbyItems.Count > 0)
        {
            nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = false;
            if (itemToPickUpID + 1 > nearbyItems.Count-1)
            {
                itemToPickUpID = 0;
            }
            else
            {
                itemToPickUpID++;
            }
            nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
        }
    }
    public GameObject GetPickedUpObject()
    {
        if (nearbyItems.Count > 0)
        {
            return nearbyItems[itemToPickUpID].gameObject;
        }
        else
        {
            return null;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Watch watch))
        {
            nearbyItems.Add(other);
            if (nearbyItems.Count < 2)
            {
                watch.isSelected = true;
            }
            else
            {
                for (int i = 0; i < nearbyItems.Count; i++)
                {
                    nearbyItems[i].GetComponent<Watch>().isSelected = false;
                }
                
                nearbyItems.OrderBy(item => item.transform.position.y);
                nearbyItems.Reverse();
                itemToPickUpID = nearbyItems.IndexOf(other);
                
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Watch watch))
        {
            nearbyItems.Remove(other);
            if (watch.isSelected)
            {
                watch.isSelected = false;
            }
            if (nearbyItems.Count > 1)
            {
                itemToPickUpID = nearbyItems.Count - 1;
                nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
            }
            else
            {
                itemToPickUpID = 0;
            }
        }
    }
    
}
