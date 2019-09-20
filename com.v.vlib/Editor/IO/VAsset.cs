using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public static class VAsset
    {
        public static void CreateAsset(Object obj,string path)
        {
            if (AssetDatabase.LoadAssetAtPath(path, typeof(Object)))
            {
                if (EditorUtility.DisplayDialog("", path + " 已经存在，你是否覆盖它？", "是的，给大爷我覆盖它", "不好意思,取错名字了"))
                {
                    AssetDatabase.CreateAsset(obj, path);
                }
            }
            else {
                AssetDatabase.CreateAsset(obj, path);
            }      
        }

        public static bool IsAssetExsit(string path)
        {
            return AssetDatabase.LoadAssetAtPath(path, typeof(Object))!=null;
        }
    }
}
