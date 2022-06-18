using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterColor
{
    public string characterName;
    public Color nameColor;

    public override string ToString() => $"#{ColorUtility.ToHtmlStringRGB(nameColor)}";
}

[CreateAssetMenu(fileName = "New Character Color Set", menuName = "Character Color Set")]
public class DialogueNameColors : ScriptableObject
{
    public CharacterColor[] characterColors;
}
