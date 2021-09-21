using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

//Refactoring is done! You may enter safely
public class WorkbenchPrecise : Workbench
{
    private Item watchToDrop;

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


    // Update is called once per frame
    void Update()
    {
        if (isOperated && itemSlots[0] != null)
        {
            if(!workParticles.isEmitting)
            {
                workParticles.Play();
                SoundManager.PlaySound(SoundManager.Sound.WorkAdvanced);
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

        KeepSlotsInPlace(true);
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
                else if (itemSlots[0].WatchItem.State != ItemState.EmptyState && itemSlots[0].WatchItem.itemType == ItemType.FullMechanism)
                {
                    //endParticles.Play();
                    itemSlots[0].TrueState = ItemState.EmptyState;
                    itemSlots[0].WatchItem.SetAllStates(ItemState.EmptyState);
                    itemSlots[0].WatchItem.itemType = ItemType.EmptyMechanism;
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
        else
        {
            if ((itemSlots[0].WatchItem.itemType == ItemType.Mechanism  && itemSlots[0].WatchItem.State == ItemState.Repaired) || itemSlots[0].WatchItem.itemType == ItemType.EmptyMechanism)
            {
                Debug.Log("Hlep");// there is no help, only salvation

                var recipeFilled = CheckForAllMechanismComponents(itemSlots);

                if(recipeFilled)
                {
                    Watch newWatch = WatchTemplate.GetComponent<Watch>();
                    Watch newPart = Instantiate(newWatch, dropLocation.position, Quaternion.identity);
                    newPart.WatchItem = watchToDrop;
                    newPart.WatchItem.SetAllStates(ItemState.Repaired);

                    RecipeListView.RemoveCheckForRecipes(newPart);

                    EmptySlot(0);
                    EmptySlot(1);
                    EmptySlot(2);

                    checkMark.Play();
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
    bool CheckForAllMechanismComponents(Watch[] slots)
    {
        var filledSlots = slots.Count(watch => watch != null);

        var correctComponents = 0;
        var myParentItem = slots[0].WatchItem.parentItem;
        if (slots[0].WatchItem.itemType == ItemType.EmptyMechanism)
        {
            myParentItem = slots[1].WatchItem.parentItem;
            Debug.Log("Swap");
        }

        for (int j = 0; j < filledSlots; j++)
        {
            if (slots[j].WatchItem.itemType == ItemType.EmptyMechanism)
            {
                Debug.Log("EmptyCheck");
                correctComponents++;
            }
            else
            {
                if (slots[j].WatchItem.parentItem == myParentItem && slots[j].WatchItem.State == ItemState.Repaired && slots[j].WatchItem.itemType == ItemType.Mechanism)
                {
                    correctComponents++;
                }
                else
                {
                    correctComponents--;
                }
            }
        }

        if (correctComponents >= myParentItem.components.Count+1)
        {
            watchToDrop = new Item();
            watchToDrop = myParentItem;
            SoundManager.PlaySound(SoundManager.Sound.ClockCompleted);
            return true;
        }
        SkipLoop: ;

        return false;
    }
}
