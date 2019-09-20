using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace V
{
    public class CubeMapMaker : EditorWindow
    {
        
        [MenuItem("V/截取天空盒")]
        static void CreateCubeMap()
        {
            Debug.Log("开始截取天空盒");
            var window = EditorWindow.GetWindow<CubeMapMaker>();
            window.titleContent = new GUIContent("CubeMap Maker");
            window.minSize = new Vector2(400,100);
            window.Show();
        }

        [System.Flags]
        public enum CubeSize
        {
            X64 = 2 << 5,
            X128 = 2 << 6,
            x256 = 2 << 7,
            x512 = 2 << 8
        }

        public GameObject referencePosition;
        public CubeSize cubeSize = CubeSize.X128;
        private void OnGUI()
        {

            //GUILayout.ObjectField()
            GUILayout.BeginHorizontal();
            GUILayout.Label("截取位置");
            referencePosition = EditorGUILayout.ObjectField(referencePosition, typeof(GameObject),true) as GameObject;
            //cubeSize = (CubeSize)EditorGUILayout.EnumPopup(cubeSize);

            GUILayout.EndHorizontal();

            if (GUILayout.Button("截取天空盒"))
            {
                if (referencePosition)
                {
                    Make1DCubeMap_X();
                }
                else {
                    EditorUtility.DisplayDialog("Warning","请选择截取位置","OK");
                }
            }
        }
        string saveToPath = "";

        private void OnEnable()
        {
            saveToPath = Path.GetFullPath("Assets").Replace('\\', '/');
        }

        void Make1DCubeMap_X()
        {
            saveToPath = EditorUtility.SaveFilePanel("储存CubeMap", saveToPath, "_Cube"+ cubeSize.ToString(), "exr");
            if (saveToPath == null) { Debug.Log("Empty Path Return"); return; }
            if (saveToPath.Length <= 0) { Debug.Log("Cancle Return"); return; }
            Cubemap cubemap = new Cubemap((int)cubeSize, TextureFormat.RGBAFloat,false);
            GameObject go = new GameObject("CubemapCamera");
            Camera cameara = go.AddComponent<Camera>();
            go.transform.position = referencePosition.transform.position;
            go.transform.rotation = Quaternion.identity;
          
            cameara.RenderToCubemap(cubemap);
            DestroyImmediate(go);

            //Debug.Log(Application.dataPath + "/" + cubemap.name + "_PositiveX.png");
            var tex = new Texture2D(cubemap.width * 6, cubemap.width, TextureFormat.RGBAFloat, false);

            int widthOffset = 0;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.PositiveX), cubemap.width, widthOffset);
            widthOffset += cubemap.width;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.NegativeX), cubemap.width, widthOffset);
            widthOffset += cubemap.width;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.PositiveY), cubemap.width, widthOffset);
            widthOffset += cubemap.width;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.NegativeY), cubemap.width, widthOffset);
            widthOffset += cubemap.width;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.PositiveZ), cubemap.width, widthOffset);
            widthOffset += cubemap.width;
            Texture2D_SetPixel(ref tex, cubemap.GetPixels(CubemapFace.NegativeZ), cubemap.width, widthOffset);
            var bytes = tex.EncodeToEXR();
            File.WriteAllBytes(saveToPath, bytes);

            DestroyImmediate(tex);
            EditorUtility.DisplayDialog("截取成功", "CubeMap截取成功，在目录： " + saveToPath, "ok");
            AssetDatabase.Refresh();
        }

        void Texture2D_SetPixel(ref Texture2D tex,Color[] colors,int chunkSize,int colOffset)
        {
            int  y, x;  
             y = x = 0;
            for (int index = 0; index < colors.Length; index++)
            {
                Color c = colors[index];
                c = c.linear;
                x += colOffset;
                tex.SetPixel(x, y, c);
                x = index % chunkSize;
                y = (chunkSize - 1) - (index / chunkSize);                
            }
        }

    }
}
