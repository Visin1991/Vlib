using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V.Vlib
{
    public static class Test
    {
        public static void TestFunc()
        {
            EditorGUI.BeginChangeCheck();
            Debug.Log("Test");
            EditorGUI.EndChangeCheck();

        }
    }

}