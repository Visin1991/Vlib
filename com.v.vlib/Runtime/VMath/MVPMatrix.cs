using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class MVPMatrix : MonoBehaviour
    {

        public float outlineScale = 1.1f;
        //Matrix4x4 m = Matrix4x4.identity;
        Material mat;
        Renderer m_render;
        // Use this for initialization

        void Start()
        {
            gameObject.vCheckAndCache_RenderMat(ref m_render, ref mat);
        }

        // Update is called once per frame
        void Update()
        {
            if (gameObject.vCheckAndCache_RenderMat(ref m_render, ref mat))
            {
                Matrix4x4 mvp = V.Math.Matrix.MatrixExtentions.MVP(transform, Camera.main);
                mat.SetMatrix("V_MVP", mvp);

            }
        }
    }
}
