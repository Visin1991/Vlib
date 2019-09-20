using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public static class Vector4Extention
    {

        public static Vector4 MultiplyAdd(this Vector4 V3, Vector4 V1, Vector4 V2)
        {
            Vector4 Result;
            Result.x = V1.x * V2.x + V3.x;
            Result.y = V1.y * V2.y + V3.y;
            Result.z = V1.z * V2.z + V3.z;
            Result.w = V1.w * V2.w + V3.w;
            return Result;
        }
    }
}
