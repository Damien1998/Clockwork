using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Recipe")]
public class Recipe : ScriptableObject
{
    public List<Item> Items = new List<Item>();
    //Recipe result item
    public Item result;

    //Required part IDs
}
