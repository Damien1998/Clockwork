using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum ItemState { Broken,Unfixable,Repaired,UnknownState,ComplexBroken,EmptyState};

[CreateAssetMenu(fileName = "New Item",menuName = "Item")]
public class Item 
{
    private ItemState state;
    private Action<Item> itemStateChangeCb;

    public ItemState trueState;
    public int itemID;
    public Sprite[] itemImages = new Sprite[1];
    public List<Item> components = new List<Item>();
    public Item parentItem;
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
        this.trueState = templateitem.trueState;
        this.state = templateitem.state;
        this.components = templateitem.components;
        this.itemID = templateitem.itemID;
        this.parentItem = templateitem.parentItem;

        itemImages = new Sprite[templateitem.itemImages.Length];
        for(int i = 0; i < templateitem.itemImages.Length; i++)
        {
            itemImages[i] = templateitem.itemImages[i];
        }

        this.trueState = templateitem.trueState;
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
