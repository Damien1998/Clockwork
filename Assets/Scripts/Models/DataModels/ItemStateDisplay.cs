using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemStates",menuName = "ItemStates")]
public class ItemStateDisplay : ScriptableObject
{
    public Material selected, notSelected, outlineBlue, outlineRed, outlineGreen, outlineYellow, outlineOrange;
    public Material stateOutlineBlue, stateOutlineRed, stateOutlineGreen, stateOutlineYellow, stateOutlineOrange;
    public Sprite[] itemStates;
}
