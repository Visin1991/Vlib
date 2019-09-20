using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace V
{
    public static class OffScreenRendering
    {
        /// <summary>
        /// Rendering Mesh to a billboard Atlas......
        /// </summary>
        /// <param name="_bakeingMesh">The mesh We want to Rendering</param>
        /// <param name="_cameraPos"> a list of camera to object direction</param>
        /// <param name="_viewPorts"> a list of viewPos for the atlas</param>
        /// <param name="_materials"> a list of materials we want to use for each submesh</param>
        /// <param name="_padding"> padding size for each tile</param>
        /// <returns></returns>
        public static void RenderImpostor(Mesh _bakeingMesh, List<Vector3> _cameraPos, List<Rect> _viewPorts, List<Material> _materials, float _padding,ref RenderTexture[] _GBuffer,ref RenderTexture _depthBuffer,bool clearRT = true)
        {
            //Create RenderTexture
            RenderTargetIdentifier[] rtIDs = new RenderTargetIdentifier[_GBuffer.Length];
            for (int i = 0; i < _GBuffer.Length; i++)
            {
                rtIDs[i] = _GBuffer[i];
            }

            //Create CommandBuffer
            CommandBuffer commandBuffer = new CommandBuffer();
            commandBuffer.SetRenderTarget(rtIDs, _depthBuffer);
            if (clearRT)
            {
                commandBuffer.ClearRenderTarget(true, true, Color.clear, 1);
            }

            Bounds bounds = _bakeingMesh.bounds;
            float max = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
            float maxHalf = 0.5f * max + _padding;
            float depthMax = Mathf.Sqrt(bounds.size.x * bounds.size.x + bounds.size.y + bounds.size.y + bounds.size.z + bounds.size.z);

            GameObject cameraObj = new GameObject("Created_Camera_Obj");
            cameraObj.hideFlags = HideFlags.HideAndDontSave;
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.transform.localScale = new Vector3(1, 1, 1);
            Vector3 boundsOffset = bounds.center;

            for (int i = 0; i < _cameraPos.Count; i++)
            {
                Vector3 cameraDirVector = _cameraPos[i].normalized * depthMax;
                camera.transform.position = cameraDirVector + boundsOffset;
                camera.transform.rotation = Quaternion.LookRotation(Vector3.zero - cameraDirVector, Vector3.up);
                Matrix4x4 projMatrix = Matrix4x4.Ortho(-maxHalf, maxHalf, -maxHalf, maxHalf, 0.0f, depthMax);
                commandBuffer.SetViewProjectionMatrices(camera.worldToCameraMatrix, projMatrix); //World to camera space & camera to clip space
                commandBuffer.SetViewport(_viewPorts[i]);
                for (int m = 0; m < _materials.Count; m++)
                {
                    if (m < _bakeingMesh.subMeshCount)
                        commandBuffer.DrawMesh(_bakeingMesh, Matrix4x4.identity, _materials[m], m, 0);
                }
            }

            Graphics.ExecuteCommandBuffer(commandBuffer);

            GameObject.DestroyImmediate(cameraObj);

            commandBuffer.Release();
            commandBuffer = null;
        }

        static CommandBuffer depthRenderingCommand = new CommandBuffer();
        public static void RenderMesh(ref RenderTexture _colorBuffer,Mesh mesh,Material material,bool clearFlag,Matrix4x4 M_Matrix, Matrix4x4 viewMatrix,Matrix4x4 projectionMatrix,RenderTexture _depthBuffer = null)
        {
            depthRenderingCommand.Clear();
            depthRenderingCommand.name = "V Simple Pre-Depth";
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
                depthRenderingCommand.ClearRenderTarget(true, true,Color.clear,1);
            }

            depthRenderingCommand.SetViewProjectionMatrices(viewMatrix, projectionMatrix);

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                depthRenderingCommand.DrawMesh(mesh, M_Matrix, material, i);
            }

            Graphics.ExecuteCommandBuffer(depthRenderingCommand);
        }

    }
}
