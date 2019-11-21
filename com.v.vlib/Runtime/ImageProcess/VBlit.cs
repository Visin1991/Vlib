using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VBlit
    {
        private static Material m_Mat_ColorToGrayScale;
        public static Material Material_ColorToGrayScale
        {
            get
            {
                if (m_Mat_ColorToGrayScale == null)
                {
                    m_Mat_ColorToGrayScale = new Material(Shader.Find("Hidden/V/ColorToGrayScale"));
                }
                return m_Mat_ColorToGrayScale;
            }
        }



        /// <summary>
        /// Blit to a New Render Texsture, with the same size 
        /// </summary>
        /// <param name="source">the Input Source Texture as  _MainTex</param>
        /// <param name="material"> Blit Material</param>
        /// <returns></returns>
        public static RenderTexture Blit_To_New(Texture source, Material material)
        {
            int w = source.width;
            int h = source.height;

            var rt_descriptor = new RenderTextureDescriptor(w, h)
            {
                sRGB = false,
                colorFormat = RenderTextureFormat.Default,
                depthBufferBits = 0
            };

            var rt = RenderTexture.GetTemporary(rt_descriptor);

            if (material)
            {
                Graphics.Blit(source, rt, material);
            }
            return rt;
        }

        /// <summary>
        /// Blit to a new Render Texture with a given RenderTexture Descriptor
        /// </summary>
        /// <param name="source">the Input Source Texture as  _MainTex</param>
        /// <param name="rt_descriptor">RenderTextureDescriptor</param>
        /// <param name="material">Blit Material</param>
        /// <returns></returns>
        public static RenderTexture Blit_To_New(Texture source, RenderTextureDescriptor rt_descriptor, Material material)
        {
            RenderTexture rt = RenderTexture.GetTemporary(rt_descriptor);
            if (material)
            {
                Graphics.Blit(source, rt, material);
            }
            return rt;
        }

        public static RenderTexture Blit_To_New_Color_To_GrayScale(Texture source)
        {
            int w = source.width;
            int h = source.height;

            var rt_descriptor = new RenderTextureDescriptor(w, h)
            {
                sRGB = false,
                colorFormat = RenderTextureFormat.Default,
                depthBufferBits = 0
            };

            var rt = RenderTexture.GetTemporary(rt_descriptor);

            Material mat_ColorToGrayScale = Material_ColorToGrayScale;
            if (mat_ColorToGrayScale != null)
            {
                Graphics.Blit(source, rt, mat_ColorToGrayScale);
            }

            return rt;
        }
    }
}
