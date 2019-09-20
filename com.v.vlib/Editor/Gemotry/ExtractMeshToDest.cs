using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public static class ExtractMeshToDest
    {
        [MenuItem("V/GameObject/提取Mesh到指定目录")]
        public static void Do()
        {
            GameObject obj = Selection.activeGameObject;
            if (obj != null)
            {
                string directory = EditorUtility.OpenFolderPanel("Save", "Assets/KGame", "");
                if (!directory.Contains(VIO.WorkSpace())) { return; }
                VIO.CopyMeshToDst(obj.GetComponent<MeshFilter>(), directory, false);
                AssetDatabase.Refresh();
            }
        }
    }
}
