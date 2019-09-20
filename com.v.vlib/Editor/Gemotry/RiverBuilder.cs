using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace V
{
    public class RiverBuilder : EditorWindow {

        [MenuItem("V/River")]
        public static void OpenRiverWindow()
        {
            var window = EditorWindow.GetWindow<RiverBuilder>();
            window.titleContent = new GUIContent("River Builder");
            window.Show();
        }

        private void OnGUI()
        {
            if (GUILayout.Button("创建River"))
            {
                CreateCatmullRomSpline();
            }
        }

        void CreateCatmullRomSpline()
        {
            string riverFile = VIO.UnitySaveFolder();
            if (riverFile == null)
            {
                return;
            }

            GameObject gameObject = new GameObject("River");
            Camera camera = null;
            for (int i = 0; i < 10; i++)
            {
                if (SceneView.lastActiveSceneView == null || SceneView.lastActiveSceneView.camera == null)
                {
                    continue;
                }
                camera = SceneView.lastActiveSceneView.camera;
            }
            if (camera != null)
            {
                gameObject.transform.position = camera.transform.position + camera.transform.forward * 10.0f;
            }

            gameObject.AddComponent<CatmullRomSpline>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.receiveShadows = true;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            if (meshRenderer.sharedMaterial == null)
                meshRenderer.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat");

            Selection.activeGameObject = gameObject;


        }

        //private void OnEnable()
        //{
        //    SceneView.onSceneGUIDelegate += OnSceneGUI;
        //}

        //private void OnDisable()
        //{
        //    SceneView.onSceneGUIDelegate -= OnSceneGUI;
        //}


        //static void OnSceneGUI(SceneView sceneView)
        //{
        //    if (Event.current.isKey)
        //        Debug.Log(Event.current.keyCode);
        //}

    }
}
