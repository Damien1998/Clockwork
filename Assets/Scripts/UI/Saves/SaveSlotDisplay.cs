using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotDisplay : MonoBehaviour
{
   private SaveSlotManager myManager;
   public int saveID;
   public TextMeshProUGUI saveInfo,saveName;
   public GameObject trophyImageTemplate,trophyImageList;

   public SaveSlotManager Manager { get  => myManager;
      set =>  myManager = value;
   }

   public void DisplaySave()
   {
      if (SaveController.CheckForSaves(saveID))
      {
         Debug.Log(saveInfo);

         var mySave = SaveController.GetSave(saveID);
         Debug.Log(mySave.levels[mySave.levels.Count-1].name);

         // Checking Completed Levels
         if (mySave.levels.Count >0)
         {
            saveInfo.text = $"Poziom: {mySave.levels[mySave.levels.Count-1].name}";
         }
         else
         {
            saveInfo.text = "You didn't Complete Any Level Yet!";
         }

         saveName.text = $"{mySave.saveName}";

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
         saveInfo.text = "New Save";
         saveName.text = "Name";
      }
   }
   public void SelectThisSlot()
   {
      myManager.SelectSaveSlot(saveID);
   }


}
