using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotInteractive : SaveSlotDisplay
{
    public InputField nameField;
    [SerializeField]
    private Animator myAnimator;

    private void UpdateNameText(string nameText)
    {
        saveName.text = nameText;
    }
    public void DeleteSave()
    {
        if (SaveController.CheckForSaves(saveID))
        {
            DisplayDelete();
            saveInfo.text = "Clear Save";
            saveName.text = "No Save!";
            SaveController.DeleteSave(saveID);
            Manager.DisplaySaveSlots();
            DisplaySave();           
        }
    }
    public void ChangeSaveName(string name)
    {
        SaveController.ChangeSaveName(name);
        SaveController.SaveGame();
        UpdateNameText(name);
        Manager.DisplaySaveSlots();
    }

    public void DisplayDelete()
    {
        myAnimator.SetTrigger("Delete");
    }

    public void DisplaySelect()
    {
        myAnimator.SetTrigger("Select");
    }
}
