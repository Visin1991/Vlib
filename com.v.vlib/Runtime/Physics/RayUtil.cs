using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V.Math.RayUtil
{
    public static class RayUtil
    {
        // (p0 - l0) · n  / l ·n = t

        // p0 : the world origin to Plane.
        // n  : the normal of the plane
        // p  : the intersection point on the plane
        // l0 : the origin of the ray
        // l  : direction of the ray
        /// <summary>
        /// return -1 means, the ray did not intersect with the plane.
        /// return 0 may means the ray almost parallel with the plane
        /// Detal see: https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-plane-and-ray-disk-intersection
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="plane"></param>
        /// <returns></returns>
        public static float Ray_Plane_Intersection_Distance(this Ray ray,Plane plane)
        {
            float denom = Vector3.Dot(ray.direction,plane.normal);
            if (Mathf.Abs(denom) > 0.0000001)
            {
                Vector3 worldOriginToPlane = plane.normal * plane.distance;
                float t = Vector3.Dot(worldOriginToPlane - ray.origin, plane.normal) / denom;
                return t;
            }
            return -1;
        }
    }
}
