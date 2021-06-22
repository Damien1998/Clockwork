using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyTableDisplay : MonoBehaviour
{
   private TrophyTable myTrophyTable;

   public TrophyTable CurrentTrophyTable
   {
      get => myTrophyTable;
      set
      {
         if (value != myTrophyTable) myTrophyTable = value;
      }
   }

   public int TrophyID;

   public void PickTrophy()
   {
      myTrophyTable.DisplayTrophy(TrophyID);
      Debug.Log("VAR9");
   }
}
