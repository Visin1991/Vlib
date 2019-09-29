using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [System.Serializable]
    public class GaussianBlurFilter
    {
        [Range(0, 50)]
        public int iteration;
        [Range(0, 2)]
        public float sampleFactor;

        private static Material blurMaterial;

        public void Apply(Texture source, RenderTexture destination)
        {
            if (blurMaterial == null)
            {
                blurMaterial = new Material(Shader.Find("Hidden/V/GaussianBlur"));
            }
                RenderTexture rt1, rt2;
                int w = Mathf.RoundToInt(source.width * sampleFactor);
                int h = Mathf.RoundToInt(source.height * sampleFactor);
                if (w < 1) w = 1;
                if (h < 1) h = 1;

                var rtd = new RenderTextureDescriptor(w, h) { sRGB = false };

                rt1 = RenderTexture.GetTemporary(rtd);
                rt2 = RenderTexture.GetTemporary(rtd);

                //Copy Sourcce to rt1
                Debug.Log("Copy......");
                Graphics.Blit(source, rt1);

                //Iteration rt1 <=> rt2
                for (var i = 0; i < iteration; i++)
                {
                    Debug.Log("Do Bluring Iteration");
                    Graphics.Blit(rt1, rt2, blurMaterial, 1);   // Bluring Horizontal
                    Graphics.Blit(rt2, rt1, blurMaterial, 2);   // Bluring Vertical
                }

                Debug.Log("Set to Destination");
                //Final blit rt1 to destination
                Graphics.Blit(rt1, destination);    //Quarter downsampler!!! Is it necessary ? this should be done in hardware

                RenderTexture.ReleaseTemporary(rt1);
                RenderTexture.ReleaseTemporary(rt2);
        }
    }
}
