using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class ClipToWorldMatrix : MonoBehaviour
    {

        public float rotationSpeed = 20.0f;
        Renderer m_renderer;
        Material m_mat;
        private Vector2 scaleXZ;

        public bool IMVPProjection = false;


        // Update is called once per frame
        void LateUpdate()
        {
            if (gameObject.vCheckAndCache_RenderMat(ref m_renderer, ref m_mat))
            {

                m_mat.SetMatrix("_ClipToWorld", V.Math.Matrix.MatrixExtentions.ClipToWorld(Camera.main));
                m_mat.SetVector("_Center", transform.position);
                scaleXZ.x = transform.localScale.x;
                scaleXZ.y = transform.localScale.z;
                m_mat.SetVector("_ScaleXZ", scaleXZ);
                m_mat.SetFloat("_RotationSpeed", Mathf.Deg2Rad * rotationSpeed * Time.time);
            }

        }
    }
}
