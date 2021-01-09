using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemStates",menuName = "ItemStates")]
public class ItemStateDisplay : ScriptableObject
{
    public Material selected, notSelected;
    public Sprite[] itemStates;
}
