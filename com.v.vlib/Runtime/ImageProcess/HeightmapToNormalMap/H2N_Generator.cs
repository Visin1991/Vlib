using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [System.Serializable]
    public struct H2N_Generator
    {
        [SerializeField]
        public GaussianBlurFilter preBlur;
        [SerializeField]
        public SobelNormalFilter sobeNormal_Filter;
        [SerializeField]
        public NormalizeNormalMapFilter normalizeFilter;
        [SerializeField]
        public GaussianBlurFilter postBlur;

        public void Apply(Texture source, RenderTexture destiniation)
        {

            int w = source.width;
            int h = source.height;
            var rt_descriptor = new RenderTextureDescriptor(w, h) { sRGB = false };

            RenderTexture blurHeightMapRT =  Apply_HeightMap_Bluring(source, rt_descriptor);
            RenderTexture sobNormal = Apply_SobeNormal(blurHeightMapRT, rt_descriptor);
            RenderTexture postBlur_Normal_RT = Apply_Normal_Bluring(sobNormal, rt_descriptor);
            normalizeFilter.Apply(postBlur_Normal_RT, destiniation);
        }

        RenderTexture Apply_HeightMap_Bluring(Texture source,RenderTextureDescriptor rt_descriptor)
        {
            RenderTexture blurHeightMapRT = RenderTexture.GetTemporary(rt_descriptor);
            preBlur.Apply(source, blurHeightMapRT);
            return blurHeightMapRT;
        }

        RenderTexture Apply_SobeNormal(RenderTexture source, RenderTextureDescriptor rt_descriptor)
        {
            RenderTexture sobNormal = RenderTexture.GetTemporary(rt_descriptor);
            sobeNormal_Filter.Apply(source, sobNormal);
            RenderTexture.ReleaseTemporary(source);
            return sobNormal;
        }

        RenderTexture Apply_Normal_Bluring(RenderTexture source, RenderTextureDescriptor rt_descriptor)
        {
            RenderTexture postBlur_Normal_RT = RenderTexture.GetTemporary(rt_descriptor);
            postBlur.Apply(source, postBlur_Normal_RT);
            RenderTexture.ReleaseTemporary(source);
            return postBlur_Normal_RT;
        }
    }

}