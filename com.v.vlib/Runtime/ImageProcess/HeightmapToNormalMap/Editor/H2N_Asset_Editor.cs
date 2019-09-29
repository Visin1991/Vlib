using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace V
{
    [CustomEditor(typeof(H2N_Asset))]
    public class H2N_Asset_Editor : Editor
    {
        H2N_Asset h2nAsset;

        RenderTexture rTex;
        GUIContent texContent;

        private void OnEnable()
        {
            serializedObject.Update();
            h2nAsset = target as H2N_Asset;
            rTex = new RenderTexture(256, 256, 0);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (h2nAsset.setting.destinationMap == null)
            {
                if (GUILayout.Button("Create destination texture"))
                {
                    string targetPath = AssetDatabase.GetAssetPath(h2nAsset).Substring(0, AssetDatabase.GetAssetPath(h2nAsset).Length - 5) + "png";
                    h2nAsset.setting.destinationMap = WriteTextureToDisk(h2nAsset.setting.CreateTexture(),targetPath);
                    EditorUtility.SetDirty(h2nAsset);
                }
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Apply"))
            {
                UpdateDestMap(h2nAsset.setting);
            }
            EditorGUILayout.EndHorizontal();

            if (serializedObject.hasModifiedProperties)
            {
                Debug.Log("Change.........");
                serializedObject.ApplyModifiedProperties();
            }

        }

        public void UpdateDestMap(H2N_Setting setting)
        {
            if (setting.destinationMap == null)
                return;
            var path = AssetDatabase.GetAssetPath(setting.destinationMap);
            WriteTextureToDisk(setting.CreateTexture(), path);
        }

        Texture2D WriteTextureToDisk(Texture2D textureCache,string targetPath)
        {
            if (textureCache == null) { return null; }
            System.IO.File.WriteAllBytes(targetPath, ImageConversion.EncodeToPNG(textureCache));
            AssetDatabase.ImportAsset(targetPath);
            Object.DestroyImmediate(textureCache);
            return AssetDatabase.LoadMainAssetAtPath(targetPath) as Texture2D;
        }

    }
}
