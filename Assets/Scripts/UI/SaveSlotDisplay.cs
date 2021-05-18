using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotDisplay : MonoBehaviour
{
   public SaveSlotManager myManager;
   public int saveID;
   public Text saveInfo,saveName;
   public InputField nameField;
   public GameObject trophyImageTemplate,trophyImageList;
   
   public void DisplaySaves()
   {
      if (SaveController.CheckForSaves(saveID))
      {
         var mySave = SaveController.GetSave(saveID);
         if (mySave.levels.Count >0)
         {
            saveInfo.text = $"Poziom: {mySave.levels[mySave.levels.Count-1].name}";
         }
         else
         {
            saveInfo.text = "You didn't Complete Any Level Yet!";
         }

         if (mySave.saveName != "No Save")
         {
            saveName.text = mySave.saveName;
         }
         else
         {
            saveName.text = $"Save {mySave.saveID}";
         }

         for (int i = 0; i < trophyImageList.transform.childCount; i++)
         {
            Destroy(trophyImageList.transform.GetChild(i).gameObject);
         }

         for (int i = 0; i < mySave.completedSideQuests.Count; i++)
         {
            var myTrophy = Resources.Load<Trophy>($"Trophies/Trophy {mySave.completedSideQuests[i].TrophyID}");
            var trophyObject = Instantiate(trophyImageTemplate, trophyImageList.transform);
            trophyObject.GetComponent<Image>().sprite = myTrophy.trophyImage;
         }
      }
      else
      {
         saveInfo.text = "New Save(You didn't Complete Any Level Yet!)";
         saveName.text = "Name";
      }
   }
   private void UpdateNameText(string nameText)
   {
      saveName.text = nameText;
   }

   public void SelectThisSlot()
   {
      myManager.SelectSaveSlot(saveID);
   }
   public void ChangeSaveName(string name)
   {
      SaveController.ChangeSaveName(name);
      SaveController.SaveGame();
      UpdateNameText(name);
   }
   public void DeleteSave()
   {
      if (SaveController.CheckForSaves(saveID))
      {
         saveInfo.text = "Clear Save";
         saveName.text = "No Save!";
         SaveController.DeleteSave(saveID);
      }
   }
}
