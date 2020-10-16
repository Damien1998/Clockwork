using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemState { Broken,Unfixable,Repaired,UnknownState,EmptyState};

[CreateAssetMenu(fileName = "New Item",menuName = "Item")]
public class Item : ScriptableObject
{
    private ItemState state = ItemState.UnknownState;
    private Action<Item> itemStateChangeCb;
    
    //A true state that should be only used when item leaves the unknown state
    public ItemState TrueState;
    
    public int itemID;
    public Sprite itemImage;
    public List<Item> components = new List<Item>();
    public List<ItemState> componentsStates = new List<ItemState>();
    //<summary>
    //A Getter and setter to check if the type was changed 
    //If it was it will perform itemStateChange Callback 
    //</summary>
    public ItemState State
    {
        get => state;
        set
        {
            var oldState = state;
            state = value;
            if (itemStateChangeCb != null && oldState != state) itemStateChangeCb(this);
        }
    }
    //<summary>
    //Function to Set Parameters when creating the item
    //It can be used when creating new items from scripts 
    //</summary>
    public void SetParameters(Item templateitem)
    {
        this.state = templateitem.state;
        this.components = templateitem.components;
        this.itemID = templateitem.itemID;
        this.itemImage = templateitem.itemImage;
    }
    public void RegisterItemCallback(Action<Item> callback)
    {
        itemStateChangeCb += callback;
    }
    public void UnRegisterItemCallback(Action<Item> callback)
    {
        if (itemStateChangeCb != null) itemStateChangeCb -= callback;
    }
}
