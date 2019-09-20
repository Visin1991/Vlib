using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class GraphicsUtili
    {
        public static MaterialInfo[] Get_MaterialInfo(MeshRenderer mr,string pbrTextureName)
        {

            if (mr == null) { return null; }
            Material[] materials = mr.sharedMaterials;

            MaterialInfo[] infos = new MaterialInfo[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                Material mat = materials[i];
                infos[i] = new MaterialInfo();
                if (mat != null)
                {
                    infos[i].matName = mat.name;
                    infos[i].Shadername = mat.shader.name;

                    if (mat.HasProperty("_MainTex"))
                    {
                        Texture mainTex = mat.GetTexture("_MainTex");
                        infos[i].path_D = UnityEditor.AssetDatabase.GetAssetPath(mainTex);
                    }
                    if (mat.HasProperty("_BumpMap"))
                    {
                        Texture bumpTex = mat.GetTexture("_BumpMap");
                        infos[i].path_N = UnityEditor.AssetDatabase.GetAssetPath(bumpTex);
                    }
                    if (mat.HasProperty(pbrTextureName))
                    {
                        Texture pbrTex = mat.GetTexture(pbrTextureName);
                        infos[i].path_P = UnityEditor.AssetDatabase.GetAssetPath(pbrTex);
                    }
                }
                else
                {
                    return null;
                }
            }
            
            return infos;
        }
    }

    public class MaterialInfo
    {
        public string matName;
        public string Shadername;
        public string path_D;
        public string path_P;
        public string path_N;
    }
}
