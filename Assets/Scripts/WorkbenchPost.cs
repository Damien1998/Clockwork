using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//
public class WorkbenchPost : Workbench
{
    private bool invalidItemInside, isEmpty;

    [SerializeField] private ParticleSystem checkMark, crossMark;

    // Start is called before the first frame update
    void Start()
    {
        //This workbench only has one slot
        //Use multiple if you want more at the same time
        numberOfSlots = 1;
        itemSlots = new Watch[1];
        isEmpty = true;
        workTimer = workTimerBase;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEmpty)
        {
            Work();
        }
        
        if(workTimer <= 0)
        {
            SoundManager.PlaySound(SoundManager.Sound.WorkPostal);
            endParticles.Play();
            checkMark.Play();
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
            isEmpty = true;
        }

        //I made a different check for this
        //There will be different particle fx for dropping valid and invalid items
        if(invalidItemInside && workTimer <= (workTimerBase / 10))
        {
            endParticles.Play();
            crossMark.Play();
            timerDisplay.gameObject.SetActive(false);
            DropItems();
            workTimer = workTimerBase;
            isEmpty = true;
        }
    }

    //Fixing the item is done when the item is placed on the workbench

    public override void PlaceItem(Watch itemToPlace)
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            if (itemSlots[i] == null)
            {
                if (itemToPlace.WatchItem.State == ItemState.Unfixable)
                {
                    invalidItemInside = false;
                    itemToPlace.WatchItem.State = ItemState.Repaired;
                }
                else
                {
                    invalidItemInside = true;
                }

                isEmpty = false;
                itemSlots[i] = itemToPlace;
                itemToPlace.gameObject.SetActive(false);
                break;
            }
        }        
    }
}
