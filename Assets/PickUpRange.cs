using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpRange : MonoBehaviour
{
    private List<Collider2D> nearbyItems = new List<Collider2D>();
    private int _itemId;
    private int itemToPickUpID
    {
        get => _itemId;
        set { _itemId = value; HighLightCurrentItem();}
    }

    private bool CheckForChanges<T>(List<T> list1,List<T> list2)
    {
        if (list1.Count != list2.Count)
        {
            return true;
        }
        for (int i = 0; i < list1.Count; i++)
        {
            if (!list1[i].Equals(list2[i]))
            {
                return true;
            }
        }
        return false;
    }

    public bool HighLightItems = true;

    private void Update()
    {
        if (nearbyItems.Count > 0  && !Input.GetButton("Pickup1") && !Input.GetButtonUp("Pickup1"))
        {
            var tmpList = nearbyItems.OrderBy(item => Vector3.Distance(item.gameObject.transform.position, transform.position)).ToList();
            if(CheckForChanges(tmpList,nearbyItems))
            {
                if (nearbyItems.Count > 0)
                {
                    nearbyItems = tmpList;
                }

                if (itemToPickUpID >= 0 && nearbyItems.Count > itemToPickUpID)
                {
                    var tmpWatch = nearbyItems[itemToPickUpID];
                    if (tmpWatch != null&&nearbyItems[0] != null)
                    {
                        nearbyItems[itemToPickUpID] = nearbyItems[0];
                        nearbyItems[0] = tmpWatch;
                    }
                }
                itemToPickUpID = 0;
            }
            else if (nearbyItems.Count == 1)
            {
                itemToPickUpID = 0;
            }
        }
    }

    public void ClearList()
    {
        nearbyItems = new List<Collider2D>();
    }

    public void ResetID()
    {
        itemToPickUpID = 0;
        ClearAllItems();
    }
    public void RefreshItems()
    {
        if (nearbyItems.Count > 0)
        {
            nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = false;
            nearbyItems[itemToPickUpID].GetComponent<Watch>().isSelected = true;
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
        Debug.LogError(nearbyItems.Count);
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
