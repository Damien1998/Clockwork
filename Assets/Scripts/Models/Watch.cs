using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

//Refactoring is done! You may enter safely
public class Watch : MonoBehaviour
{
    private Item myItem = new Item();
    //A true state that should be only used when item leaves the unknown state
    public ItemState TrueState;

    public Item WatchItem
    {
        get => myItem;
        set
        {
            var oldItem = myItem;
            myItem = value;
            if (myItem != null && oldItem != myItem) myItem.RegisterItemCallback(OnItemStateChange);
            OnItemStateChange(myItem);
            OnItemChange(myItem);
            myItem.SetParameters(value);
        }
    }
    public List<SpriteRenderer> itemRenderer;
    public SpriteRenderer stateRenderer;
    private bool selected;
    private ItemStateDisplay _currentItemDisplay => GameManager.instance.itemStates;

    public bool isPlacedOnWorkbench;

    public void ChangeSortingLayer(string newLayer)
    {
        if(!Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Conveyor")))
        {
            foreach (SpriteRenderer renderer in itemRenderer)
            {
                renderer.sortingLayerName = newLayer;
            }

            stateRenderer.sortingLayerName = newLayer;
        }
        
    }

    public bool isSelected
    {
        get => selected;
        set
        {
            selected = value;
            OnSelectChange();
        }
    }

    public void SetItemType(ItemType type)
    {
        WatchItem.itemType = type;
    }

    private void OnItemChange(Item changedItem)
    {
        for(int i = 0; i < itemRenderer.Count; i++)
        {
            if(changedItem.itemImages.Length > i)
            {
                if (changedItem.itemImages[i] != null)
                {
                    itemRenderer[i].gameObject.SetActive(true);
                    itemRenderer[i].sprite = changedItem.itemImages[i];
                }
                else
                {
                    itemRenderer[i].gameObject.SetActive(false);
                }
            }
            else
            {
                itemRenderer[i].gameObject.SetActive(false);
            }
        }

        //itemRenderer.sprite = changedItem.itemImages;
        for (int i = 0; i < myItem.componentsStates.Count; i++)
        {
            myItem.components[i].State = myItem.componentsStates[i];
        }

        if (changedItem.extraState == ItemState.Death)
        {
            gameObject.tag = "Death";
        }
        else
        {
            gameObject.tag = "Untagged";
        }
    }
    
    IEnumerator StartMoving()
    {
        var VelocityRange = 5;
        while (WatchItem.extraState == ItemState.Moving)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-VelocityRange , VelocityRange), Random.Range(-VelocityRange, VelocityRange)));
            yield return new WaitForSeconds(1.5f);
        }
        yield break;
    }
    
    //<summary>
    //Watch Item callback that can be adjusted if needed for the sake of unique items
    //It operates the state images
    //</summary>
    private void OnItemStateChange(Item item)
    {
        Debug.Log( $"Changing State {item.State}");
        stateRenderer.sprite = _currentItemDisplay.itemStates[(int)item.State];

        switch (item.extraState)
        {
            case ItemState.Frozen:
                stateRenderer.sprite = _currentItemDisplay.itemStates[5];
                break;
            case ItemState.Tower:
                stateRenderer.sprite = _currentItemDisplay.itemStates[5];
                break;
            case ItemState.Moving:
                StartCoroutine(StartMoving());
                break;
            case ItemState.Death:
                var deathWatches = GameObject.FindGameObjectsWithTag("Death");

                //var sameItem = SearchForTheSameItemState(item);

                foreach (var deathWatch in deathWatches)
                {
                    if (deathWatch.GetComponent<Watch>().WatchItem.State != item.State)
                    {
                        deathWatch.GetComponent<Watch>().WatchItem.State = item.State;
                    }
                }
                //stateRenderer.sprite = _currentItemDisplay.itemStates[5];
                // Add Particle Effects Here
                break;
        }
    }


    
    private void OnSelectChange()
    {
        if (isSelected == true)
        {
            //Temporary - no highlighting for items placed on workbenches
            if(!isPlacedOnWorkbench)
            {
                ChangeSortingLayer("ItemsHeld");
                if (myItem.State == ItemState.UnknownState)
                {
                    //Blue
                    foreach (SpriteRenderer renderer in itemRenderer)
                    {
                        renderer.material = _currentItemDisplay.outlineBlue;
                    }
                    //stateRenderer.material = _currentItemDisplay.stateOutlineBlue;
                }
                else if (myItem.State == ItemState.Unfixable)
                {
                    //Red
                    foreach (SpriteRenderer renderer in itemRenderer)
                    {
                        renderer.material = _currentItemDisplay.outlineRed;
                    }
                    //stateRenderer.material = _currentItemDisplay.stateOutlineRed;
                }
                else if (myItem.State == ItemState.Repaired)
                {
                    //Green
                    foreach (SpriteRenderer renderer in itemRenderer)
                    {
                        renderer.material = _currentItemDisplay.outlineGreen;
                    }
                    //stateRenderer.material = _currentItemDisplay.stateOutlineGreen;
                }
                else if ((myItem.State == ItemState.ComplexBroken && (myItem.itemType == ItemType.FullWatch || myItem.itemType == ItemType.FullCasing)))
                {
                    //Purple
                    foreach (SpriteRenderer renderer in itemRenderer)
                    {
                        renderer.material = _currentItemDisplay.outlineYellow;
                    }
                    //stateRenderer.material = _currentItemDisplay.stateOutlineYellow;
                }
                else
                {
                    //Orange
                    foreach (SpriteRenderer renderer in itemRenderer)
                    {
                        renderer.material = _currentItemDisplay.outlineOrange;
                    }
                    //stateRenderer.material = _currentItemDisplay.stateOutlineOrange;
                }
            }
        }
        else
        {
            if(isPlacedOnWorkbench)
            {
                ChangeSortingLayer("ItemsWorkbench");
            }
            else
            {
                ChangeSortingLayer("Items");
            }
            
            foreach (SpriteRenderer renderer in itemRenderer)
            {
                renderer.material = _currentItemDisplay.notSelected;
            }
            stateRenderer.material = _currentItemDisplay.notSelected;
        }
    }
    
    public Item SearchForTheSameItemState(Item originalItem)
    {
        var HighestItem = originalItem;

        if (HighestItem.parentItem != null)
        {
            while (HighestItem.parentItem != null)
            {
                HighestItem = HighestItem.parentItem;
            }    
        }

        return GetSameTypeItem(HighestItem, originalItem); 
    }

    private Item GetSameTypeItem(Item highestItem, Item originalItem)
    {

        if (highestItem.itemImages.ToList().First() != null)
        {
            if (highestItem.extraState == originalItem.extraState &&
                highestItem.itemImages.First().name != originalItem.itemImages.First().name)
            {
                return highestItem;
            }
        }

        if (highestItem.components.Count > 0)
        {
            foreach (var component in highestItem.components)
            {
                var TypeItem = GetSameTypeItem(component, originalItem); 
                if (TypeItem.extraState == originalItem.extraState && TypeItem.itemImages.First().name != originalItem.itemImages.First().name)
                {
                    return TypeItem;
                }
            }
        }
        
        return highestItem;
    }
}

