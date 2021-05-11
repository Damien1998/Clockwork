using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpRange : MonoBehaviour
{
    private List<Collider2D> nearbyItems = new List<Collider2D>();
    private int _itemId;
    private int itemToPickUpID { get => _itemId;
        set { _itemId = value; HighLightCurrentItem();}
    }

    private bool CheckForChanges<T>(List<T> list1,List<T> list2)
    {
        if (list1.Count != list2.Count)
        {
            return false;
        }
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].Equals(list2[i]))
            {
                return false;
            }
        }
        return true;
    }
    public bool HighLightItems = true;


    private void Update()
    {
        if (nearbyItems.Count > 0 && !Input.GetButton("Pickup1")&&!Input.GetButtonUp("Pickup1"))
        {
            var tmpList = nearbyItems.OrderBy(item => Vector3.Distance(item.gameObject.transform.position, transform.position)).ToList();
            if(CheckForChanges(tmpList,nearbyItems))
            {
                nearbyItems = tmpList;
                itemToPickUpID = 0;
            } 
        }
    }
    public void ClearAllItems()
    {
        foreach (var t in nearbyItems)
        {
            t.GetComponent<Watch>().isSelected = false;
        }
    }
    public void ChangePickedUpObject()
    {
        if (nearbyItems.Count > 0)
        {
            if (itemToPickUpID + 1 > nearbyItems.Count-1)
            {
                itemToPickUpID = 0;
            }
            else
            {
                itemToPickUpID++;
            }
        }
    }
    public GameObject GetPickedUpObject()
    {
        return nearbyItems.Count > 0 ? nearbyItems[itemToPickUpID].gameObject : null;
    }
    private void HighLightCurrentItem()
    {
        if (HighLightItems && nearbyItems.Count>0)
        {
            ClearAllItems();
            nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Watch watch))
        {
            nearbyItems.Add(other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out Watch watch) && nearbyItems.Contains(other))
        {
            nearbyItems.Remove(other);
            if (watch.isSelected)
            {
                watch.isSelected = false;
            }
        }
    }
    
}
