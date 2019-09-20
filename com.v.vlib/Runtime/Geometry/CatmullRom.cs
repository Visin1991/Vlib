using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class CatmullRom
    {
        public static Vector3 Position(float t,Vector3 p0,Vector3 p1,Vector3 p2,Vector3 p3)
        {
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
            return pos;
        }

        public static Vector3 TangentForward(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            return (0.5f * ((-p0 + p2) + 2f * (2f * p0 - 5f * p1 + 4f * p2 - p3) * t + 3f * (-p0 + 3f * p1 - 3f * p2 + p3) * t * t)).normalized;
        }
    }
}
