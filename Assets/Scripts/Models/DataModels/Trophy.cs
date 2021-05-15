using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Trophy",menuName = "Trophy")]
public class Trophy : ScriptableObject
{
    public Sprite trophyImage;
    public string trophyName;
    public string Description;
}
