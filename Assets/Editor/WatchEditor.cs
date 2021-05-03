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
                ChangeWatchState(ItemState.Broken);
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Unfixable"))
            {
                ChangeWatchState(ItemState.Unfixable);
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Unknown"))
            {
                ChangeWatchState(ItemState.UnknownState);
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
            if(GUILayout.Button("Set State To Repaired"))
            {
                ChangeWatchState(ItemState.Repaired);
                Debug.Log("The State of Item is now " + target.WatchItem.State);
            }
        }
        private void ChangeWatchState(ItemState _itemState)
        {
            var itemState = _itemState;

            target.WatchItem.State = itemState ;
            target.WatchItem.trueState = itemState ;
            target.TrueState = itemState;
        }
    }
}