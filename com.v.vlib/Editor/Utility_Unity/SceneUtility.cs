using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace V
{
    public static class SceneUtility
    {
        [MenuItem("V/TestNewScene")]
        public static void NewScene()
        {
            if (VDialog.DisplayDialog("是否需要建立新的Scene"))
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
            }
        }

        public static HashSet<GameObject> FindAllPrefabsInScene()
        {
            Transform[] transforms = GameObject.FindObjectsOfType<Transform>();
            HashSet<GameObject> prefabs = new HashSet<GameObject>();
            for (int i = 0; i < transforms.Length; i++)
            {
                GameObject root = PrefabUtility.GetPrefabInstanceHandle(transforms[i].gameObject) as GameObject;
                bool prefab =  root != null;
                if (!prefab) { continue; }
                prefabs.Add(root);
            }
            return prefabs;
        }

        public static void FindAllPrefabsInScene(System.Action<GameObject,bool> action)
        {
            Transform[] transforms = GameObject.FindObjectsOfType<Transform>();
            HashSet<string> prefabs = new HashSet<string>();
            HashSet<GameObject> roots = new HashSet<GameObject>();
         
            for (int i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] == null)
                {
                    continue;
                }

                Object prefab = PrefabUtility.GetCorrespondingObjectFromSource(transforms[i].gameObject);
                bool isprefab = prefab != null;
                if (!isprefab) { continue; }
              
                GameObject obj = PrefabUtility.GetPrefabInstanceHandle(transforms[i].gameObject) as GameObject;
                
                //First Time Call back the Prefab
                if (prefabs.Add(AssetDatabase.GetAssetPath(prefab)))
                {
                    roots.Add(obj); // Tag the Root
                    if (action != null) { action(obj,true); }
                }
                else
                {
                    //Tag the Root
                    if (roots.Add(obj))
                    {
                        if (action != null) { action(obj, false); }
                    }
                }
            }
        }

        static class VDialog
        {
            public static bool DisplayDialog(string message)
            {
                return EditorUtility.DisplayDialog("V", message, "Yes", "Cancel");
            }
        }

        static class VFile
        {
            public static void SaveFile(System.Action<string> action, string extention = "txt", string defaultName = "Non")
            {
                string path = EditorUtility.SaveFilePanel("V", "", defaultName, extention);
                if (path.Length != 0)
                {
                    action(path);
                }
            }

            public static string SaveFile(string extention = "txt", string defaultName = "Non")
            {
                return EditorUtility.SaveFilePanel("V", "", defaultName, extention);
            }

        }
    }

    [CustomEditor(typeof(VMegicBillbord))]
    public class VMegicBillbordInspector : Editor {

        public enum Mode
        {
            View,
            Paint,
            Edit,
            Erase
        }

        public Mode selectedMode;
        public Mode currentMode;

        private void OnSceneGUI()
        {
            DrawModeGUI();
        }

        private void DrawModeGUI()
        {
            List<string> modes = Enum.GetListStringFromEnum<Mode>();
            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10f, 10f, 360, 40f));
            selectedMode = (Mode)GUILayout.Toolbar((int)currentMode, modes.ToArray(), GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
            Handles.EndGUI();

        }
    }

    public class SceneHelper
    {
        public void __SelectComponent(System.Type T)
        {
            Selection.activeObject = Object.FindObjectOfType(T);
        }
    }




}
