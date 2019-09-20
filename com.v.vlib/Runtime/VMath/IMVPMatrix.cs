using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class IMVPMatrix : MonoBehaviour
    {

        Renderer m_renderer;
        Material m_mat;

        private void LateUpdate()
        {
            if (gameObject.vCheckAndCache_RenderMat(ref m_renderer, ref m_mat))
            {
                m_mat.SetMatrix("_ClipToLocal", V.Math.Matrix.MatrixExtentions.IMVP(transform, Camera.main));
            }
        }
    }
}
