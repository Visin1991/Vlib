using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public static class Matrix4x4Extention
    {

        /// <summary>
        /// XMMatrixRotationAxis
        /// </summary>
        public static Matrix4x4 CreateRotationMatrix(Vector3 aixs, float angle)
        {
            return Matrix4x4.Rotate(Quaternion.AngleAxis(angle, aixs));
        }

    }
}