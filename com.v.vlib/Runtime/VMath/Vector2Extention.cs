using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    public static class Vector2Extention
    {

        public static Vector2 Rotate(this Vector2 vector2, float angle)
        {
            float x = vector2.x;
            float y = vector2.y;
            float newX = x * Mathf.Cos(Mathf.Deg2Rad * angle) - y * Mathf.Sin(Mathf.Deg2Rad * angle);
            float newY = x * Mathf.Sin(Mathf.Deg2Rad * angle) + y * Mathf.Cos(Mathf.Deg2Rad * angle);
            return new Vector2(newX, newY);
        }

        public static Vector2 CartesianToPolar(this Vector2 xy)
        {
            float r = Mathf.Sqrt(xy.x * xy.x + xy.y * xy.y);
            float theta = Mathf.Atan2(xy.y, xy.x);
            return new Vector2(r, theta);
        }

        public static Vector2 PolarToCartesian(this Vector2 rt)
        {
            return new Vector2(rt.x * Mathf.Cos(rt.y), rt.x * Mathf.Sin(rt.y));
        }

        public static Vector2 RotatePointAround(this Vector2 p, Vector2 c, float radius)
        {
            p -= c;
            Vector2 polar = CartesianToPolar(p);
            polar.y += radius;
            return PolarToCartesian(polar) + c;
        }
    }
}
