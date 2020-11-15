using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

//Refactoring is done! You may enter safely
public class Watch : MonoBehaviour
{
    private Item myItem;
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
        }
    }
    public SpriteRenderer itemRenderer;

    public SpriteRenderer stateRenderer;
    private bool selected;

    public bool isSelected
    {
        get => selected;
        set
        {
            selected = value;
            OnSelectChange();
        }
    }

    private void OnItemChange(Item changedItem)
    {
        itemRenderer.sprite = changedItem.itemImage;
        for (int i = 0; i < myItem.componentsStates.Count; i++)
        {
            myItem.components[i].State = myItem.componentsStates[i];
        }
    }
    //<summary>
    //Watch Item callback that can adjusted if need for the sake of unique items
    //It operates the state images
    //</summary>
    private void OnItemStateChange(Item item)
    {
        if (item.State == ItemState.UnknownState) stateRenderer.sprite = GameManager.instance.unknownImage;
        else if (item.State == ItemState.Unfixable) stateRenderer.sprite = GameManager.instance.unfixableImage;
        else if (item.State == ItemState.Broken) stateRenderer.sprite = GameManager.instance.brokenImage;
        else if (item.State == ItemState.Repaired) stateRenderer.sprite = GameManager.instance.repairedImage;
        else if (item.State == ItemState.ComplexBroken) stateRenderer.sprite = GameManager.instance.complexBrokenImage;
        else stateRenderer.sprite = null;
    }

    private void OnSelectChange()
    {
        if (isSelected == true)
        {
            itemRenderer.material = GameManager.instance.Selected;
        }
        else
        {
            itemRenderer.material = GameManager.instance.NonSelected;
        }
    }
}

