using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public static class Vector3Extention
    {

        public static Vector3 RandomPos(this Vector3 vector3, float from, float to)
        {
            return new Vector3(Random.Range(from, to), Random.Range(from, to), Random.Range(from, to));
        }

        public static Vector3 RandomPosXZ(this Vector3 vector3, float from, float to)
        {
            return new Vector3(Random.Range(from, to), 0, Random.Range(from, to));
        }

        public static Vector3 Multiply(this Vector3 value1, Vector3 value2)
        {
            Vector3 Result;
            Result.x = value1.x * value2.x;
            Result.y = value1.y * value2.y;
            Result.z = value1.z * value2.z;
            return Result;
        }

        public static Vector3 Divide(this Vector3 value1, float v2)
        {
            Vector3 Result;
            Result.x = value1.x / v2;
            Result.y = value1.y / v2;
            Result.z = value1.z / v2;
            return Result;
        }

        public static Vector3 MultiplyAdd(this Vector3 V3, Vector3 V1, Vector3 V2)
        {
            Vector3 Result;
            Result.x = V1.x * V2.x + V3.x;
            Result.y = V1.y * V2.y + V3.y;
            Result.z = V1.z * V2.z + V3.z;
            return Result;
        }

        public static Vector3 RotateAxis(this Vector3 v, Vector3 axis, float angle)
        {
            return Quaternion.AngleAxis(angle, axis) * v;
        }

        public static Vector3 RotateAroundYAxis(this Vector3 v, float deg)
        {
            float alpha = deg * Mathf.PI / 180.0f;
            float sina = Mathf.Sin(alpha);
            float cosa = Mathf.Cos(alpha);
            Matrix4x4 m2x2 = new Matrix4x4(new Vector4(cosa, -sina, 0, 0),
                                           new Vector4(sina, cosa, 0, 0),
                                           Vector4.zero, Vector4.zero);
            Vector2 xz = m2x2.MultiplyVector(new Vector2(v.x, v.z));
            return new Vector3(xz.x, v.y, xz.y);

        }

        public static float MagnitudeSq(this Vector3 v)
        {
            return Vector3.Dot(v, v);
        }

        public static Vector3 Mirror(this Vector3 v, Vector3 axis)
        {
            return v - axis * (Vector3.Dot(v, axis) * 2);
        }


    }
}
