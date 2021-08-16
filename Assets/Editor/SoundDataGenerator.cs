using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SoundController)), ExecuteInEditMode]
public class SoundDataGenerator : UnityEditor.Editor
{
    private new SoundController target;

    private void OnEnable()
    {
        target = (SoundController)base.target;
    }
    public override void OnInspectorGUI () {
        DrawDefaultInspector();
        if(GUILayout.Button("Generate Sounds"))
        {
            var sounds = Resources.LoadAll("Sounds", typeof(SoundModel));

            List<SoundModel> soundsList = new List<SoundModel>();

            foreach (var sound in sounds)
            {
                soundsList.Add((SoundModel)sound);
            }

            target.SetSounds(soundsList);
        }
    }
}
