using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    [System.Serializable]
    public struct SobelNormalFilter
    {
        [Range(0, 1)]
        public float bumpEffect;
        private static Material normalMapMaterial;

        public void Apply(Texture source, RenderTexture destination)
        {
            if (normalMapMaterial == null)
            {
                normalMapMaterial = new Material(Shader.Find("Hidden/V/SobelH2N")); 
            }

            float v = bumpEffect * 2f - 1f;
            float z = 1f - v;
            float xy = 1f + v;

            normalMapMaterial.SetVector("_Factor", new Vector4(xy, xy, z, 1));

            Graphics.Blit(source, destination, normalMapMaterial);
        }
    }
}
