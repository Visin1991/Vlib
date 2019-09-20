using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace V
{
    public class PrintGUID_TargetPath : EditorWindow
    {
        [MenuItem("V/Debug/GUID")]
        static void CreateWindow()
        {
            PrintGUID_TargetPath window = EditorWindow.GetWindow<PrintGUID_TargetPath>();
            window.maxSize = new Vector2(300,50);
            window.Show();
        }

        string guid;

        private void OnGUI()
        {
            GUILayout.BeginHorizontal();
           
            GUILayout.Label("GUID : ",GUILayout.MaxWidth(50.0f));
            guid = GUILayout.TextField(guid);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Print GUID Target Path"))
            {
                Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
            }         
        }
    }
}