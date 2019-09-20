using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class VFrustum
    {
        /// <summary>
        /// Create a Frustum.......
        /// </summary>
        /// <param name="near">projection length of near plane</param>
        /// <param name="far">projection length of far plane</param>
        /// <param name="alpha">Vertical field of view angle</param>
        /// <param name="aspect">aspect ration r : defined by r = w/h --> w is the width of the projection window,h is the height of the projection window</param>
        public VFrustum(float _near, float _far, float _alpha, float _aspect)
        {
            near = _near;
            far = _far;
            alpha = _alpha;
            aspect = _aspect;
        }

        public VFrustum(float _near, float _far, float _alpha, float screenWidth, float screenHeight)
        {
            near = _near;
            far = _far;
            alpha = _alpha;
            aspect = screenWidth / screenHeight;
        }

        public float HorizontalFOV
        {
            get {return 2 * Mathf.Atan(aspect * Mathf.Tan(alpha / 2));}
        }

        public float Beta
        {
            get { return 2 * Mathf.Atan(aspect * Mathf.Tan(alpha / 2)); }
        }

        public bool Contains(Vector3 pos)
        {
            pos = NDC(pos);
            bool contains = -aspect <= pos.x && pos.x <= aspect;
            contains = contains && -1 <= pos.y && pos.y <= 1;
            contains = contains && near <= pos.z && pos.z <= far;
            return contains;
        }

        public Vector3 NDC(Vector3 pos)
        {
            Matrix4x4 projectionMatrix = new Matrix4x4();
            projectionMatrix.m00 = 1 / (aspect * Mathf.Tan(alpha / 2));
            projectionMatrix.m11 = 1 / (         Mathf.Tan(alpha / 2));
            projectionMatrix.m22 = far / (far - near);
            projectionMatrix.m23 = 1;
            projectionMatrix.m32 = (-near * far) / (far - near);
            projectionMatrix.m33 = 0;
            return projectionMatrix * pos;
        }


        /* Normalized Device Coordinates(NDC)
         * In view space, The projection window has a height of 2, and width of 2r. then aspect ratio r = 2r /2 
         * if the Height range in View space is [-1,1]
         * then the width range in view space will be [-r,r]
         * 
         * In NDC coordinates, the projection window has a height of 2 and a width of 2,it dimensions are fixed,
         * the hardware need not know the aspect ratio......
         * 
         * Solution:
         *      scale the projected x-coordinate from the interval[-r,r] to [-1,1]
         *      -r <=   x'  <= r
         *      -1 <=  x'/r <= 1
         
                x' = x /(r * z * tan(alpha/2));
                y' = y /(    z * tan(alpha/2));

         * DirectX Projection Matrix:
         * 
         *  1/(r*tan(alpha/2))            0                 0                  0
         *            0           1/(tan(alpha/2))          0                  0
         *            0                   0               f/(f-n)              1
         *            0                   0                -n*f/(f-n)          0
        */

       
        //-----------------------------------------------------------------------------
        //private
        float near;
        float far;
        float alpha;
        float aspect;
    }
}
