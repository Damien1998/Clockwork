using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ItemStates",menuName = "ItemStates")]
public class ItemStateDisplay : ScriptableObject
{
    public Material selected, notSelected, outlineBlue, outlineRed, outlineGreen, outlineYellow, outlineOrange;
    public Material stateOutlineBlue, stateOutlineRed, stateOutlineGreen, stateOutlineYellow, stateOutlineOrange;

    /**
     * 0 - UnknownState
     * 1 - Unfixable
     * 2 - Broken
     * 3 - Repaired
     * 4 - ComplexBroken
     * 5 - EmptyState
     * 6 - Soaked
     * 7 - Frozen
     * 8 - Tower
     * 9 - Moving
     * 10 - Death
     * 11 - Repaired (single part)
     * **/
    public Sprite[] itemStates;
}
