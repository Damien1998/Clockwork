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
   private SaveController _saveController;

   private void Start()
   {
      _saveController = new SaveController();
      if (_saveController.CheckForSaves(saveID))
      {
         _saveController.LoadGame(saveID);
         saveInfo.text = "LoadedSave Completed Levels : " +_saveController.CompletedLevels();
         nameField.text = _saveController.currentSaveName();
         achievments.text = "None , yet";
      }
      else
      {
         saveInfo.text = "Clear Save";
         nameField.text = "Name";
         achievments.text = "None"; 
      }
   }

   public void DeleteSave()
   {
      if (_saveController.CheckForSaves(saveID))
      {
         saveInfo.text = "Clear Save";
         nameField.text = "Name";
         achievments.text = "None";
         _saveController.DeleteSave(saveID);
         GameManager.instance.SetSaveController(_saveController);
      }
   }
}
