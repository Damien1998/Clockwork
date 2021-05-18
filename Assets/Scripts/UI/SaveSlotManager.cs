using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
   [SerializeField] private SaveSlotDisplay currentSlot;
   [SerializeField]private GameObject SaveSlotTemplate,SaveSlotList;

   private void Awake()
   {
      currentSlot.saveID = SaveController.currentSave.saveID;
      currentSlot.DisplaySaves();
      DisplaySaveSlots();
   }

   public void ClearAllSlots()
   {
      for (int i = 0; i < SaveSlotList.transform.childCount; i++)
      {
         Destroy(SaveSlotList.transform.GetChild(i).gameObject);
      }
   }
   private void DisplaySaveSlots()
   {
      DirectoryInfo fileDirectory = new DirectoryInfo(Application.persistentDataPath);
      for (int i = 0; i < DirCount(fileDirectory); i++)
      {
         var saveSlot = Instantiate(SaveSlotTemplate, SaveSlotList.transform);
         saveSlot.GetComponent<SaveSlotDisplay>().saveID = (int) Char.GetNumericValue(GetSaveName(fileDirectory,i),8);
         saveSlot.GetComponent<SaveSlotDisplay>().myManager = this;
         saveSlot.GetComponent<SaveSlotDisplay>().DisplaySaves();
      }
   }

   public void SelectSaveSlot(int id)
   {
      currentSlot.saveID = id;
      currentSlot.DisplaySaves();
   }
   static string GetSaveName(DirectoryInfo d,int fileIndex)
   {
      FileInfo[] fis = d.GetFiles();
      return fis[fileIndex].Name;
   }
   static long DirCount(DirectoryInfo d)
   {
      long i = 0;
      // Add file sizes.
      FileInfo[] fis = d.GetFiles();
      foreach (FileInfo fi in fis)
      {
         if (fi.Extension.Contains("clk"))
            i++;
      }
      return i;
   }
}
