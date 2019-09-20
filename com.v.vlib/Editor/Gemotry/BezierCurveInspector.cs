using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    [CustomEditor(typeof(BezierCurve))]
    public class BezierCurveInspector : Editor
    {

        private BezierCurve curve;
        private Transform handleTransform;
        private Quaternion handleRotation;


        private const int lineSteps = 10;
        private const float directionScale = 0.5f;



        private void OnSceneGUI()
        {
            curve = target as BezierCurve;
            handleTransform = curve.transform;
            handleRotation = Tools.pivotRotation == PivotRotation.Local ?
                handleTransform.rotation : Quaternion.identity;

            Vector3 p0 = ShowPoint(0);
            Vector3 p1 = ShowPoint(1);
            Vector3 p2 = ShowPoint(2);
            Vector3 p3 = ShowPoint(3);

            //Show the Control Line
            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);


            //-------------Debug------------
            //ShowDirections();
            //Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2f);
            //------------------------------
        }

        private void ShowDirections()
        {
            for (int i = 0; i <= lineSteps; i++)
            {

                Vector3 tangentDir = curve.GetDirection(i / (float)lineSteps);
                Quaternion rotation = Quaternion.LookRotation(tangentDir);
                Vector3 normal = rotation * Vector3.up;
                Vector3 right = Vector3.Cross(tangentDir, normal);

                Vector3 point = curve.GetPoint(i / (float)lineSteps);

                Handles.color = Color.green;
                Handles.DrawLine(point, point + tangentDir * directionScale);

                Handles.color = Color.red;
                Handles.DrawLine(point, point + normal * directionScale);

                Handles.color = Color.blue;
                Handles.DrawLine(point, point + right * directionScale);

            }
        }

        private Vector3 ShowPoint(int index)
        {
            Vector3 point = handleTransform.TransformPoint(curve.points[index]);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(curve, "Move Point");
                EditorUtility.SetDirty(curve);
                curve.points[index] = handleTransform.InverseTransformPoint(point);
            }
            return point;
        }
    }
}