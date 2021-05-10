using System;
using System.Collections;
using System.Collections.Generic;
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
         SaveController.LoadGame(saveID);
         saveInfo.text = "LoadedSave Completed Levels : " +SaveController.CompletedLevels();
         nameField.text = SaveController.currentSaveName();
         nameField.interactable = true;
         achievments.text = "None , yet";
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
