using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace V
{
    [ExecuteInEditMode]
    public class CameraDepthSetting : MonoBehaviour
    {

#if UNITY_EDITOR
        public DepthTextureMode mode = DepthTextureMode.Depth;
        // Use this for initialization
        void Start()
        {
            Camera camera = GetComponent<Camera>();
            camera.depthTextureMode = mode;
        }
#endif
    }
}
