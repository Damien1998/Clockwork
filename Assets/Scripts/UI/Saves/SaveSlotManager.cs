using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class SaveSlotManager : MonoBehaviour
{
   [SerializeField] private SaveSlotInteractive currentSlot;
   [SerializeField]private GameObject SaveSlotTemplate,SaveSlotList;

   private void OnEnable()
   {
      currentSlot.saveID = SaveController.currentSave.saveID;
      currentSlot.Manager = this;
      currentSlot.DisplaySave();
      DisplaySaveSlots();
   }

   private void ClearAllSlots()
   {
      for (int i = 0; i < SaveSlotList.transform.childCount; i++)
      {
         Destroy(SaveSlotList.transform.GetChild(i).gameObject);
      }
   }
   public void DisplaySaveSlots()
   {
      ClearAllSlots();
      DirectoryInfo fileDirectory = new DirectoryInfo(Application.persistentDataPath);
      for (int i = 0; i < DirCount(fileDirectory); i++)
      {
         var saveSlot = Instantiate(SaveSlotTemplate, SaveSlotList.transform);
         string saveName = GetSaveName(fileDirectory, i);
         saveName = saveName.Remove(saveName.Length - 4, 4);
         saveSlot.GetComponent<SaveSlotDisplay>().saveID = Int32.Parse(saveName.Remove(0,8));
         saveSlot.GetComponent<SaveSlotDisplay>().Manager = this;
         saveSlot.GetComponent<SaveSlotDisplay>().DisplaySave();
      }
   }

   public void SelectSaveSlot(int id)
   {
      currentSlot.saveID = id;
      currentSlot.DisplaySave();
      SaveController.LoadGame(id);
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
