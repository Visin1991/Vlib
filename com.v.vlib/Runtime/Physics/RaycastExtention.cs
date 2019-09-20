using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V.VPhysics
{
    public static class RaycastExtention
    {
        public static RaycastHit temphit = new RaycastHit();

        public static bool If_Collision_FromAtoB(Vector3 fromPos, Vector3 toPos)
        {
            Vector3 dir = (toPos - fromPos);
            float length = dir.magnitude;
            if (UnityEngine.Physics.Raycast(fromPos, dir, length))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool If_Collide_Target_FromAtoB(Vector3 fromPos, Vector3 toPos,Collider targetC)
        {
            Vector3 dir = (toPos - fromPos);
            float length = dir.magnitude;
            if (UnityEngine.Physics.Raycast(fromPos, dir,out temphit, length))
            {
                if (temphit.collider == targetC) { return true; }
                else { return false; }
            }
            else
            {
                return false;
            }
        }

        public static bool If_Collision_FromAtoB(Vector3 fromPos, Vector3 toPos, ref RaycastHit hit)
        {
            Vector3 dir = (toPos - fromPos);
            float length = dir.magnitude;
            if (UnityEngine.Physics.Raycast(fromPos, dir, out hit, length))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Raycast from A to B collid with a collider which the gameObject contains a T component
        /// </summary>
        /// <typeparam name="T"> The Component which we want to check if the collider's gameObject have</typeparam>
        /// <param name="fromPos"> From positon </param>
        /// <param name="toPos"> Destination Position</param>
        /// <returns></returns>
        public static T If_Collision_FromAtoB<T>(Vector3 fromPos, Vector3 toPos ) where T : MonoBehaviour
        {
            Vector3 dir = toPos - fromPos;
            float length = dir.magnitude;
            if (Physics.Raycast(fromPos, dir, out temphit, length))
            {
                T t = temphit.collider.GetComponent<T>();
                return t;
            }
            else {
                return null;
            }
        }
    }
}
