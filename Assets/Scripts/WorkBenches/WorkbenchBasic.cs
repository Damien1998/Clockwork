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
                SoundManager.PlaySound(SoundManager.Sound.WorkSimple);
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

        KeepSlotsInPlace(true);
    }

    public override void PlaceItem(Watch itemToPlace)
    {
        Debug.Log(itemToPlace.WatchItem.itemType);
        if (itemToPlace.WatchItem.itemType == ItemType.Mechanism) return;

        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] == null)
            {
                itemToPlace.ChangeSortingLayer("ItemsWorkbench");
                itemToPlace.transform.position = slotPositions[i].position;
                itemToPlace.isPlacedOnWorkbench = true;
                itemSlots[i] = itemToPlace;
                slotsFilled = i + 1;
                break;
            }
        }
    }


    protected override void DropItems()
    {
        slotsFilled = 0;
        bool isValid = true;
        //Here the items are combined or broken down according to recipes

        //If there is only one item, it is broken down
        //This doesn't accept mechanisms
        if (itemSlots[1] == null && itemSlots[0].WatchItem.State != ItemState.UnknownState)
        {
            if(itemSlots[0].WatchItem.State == ItemState.ComplexBroken && itemSlots[0].WatchItem.itemType != ItemType.FullMechanism)
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

                var recipeFilled = CheckForAllComponents(itemSlots);

                if (recipeFilled)
                {
                    //endParticles.Play();
                    Watch newWatch = WatchTemplate.GetComponent<Watch>();
                    Watch newPart = Instantiate(newWatch, dropLocation.position, Quaternion.identity);
                    newPart.WatchItem = watchToDrop;
                    newPart.WatchItem.SetParameters(watchToDrop);
                    newPart.WatchItem.SetAllStates(ItemState.Repaired);

                    RecipeListView.RemoveCheckForRecipes(newPart);

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

    ItemType CheckForQuestWatch()
    {
        var filledSlots = 0;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] != null)
            {
                filledSlots++;
            }
        }
        for (int i = 0; i < filledSlots; i++)
        {
            if (itemSlots[i].WatchItem.itemType == ItemType.QuestWatch)
            {
                return ItemType.QuestWatch;
            }
        }
        return ItemType.FullWatch;
    }

    bool CheckForAllComponents(Watch[] slots)
    {
        var filledSlots = slots.Count(watch => watch != null);

        var correctComponents = 0;
        var fixedItem = new Item();
        if (slots[0].WatchItem.itemType != ItemType.FullMechanism)
        {
            fixedItem = slots[0].WatchItem.parentItem;
        }
        else
        {
            fixedItem = slots[1].WatchItem.parentItem;
        }

        if (fixedItem == null) { return false;}

        for (int j = 0; j < filledSlots; j++)
        {
            var currentItemPart = slots[j].WatchItem;
            if(currentItemPart.State == ItemState.Repaired && currentItemPart.itemType != ItemType.FullMechanism)
            {
                if (currentItemPart.parentItem == fixedItem)
                {
                    correctComponents++;
                }
            }
            else
            {
                if (HasMechanismIn(fixedItem))
                {
                    if (currentItemPart.State == ItemState.Repaired)
                    {
                        correctComponents++;
                    }
                }
                else
                {
                    correctComponents--;
                }
            }
        }

        if (correctComponents >= fixedItem.components.Count)
        {
            watchToDrop = new Item();
            watchToDrop.SetParameters(fixedItem);
            SoundManager.PlaySound(SoundManager.Sound.ClockCompleted);
            return true;
        }

        return false;
    }

    private bool HasMechanismIn(Item parentItem)
    {
        return parentItem.components.Any(component => component.itemType == ItemType.FullMechanism);
    }
}
