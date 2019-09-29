using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public struct NormalizeNormalMapFilter
    {
        private static Material normalizeNormalMapMaterial;

        public void Apply(Texture source, RenderTexture destination)
        {
            if (normalizeNormalMapMaterial == null)
            {
                normalizeNormalMapMaterial = new Material(Shader.Find("Hidden/V/NormalizeNormalMap"));
            }
            Graphics.Blit(source, destination, normalizeNormalMapMaterial);
        }
    }
}
