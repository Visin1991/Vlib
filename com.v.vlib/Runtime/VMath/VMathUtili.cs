using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VMathUtili
{
    const float CROO = -0.5f;
    const float CR01 = 1.5f;
    const float CR02 = -1.5f;
    const float CR03 = 0.5f;

    const float CR10 = 1.0f;
    const float CR11 = -2.5f;
    const float CR12 = 2.0f;
    const float CR13 = -0.5f;
    const float CR20 = -0.5f;
    const float CR21 = 0.0f;
    const float CR22 = 0.5f;
    const float CR23 = 0.0f;
    const float CR30 = 0.0f;
    const float CR31 = 1.0f;
    const float CR32 = 0.0f;
    const float CR33 = 0.0f;

    public static Vector3 Refletion(Vector3 N, Vector3 toViewer)
    {
        Vector3 R = 2 * (Vector3.Dot(N, toViewer)) * N - toViewer;
        return R;
    }

    public static float Mod(float value, float frequency)
    {
        int n = (int)(value / frequency);
        value -= n * frequency;
        if (value < 0)
            value += frequency;
        return value;
    }

    //fractional part of the ratio a/b
    public static float Mod01(float value, float frequency)
    {
        int n = (int)(value / frequency);
        float c = value - n * frequency;
        if (value < 0)
            c += frequency;
        return c / frequency;
    }


    /*
     * result = spline(parameter,knotl, knot2, ..., knotN-1, knotN);
         If parameter is 0, the result is knot2. If parameter is 1, the result is knotN-1
       This is a approximate function....
    */
    public static float CatmullRomSpline(float x, List<float> knot)
    {
        int nknots = knot.Count;
        int span;
        int nspans = nknots - 3;

        float cO, cl, c2, c3; /* coefficients of the cubic.*/
        if (nspans < 1)
        {/* illegal */
            return 0;
        }

        /* Find the appropriate 4-point span of the spline. */
        x = Mathf.Clamp(x, 0, 1) * nspans;
        span = (int)x;

        if (span >= nspans)
            span = nspans;
        x -= span;

        float knot3 = 0;
        if (span + 3 < nknots) { knot3 = knot[span + 3]; }

        /* Evaluate the span cubic at x using Horner’s rule. */
        c3 = CROO * knot[span] + CR01 * knot[span+1] + CR02 * knot[span+2] + CR03 * knot3;
        c2 = CR10 * knot[span] + CR11 * knot[span+1] + CR12 * knot[span+2] + CR13 * knot3;
        cl = CR20 * knot[span] + CR21 * knot[span+1] + CR22 * knot[span+2] + CR23 * knot3;
        cO = CR30 * knot[span] + CR31 * knot[span+1] + CR32 * knot[span+2] + CR33 * knot3;
        return ((c3 * x + c2) * x + cl) * x + cO;
    }

    public static float Gammacorrect(float x, float gamma = 2.2f)
    {
        return Mathf.Pow(x, 1 / gamma);
    }

    public static float Bias(float x, float bais = 0.8f)
    {
        return Mathf.Pow(x, Mathf.Log(bais) / Mathf.Log(0.5f));
    }

    public static float Gain(float x, float gain)
    {
        if (x < 0.5f)
            return Bias(1 - gain, 2 * x) * 0.5f;
        else
            return 1 - Bias(1 - gain, 2 - 2 * x) * 0.5f;
    }

    public static Vector3 Spherical(float theta, float Phi,float r)
    {
        Vector3 vector3 = Vector3.zero;
        vector3.x = r * Mathf.Sin(theta) * Mathf.Cos(Phi);
        vector3.y = r * Mathf.Sin(theta) * Mathf.Sin(Phi);
        vector3.z = r * Mathf.Cos(theta);
        return vector3;
    }

    public static Vector3 SphericalCoord(Vector3 pos2Center)
    {
        float r = pos2Center.x * pos2Center.x + pos2Center.y * pos2Center.y + pos2Center.z * pos2Center.z;
        float theta = Mathf.Acos(pos2Center.z / r);
        float phi = Mathf.Atan2(pos2Center.y, pos2Center.x);
        return new  Vector3(theta, phi,r);
    }

}
