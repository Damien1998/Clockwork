using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Watch List",menuName = "watchlist")]
public class WatchList : ScriptableObject
{ 
    public List<GameObject> listOfWatches = new List<GameObject>();
}
