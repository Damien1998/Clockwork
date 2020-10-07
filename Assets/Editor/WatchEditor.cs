using System;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Watch)), ExecuteInEditMode]
    public class WatchEditor : UnityEditor.Editor
    {
        private new Watch target;

        private void OnEnable()
        {
            target = (Watch) base.target;
        }

        public override void OnInspectorGUI () {
            DrawDefaultInspector();
            if(GUILayout.Button("Set State To Broken"))
            {
                target.WatchItem.State = ItemState.Broken;
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Unfixable"))
            {
                target.WatchItem.State = ItemState.Unfixable;
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Unknown"))
            {
                target.WatchItem.State = ItemState.UnknownState;
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Repaired"))
            {
                target.WatchItem.State = ItemState.Repaired;
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
        }
    }
}