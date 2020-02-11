using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphericalRotator : MonoBehaviour
{
    [Range(0,1.0f)]
    public float inclination;
    [Range(0, 2.0f)]
    public float azimuth;

    public Material mat;

    Vector3 eulerAngle = Vector3.zero;

    private void OnValidate()
    {
        eulerAngle.x = inclination * 180.0f;
        eulerAngle.y = azimuth * 180.0f;
        eulerAngle.z = 0.0f;

        transform.eulerAngles = eulerAngle;

        if (transform.parent != null)
        {
            Vector3 sphericalPos = Spherical(inclination * Mathf.PI, azimuth * Mathf.PI);
            transform.position = transform.parent.position + sphericalPos * 5.0f;
        }

        if (mat != null)
        {
            mat.SetFloat("_Inclination", inclination);
            mat.SetFloat("_Azimuth", azimuth);
        }
    }

    public static Vector3 Spherical(float inclination, float azimuth, float r = 1.0f)
    {
        Vector3 vector3 = Vector3.zero;
        azimuth *= -1;
        vector3.z = r * Mathf.Cos(inclination) * Mathf.Cos(azimuth) * -1;
        vector3.x = r * Mathf.Cos(inclination) * Mathf.Sin(azimuth);
        vector3.y = r * Mathf.Sin(inclination);
        return vector3;
    }
}
