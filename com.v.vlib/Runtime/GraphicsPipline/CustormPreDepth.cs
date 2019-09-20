using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace V
{
    [RequireComponent(typeof(Camera))]
    public class CustormPreDepth : MonoBehaviour
    {
        public RenderTexture Color_fbo;
        public Shader depthShader;
        Material depthOnlyMaterial;
        Camera m_camera;

        CommandBuffer depthRenderingCommand;

        bool init = false;

        int default_width = 1920;
        int default_height = 1080;

        public RawImage image;

        public void ReSize(int _width,int _height)
        {
            int w = (int)(_width * 0.25f);
            int h = (int)(_height * 0.25f);
            Color_fbo = new RenderTexture(w, h, 16, RenderTextureFormat.Depth);
        }

        private void Start()
        {
            depthRenderingCommand = new CommandBuffer();
            depthRenderingCommand.name = "V Simple Pre-Depth";

            Shader depthOnlyShader = Shader.Find("V/DepthOnly");

            if (depthOnlyShader == null) {
                Debug.LogError("Cant find DepthOnly Shader");
                init = false;
                return;
            }

            depthOnlyMaterial = new Material(depthOnlyShader);
            depthOnlyMaterial.hideFlags = HideFlags.HideAndDontSave;

            m_camera = GetComponent<Camera>();
            if (m_camera == null)
            {
                Debug.LogError("No Camera Attached");
                init = false;
                return;
            }
         
            int w = (int)(default_width * 0.25f);
            int h = (int)(default_height * 0.25f);

            Color_fbo = new RenderTexture(w, h, 16, RenderTextureFormat.Depth);
            image.texture = Color_fbo;
            init = true;
        }

        private void Update()
        {
            if (!init)
            {
                Debug.LogError(" Initialization Faild......");
                return;
            }

            if (m_camera != null)
            {
                float currentFarClip = m_camera.farClipPlane;
                m_camera.farClipPlane = 100;
                MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
                for (int i = 0; i < meshFilters.Length; i++)
                {
                    bool clearFlag = i == 0;
                    RenderMesh(ref Color_fbo, meshFilters[i].sharedMesh, depthOnlyMaterial, clearFlag, meshFilters[i].transform.localToWorldMatrix, m_camera.worldToCameraMatrix, m_camera.projectionMatrix);
                }
                m_camera.farClipPlane = currentFarClip;

                Shader.SetGlobalTexture("_SLSimpleDepth", Color_fbo);
            }
        }

        void RenderMesh(ref RenderTexture _colorBuffer, Mesh mesh, Material material, bool clearFlag, Matrix4x4 M_Matrix, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, RenderTexture _depthBuffer = null)
        {
            if (depthRenderingCommand == null)
            {
                depthRenderingCommand = new CommandBuffer();
                depthRenderingCommand.name = "V Simple Pre-Depth";
            }


            depthRenderingCommand.Clear();
    
            if (_depthBuffer == null)
            {
                depthRenderingCommand.SetRenderTarget(_colorBuffer);
            }
            else
            {
                depthRenderingCommand.SetRenderTarget(_colorBuffer, _depthBuffer);
            }

            if (clearFlag)
            {
                depthRenderingCommand.ClearRenderTarget(true, true, Color.clear, 1);
            }

            depthRenderingCommand.SetViewProjectionMatrices(viewMatrix, projectionMatrix);

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                depthRenderingCommand.DrawMesh(mesh, M_Matrix, material, i);
            }

            Graphics.ExecuteCommandBuffer(depthRenderingCommand);
        }

        private void OnDestroy()
        {
            if (depthOnlyMaterial)
            {
                DestroyImmediate(depthOnlyMaterial);
            }

            if (Color_fbo)
            {
                DestroyImmediate(Color_fbo);
            }

            if (depthRenderingCommand!=null)
            {
                depthRenderingCommand.Release();
                depthRenderingCommand = null;
            }

        }
    }
}
