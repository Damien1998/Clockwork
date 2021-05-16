using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotDisplay : MonoBehaviour
{
   public int saveID;
   public Text saveInfo;
   public InputField nameField;
   public Text achievments;

   private void Awake()
   {
      DisplaySaves();
   }

   public void DisplaySaves()
   {
      if (SaveController.CheckForSaves(saveID))
      {
         var mySave = SaveController.GetSave(saveID);
         Debug.Log(mySave.levels.Count);
         if (mySave.levels.Count >0)
         {
            saveInfo.text = $"{mySave.levels[mySave.levels.Count].name}";
         }
         else
         {
            saveInfo.text = "None";
         }
         nameField.text = mySave.saveName;
         nameField.interactable = true;
         StringBuilder myTrophies = new StringBuilder();
         for (int i = 0; i < mySave.completedSideQuests.Count; i++)
         {
            var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {mySave.completedSideQuests[i].TrophyID}");
            myTrophies.Append($"{myTrophy.trophyName} ,");
         }
         achievments.text = myTrophies.ToString();
      }
      else
      {
         saveInfo.text = "Clear Save";
         nameField.text = "Name";
         nameField.interactable = false;
         achievments.text = "None"; 
      }
   }
   public void ChangeSaveName(string name)
   {
      SaveController.LoadGame(saveID);
      SaveController.ChangeSaveName(name);
      SaveController.SaveGame();
   }
   public void DeleteSave()
   {
      if (SaveController.CheckForSaves(saveID))
      {
         saveInfo.text = "Clear Save";
         nameField.text = "Name";
         achievments.text = "None";
         SaveController.DeleteSave(saveID);
      }
   }
}
