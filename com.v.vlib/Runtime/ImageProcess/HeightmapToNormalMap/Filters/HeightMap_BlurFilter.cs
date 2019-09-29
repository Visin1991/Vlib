using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [System.Serializable]
    public class HeightMap_BlurFilter
    {
        [SerializeField]
        public GaussianBlurFilter preBlur;
        [Range(0,2)]
        public float factor = 1;
        [SerializeField]
        public GaussianBlurFilter postBlur;

        Material mat;

        public void Apply(Texture source, RenderTexture destination)
        {
            int w = source.width;
            int h = source.height;

            var rt_descriptor = new RenderTextureDescriptor(w, h) { sRGB = false };

            var bluring_Height_RT = RenderTexture.GetTemporary(rt_descriptor);
            preBlur.Apply(source, bluring_Height_RT);

            var heightMap_Scaler_RT = RenderTexture.GetTemporary(rt_descriptor);
            HeightMapScaler(bluring_Height_RT, heightMap_Scaler_RT);

            RenderTexture.ReleaseTemporary(bluring_Height_RT);

            postBlur.Apply(heightMap_Scaler_RT, destination);
            RenderTexture.ReleaseTemporary(heightMap_Scaler_RT);
        }

        public void HeightMapScaler(Texture source,RenderTexture destination)
        {
            if (mat == null)
            {
                mat = new Material(Shader.Find("Hidden/V/HeightMapScaler"));
            }
            mat.SetFloat("_Factor", factor);
            Graphics.Blit(source, destination, mat);
        }

    }
}
