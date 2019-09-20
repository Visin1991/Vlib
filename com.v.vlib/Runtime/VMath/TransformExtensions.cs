using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class TransformExtensions
    {

        public static void Set_UpDir_To_TargetDir(this Transform transform, Vector3 target)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, target);
        }
    }
}
