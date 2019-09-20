using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    [ExecuteInEditMode]
    public class CameraSynchronizeTF : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Update()
        {
            if (Camera.main == null) {
                Debug.Log("No Main Camera");
                return;
            }
            if (UnityEditor.SceneView.lastActiveSceneView == null ||UnityEditor.SceneView.lastActiveSceneView.camera == null)
                return;
            Camera.main.transform.position = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
            Camera.main.transform.rotation = UnityEditor.SceneView.lastActiveSceneView.camera.transform.rotation;
        }
#endif
    }
}
