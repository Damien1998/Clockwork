using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchBasic : Workbench
{   
    // Start is called before the first frame update
    void Start()
    {
        //The basic workbench has three slots
        //If there needs to be more, I can work out a way to specify the number in the editor
        numberOfSlots = 3;
        itemSlots = new Watch[3];

        workTimer = workTimerBase;
    }

    //PlaceItem() is not overriden
    //This workbench accepts all items

    // Update is called once per frame
    void Update()
    {
        if (isOperated && itemSlots[0] != null)
        {
            Work();
        }
        else
        {
            workTimer = workTimerBase;
        }

        if (workTimer <= 0)
        {
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
        }

    }

    protected override void DropItems()
    {
        //Here the items are combined or broken down according to recipes

        //If there is only one item, it is broken down
        //This doesn't accept mechanisms
        if (itemSlots[1] == null)
        {
            if(itemSlots[0] != null && itemSlots[0].WatchItem.itemID != 10)
            {
                var currentItem = itemSlots[0];
                for (int i = 0; i < currentItem.WatchItem.components.Count; i++)
                {
                    itemSlots[i] = GenerateItem(currentItem.WatchItem.components[i]);
                    if(itemSlots[i].WatchItem.TrueState == ItemState.Repaired && itemSlots[0].WatchItem.components.Count != 0)
                    {
                        itemSlots[i].WatchItem.State = itemSlots[i].WatchItem.TrueState;
                    }
                }
                Destroy(currentItem.gameObject);
            }           
        }
        //If there are more items, the workbench tries to combine them
        //Again, this does not accept mechanisms
        else
        {
            SortItems();

            for (int i = 0; i < itemSlots.Length; i++)
            {
                if(itemSlots[i] != null)
                {
                    Debug.Log("Slot: " + i + " Item: " + itemSlots[i].WatchItem.itemID);
                }               
            }

            for (int i = 0; i < GameManager.instance.RecipesList.Count; i++)
            {
                Debug.Log("Trying a recipe");
                var recipeFilled = true;               

                for (int j = 0; j < itemSlots.Length; j++)
                {
                    if ((itemSlots[j] != null && GameManager.instance.RecipesList[i].Items.Count > j && itemSlots[j].WatchItem.itemID != GameManager.instance.RecipesList[i].Items[j].itemID)
                        || (itemSlots[j] == null && GameManager.instance.RecipesList[i].Items.Count < j)
                        || (itemSlots[j] != null && itemSlots[j].WatchItem.State != ItemState.Repaired))
                    {
                        //Debug.Log("Uh-oh, bad item: " + itemSlots[j].WatchItem.itemID);
                        recipeFilled = false;
                    }                  
                }

                if (recipeFilled)
                {
                    EmptySlot(0);
                    EmptySlot(1);
                    EmptySlot(2);
                    //itemSlots[0] = Instantiate(GameManager.instance.basicRecipes[i].resultWatch, transform.position, Quaternion.identity);     
                    itemSlots[0] = GenerateItem(GameManager.instance.RecipesList[i].result);
                    itemSlots[0].WatchItem.State = itemSlots[0].WatchItem.TrueState;
                    break;
                }
            }
        }

        base.DropItems();
    }
}
