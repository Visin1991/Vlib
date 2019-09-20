using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Created by Wei, 03/06/2018
/// </summary>

namespace V.Math.Matrix
{
    public static class MatrixExtentions 
    {
        /// <summary>
        /// Linear Transformations :
        ///     Mathematically,a mapping F(a) is linear if :----->addition and salce oder...
        ///        1. F(a + b) = F(a) + F(b)   and  F(ka) = k(F)a.
        ///        2. If F(0) = a, a != 0, then F cannot be a linear mapping,since F(k0)=a and therefore F(k0) != kF(0).
        /// 
        /// Affine Transformations :
        ///     An affine transformation is a linear transformation followed by translation.......P177
        ///     
        /// Invertible Transformations :
        ///     A transformation is invertible if there exists an opposite transformation, known as the inverse of F,
        ///         that "undoes" the original transformation.
        ///     Mathmaticlly, a mapping F(a) is invertible if there exists an inverse mapping F^-1:
        ///         F(F(a))^-1  =  F(F(a)^-1)
        ///         
        ///      AA^(-1)=I 
        ///      C=AB. 
        ///      B=A^(-1)AB=A^(-1)C 
        ///      A=ABB^(-1)=CB^(-1). 
        ///      C=AB=(CB^(-1))(A^(-1)C)=CB^(-1)A^(-1)C
        ///      CB^(-1)A^(-1)=I
        ///      B^(-1)A^(-1)=C^(-1)=(AB)^(-1)
        /// </summary>

        ///<summary>
        ///                    4x4 Homogeneous Matrices
        ///     4D vectors have four components, with the first three compontes beging the standard x,y,and z components
        /// the fourth component in a 4D vector is w, sometimes refereed to as the homogenous coordinate.
        /// 
        ///     Imagine the standard 2D plane as existing in 3D at the plane w = 1, such that physical 2D point(x,y) is 
        ///represented in homogeneous space (x,y,1).------> for all points that are not in the plane W = 1,we can compute
        ///the corresponding 2D point by projection the point onto the plane w = 1, by dividing w. So the homogeneous coordinate(x,y,w)
        ///is mapped to the physical 2D point(x/w, y/w).
        ///     For example if a point i 3D world has position A(2.5,2.0,2.5), then in 2D world, when homogenous coordinate has value 1,
        ///then position A will be (1.0,0.8,1.0)
        ///
        ///     In 3D world, usually we set the far clip plane has the homogenous value of 1......
        /// 
        ///</summary>

        ///<summary>
        ///                         RotationMatrix * TransitionMatrix
        ///         | r11  r12  r13  0||1    0    0    0|   | r11  r12  r13  0|
        ///         | r21  r22  r23  0||0    1    0    0| = | r21  r22  r23  0|
        ///         | r31  r32  r33  0||0    0    0    0|   | r31  r32  r33  0|
        ///         |  0    0    0   0||tZ   tY   tZ   1|   | tZ   tY   tZ   0|
        /// 
        /// </summary>


        public static Matrix4x4 ClipToWorld_V2W_I_Proj(Matrix4x4 viewToWorld, Matrix4x4 inverseProjection)
        {
            return viewToWorld * inverseProjection;
        }

        public static Matrix4x4 ClipToWorld_W2V_Proj(Matrix4x4 worldToView, Matrix4x4 projection)
        {
            //B^(-1)A^(-1) = (AB)^(-1)----->multiply order do matter
            //return worldToView.inverse * projection.inverse
            return (projection * worldToView).inverse;
        }

        public static Matrix4x4 ClipToWorld(Camera camera)
        {
            return camera.cameraToWorldMatrix * camera.projectionMatrix.inverse;
        }

        public static Matrix4x4 IMVP(Transform transform,Camera camera)
        {
            var proj = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, true);
            return (proj * Camera.main.worldToCameraMatrix * transform.localToWorldMatrix).inverse;
        }

        public static Matrix4x4 MVP(Transform transform, Camera camera)
        {
            var proj = GL.GetGPUProjectionMatrix(Camera.main.projectionMatrix, true);
            return (proj * Camera.main.worldToCameraMatrix * transform.localToWorldMatrix);
        }
    }
}
