using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchPrecise : Workbench
{
    private List<Item> mechanismComponents;
    [SerializeField] private ParticleSystem checkMark, crossMark;

    // Start is called before the first frame update
    void Start()
    {
        //The basic workbench has four slots
        //If there needs to be more, I can work out a way to specify the number in the editor
        numberOfSlots = 4;
        itemSlots = new Watch[4];

        workTimer = workTimerBase;
    }

    //PlaceItem() remembers the components of an inserted mechanism
    public override void PlaceItem(Watch itemToPlace)
    {
        base.PlaceItem(itemToPlace);

        if (IsAMechanism(itemToPlace.WatchItem))
        {
            mechanismComponents = itemToPlace.WatchItem.components;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isOperated && itemSlots[0] != null)
        {
            if(!workParticles.isEmitting)
            {
                workParticles.Play();
            }
            Work();
        }
        else
        {
            workParticles.Stop();
            workTimer = workTimerBase;
            timerDisplay.gameObject.SetActive(false);
        }

        if (workTimer <= 0)
        {
            workParticles.Stop();
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
        }

    }

    protected bool IsAMechanism(Item itemToCheck)
    {
        if (itemToCheck.itemID == 10 || itemToCheck.itemID == 55)
        {
            return true;
        }
        else return false;
    }

    protected override void DropItems()
    {
        bool isValid = true;
        //Here the items are combined, broken down, or repaired

        //If there is only one item, it is either a mechanism or needs repair
        if (itemSlots[1] == null && itemSlots[0].WatchItem.State != ItemState.UnknownState)
        {
            if (itemSlots[0] != null)
            {
                if (itemSlots[0].WatchItem.State == ItemState.Broken && itemSlots[0].WatchItem.components.Count == 0)
                {
                    //endParticles.Play();
                    checkMark.Play();
                    itemSlots[0].WatchItem.State = ItemState.Repaired;
                }
                else if (itemSlots[0].WatchItem.State != ItemState.EmptyState && IsAMechanism(itemSlots[0].WatchItem))
                {
                    //endParticles.Play();
                    itemSlots[0].WatchItem.State = ItemState.EmptyState;
                    for (int i = 0; i < itemSlots[0].WatchItem.components.Count; i++)
                    {
                        itemSlots[i + 1] = GenerateItem(itemSlots[0].WatchItem.components[i]);
                    }
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
        //If there are more items, the workbench tries to combine them using the mechanism's component list
        else if(mechanismComponents != null)
        {
            SortItems();
            mechanismComponents = mechanismComponents.OrderBy(item => item.itemID).ToList();

            Debug.Log(itemSlots[0].WatchItem.itemID);
            Debug.Log(itemSlots[0].WatchItem.State);

            if (IsAMechanism(itemSlots[0].WatchItem) && itemSlots[0].WatchItem.State == ItemState.EmptyState)
            {
                Debug.Log("Hlep");
                var recipeFilled = true;

                for (int i = 0; i < mechanismComponents.Count; i++)
                {
                    if((itemSlots[i+1] != null && mechanismComponents[i] != null && itemSlots[i+1].WatchItem.itemID != mechanismComponents[i].itemID)
                        || (itemSlots[i + 1] != null && itemSlots[i + 1].WatchItem.State != ItemState.Repaired)
                        || (itemSlots[i + 1] != null && mechanismComponents[i] == null)
                        || (itemSlots[i + 1] == null && mechanismComponents[i] != null))
                    {
                        recipeFilled = false;
                    }
                }

                if(recipeFilled)
                {
                    checkMark.Play();
                    itemSlots[0].WatchItem.State = ItemState.Repaired;

                    for (int i = 0; i < mechanismComponents.Count; i++)
                    {
                        EmptySlot(i + 1);
                    }
                }
                else
                {
                    Debug.Log("BAD");
                    isValid = false;
                }
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
}
