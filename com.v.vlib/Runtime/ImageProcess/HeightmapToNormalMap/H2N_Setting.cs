using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [System.Serializable]
    public class H2N_Setting 
    {

        public Texture heightMap;

        public H2N_Generator normalMapSetting;

        public HeightMap_BlurFilter heightMapSetting;

        public HeightAndNormalCombiner combiner;


        public Texture2D destinationMap;
        public int customDestinationWidth;
        public int customDestinationHeight;

        public bool HasMap(Texture tex)
        {
            return heightMap == tex;
        }

        public int DestinationWidth
        {
            get
            {
                if (customDestinationWidth > 0)
                    return customDestinationWidth;

                int a = heightMap != null ? heightMap.width : 0;
                return a;
            }
        }

        public int DestinationHeight
        {
            get
            {
                if (customDestinationHeight > 0)
                    return customDestinationHeight;

                int a = heightMap != null ? heightMap.height : 0;
                return a;
            }
        }

        //public void UpdateDestMap()
        //{
        //    if (destinationMap == null)
        //        return;

        //    //Replase the Texture
        //    var path = AssetDatabase.GetAssetPath(destinationMap);
        //    CreateTexture(path);
        //}

        public Texture2D CreateTexture()
        {
            RenderTextureDescriptor rd = new RenderTextureDescriptor(DestinationWidth, DestinationHeight)
            {
                sRGB = false
            };

            var compliteRT = RenderTexture.GetTemporary(rd);
            Apply(compliteRT);
            Texture2D saveTex = new Texture2D(DestinationWidth, DestinationHeight, TextureFormat.RGBA32, false, true);
            RenderTexture.active = compliteRT;
            saveTex.ReadPixels(new Rect(0, 0, compliteRT.width, compliteRT.height), 0, 0);
            saveTex.Apply();
            RenderTexture.ReleaseTemporary(compliteRT);
            return saveTex;
        }

        public void Apply(RenderTexture destination)
        {
            RenderTextureDescriptor rt_descriptor = new RenderTextureDescriptor(destination.width, destination.height)
            {
                sRGB = false
            };

            RenderTexture normalRT = RenderTexture.GetTemporary(rt_descriptor);

            if (combiner.mode != HeightAndNormalMapСombinerMode.ALL_height)
            {
                if (heightMap != null)
                {
                    Debug.Log("Generate Normal From Height Map");
                    normalMapSetting.Apply(heightMap, normalRT);
                }
            }


            RenderTexture heightRT = RenderTexture.GetTemporary(rt_descriptor);
            {
                if (heightMap != null)
                {
                    heightMapSetting.Apply(heightMap, heightRT);
                }
            }

            combiner.Combine(normalRT, heightRT, destination);

            RenderTexture.ReleaseTemporary(normalRT);
            RenderTexture.ReleaseTemporary(heightRT);
        }

    }

    public enum HeightAndNormalMapСombinerMode { RGB_normal, RG_normal_B_height, ALL_height }

    [System.Serializable]
    public class HeightAndNormalCombiner
    {
        public HeightAndNormalMapСombinerMode mode;
        public void Combine(Texture normalMap,Texture heightMap,RenderTexture destination)
        {
            switch (mode)
            {
                case HeightAndNormalMapСombinerMode.RGB_normal:
                    Graphics.Blit(normalMap, destination);
                    break;
                case HeightAndNormalMapСombinerMode.RG_normal_B_height:
                    PutHeightMapToBlueChanel(normalMap, heightMap, destination);
                    break;
                case HeightAndNormalMapСombinerMode.ALL_height:
                    Graphics.Blit(heightMap, destination);
                    break;
                default:
                    break;
            }
        }

        private static Material putHeightMapToBlueChanelMaterial;
        private static void PutHeightMapToBlueChanel(Texture current,Texture heightMap,RenderTexture destiniation)
        {
            if (putHeightMapToBlueChanelMaterial == null)
            {
                putHeightMapToBlueChanelMaterial = new Material(Shader.Find("Hidden/V/PutHeightMapToBlueChanel"));
            }
            putHeightMapToBlueChanelMaterial.SetTexture("_HeightMap", heightMap);
            Graphics.Blit(current, destiniation, putHeightMapToBlueChanelMaterial);
        }
    }

}
