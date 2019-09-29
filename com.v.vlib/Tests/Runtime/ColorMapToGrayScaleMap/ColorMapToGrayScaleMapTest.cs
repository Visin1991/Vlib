using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorMapToGrayScaleMapTest : MonoBehaviour
{
    public Texture2D colorTexture;
    public RenderTexture grayScaleTexture;

    [ContextMenu("Do Test")]
    public void Test()
    {
        if (grayScaleTexture)
        {
            RenderTexture.ReleaseTemporary(grayScaleTexture);
        }

        grayScaleTexture = V.VBlit.Blit_To_New_Color_To_GrayScale(colorTexture);

    }

    private void OnDestroy()
    {
        if (grayScaleTexture)
        {
            RenderTexture.ReleaseTemporary(grayScaleTexture);
        }
    }
}
