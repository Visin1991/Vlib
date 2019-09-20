using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public static class VPrefab
    {

        public static GameObject InstantiatePrefabAtPath(string directory, string _gameObjectName,string extention = ".prefab")
        {

            string prefabPath = directory + "/" + _gameObjectName + extention;
            Debug.Log("Prefab Path : " + prefabPath);
            GameObject obj = Object.Instantiate((GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject)));
            obj.name = _gameObjectName;
            return obj;
        }

        public static void CreateNewPrefabFromGameObject(GameObject obj, string directory)
        {
            string assetPath = directory + "/" + obj.name + ".prefab";
            if (AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "prefab已经存在, 是否要覆盖它?", "Yes", "No"))
                {
                    CreateNewPrefab(obj, assetPath);
                }
            }
            else
            {
                CreateNewPrefab(obj, assetPath);
            }
        }

        public static void CreateNewPrefab(GameObject obj, string localPath)
        {
            Object prefab = PrefabUtility.SaveAsPrefabAsset(obj, localPath);
        }

    }

}