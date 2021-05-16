using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchBasic : Workbench
{
    [SerializeField] private ParticleSystem checkMark, crossMark;
    private Item watchToDrop;

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
            if (!workParticles.isEmitting)
            {
                workParticles.Play();
            }
            Work();
        }
        else
        {
            workParticles.Stop();
            timerDisplay.gameObject.SetActive(false);
            workTimer = workTimerBase;
        }

        if (workTimer <= 0)
        {
            workParticles.Stop();
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
        }

    }

    protected override void DropItems()
    {
        bool isValid = true;
        //Here the items are combined or broken down according to recipes

        //If there is only one item, it is broken down
        //This doesn't accept mechanisms
        if (itemSlots[1] == null && itemSlots[0].WatchItem.State != ItemState.UnknownState)
        {
            if(itemSlots[0] != null && itemSlots[0].WatchItem.itemID != 10 && itemSlots[0].WatchItem.State == ItemState.ComplexBroken)
            {
                var currentItem = itemSlots[0];
                for (int i = 0; i < currentItem.WatchItem.components.Count; i++)
                {
                    itemSlots[i] = GenerateItem(currentItem.WatchItem.components[i]);
                    if(itemSlots[i].WatchItem.trueState == ItemState.Repaired && itemSlots[0].WatchItem.components.Count != 0)
                    {
                        itemSlots[i].WatchItem.State = itemSlots[i].WatchItem.trueState;
                    }
                }
                if(currentItem.WatchItem.components.Count > 0)
                {
                    //endParticles.Play();
                    Destroy(currentItem.gameObject);
                }
                else
                {
                    isValid = false;
                }
            }
            else
            {
                isValid = false;
            }
        }
        //If there are more items, the workbench tries to combine them
        //Again, this does not accept mechanisms
        else
        {
            SortItems();
            
                var recipeFilled = CheckForAllComponents(itemSlots);               

                if (recipeFilled)              
                {
                    //endParticles.Play();
                    Watch newWatch = WatchTemplate.GetComponent<Watch>();
                    Watch newPart = Instantiate(newWatch, dropLocation.position, Quaternion.identity);
                    newPart.WatchItem = watchToDrop;
                    newPart.WatchItem.trueState = ItemState.Repaired;
                    newPart.WatchItem.State = ItemState.Repaired;
                    newPart.isCompleteWatch = true;

                    EmptySlot(0);
                    EmptySlot(1);
                    EmptySlot(2);
                    

                    checkMark.Play();
                
                }
                if(itemSlots[1] != null)
            {
                isValid = false;
            }
        }

        if (isValid)
        {
            endParticles.Play();
        }
        else
        {
            crossMark.Play();
        }

        base.DropItems();
    }

    bool CheckForAllComponents(Watch[] slots)
    {
        var filledSlots = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                filledSlots++;
            }
        }
        for (int i = 0; i < filledSlots; i++)
        {
            var correctComponents = 0;
            var myParentItem = slots[i].WatchItem.parentItem;
            if (slots[i].WatchItem.State == ItemState.Repaired)
            {
                correctComponents++;
            }
            else
            {
                goto SkipLoop;
            }
            for (int j = 0; j < filledSlots; j++)
            {
                if (j != i)
                {
                    if (slots[j].WatchItem.parentItem == myParentItem&&slots[j].WatchItem.State == ItemState.Repaired)
                    {
                        correctComponents++;
                    }
                }
            }
            
            if (correctComponents >= myParentItem.components.Count)
            {
                watchToDrop = new Item();
                watchToDrop = myParentItem;
                return true;
            }
            SkipLoop: ;
        }
        return false;
    }
}
