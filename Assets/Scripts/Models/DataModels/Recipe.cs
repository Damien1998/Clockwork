using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Clockwork/Recipe")]
public class Recipe : ScriptableObject
{
    public List<Item> Items = new List<Item>();
    //Recipe result item
    public Item result;

    public void SetParameters(Item newResult, List<Item> newComponents)
    {
        result = newResult;
        Items = newComponents;
    }
}
