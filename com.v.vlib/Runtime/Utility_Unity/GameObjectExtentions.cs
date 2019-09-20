using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace V
{
    public static class GameObjectExtentions
    {
        //==========================================================================
        #region Mesh
        public static string vGetShadersInfo(this GameObject obj)
        {
            string content = "当前物件使用Shader 为  \n";
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers == null) { return "物件没有Renderer"; }
            foreach (Renderer r in renderers)
            {
                if (r.sharedMaterials == null) { content += " 物件缺少Materials \n"; continue; }

                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat == null)
                    {
                        content += "Material或者Shader有误";
                    }
                    else
                    {
                        content += mat.shader.name;
                    }

                    content += "\n";
                }
            }
            return content;
        }

        public static string vGetMeshDataInfo(this GameObject obj)
        {
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            string content = "MeshFilter数量：" + meshFilters.Length.ToString();

            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.sharedMesh == null)
                {
                    content += "Mesh 数据丢失";
                }
                else
                {
                    content += " 顶点数： ";
                    content += mf.sharedMesh.vertexCount.ToString();
                }
                content += "\n";
            }
            return content;

        }

        public static string vGetMeshBoundsInfo(this GameObject obj)
        {
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            string content = "MeshFilter数量：" + meshFilters.Length.ToString();

            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.sharedMesh == null)
                {
                    content += "Mesh 数据丢失";
                }
                else
                {
                    content += "Mesh Bounds大小为： ";
                    content += mf.sharedMesh.bounds.size.Multiply(obj.transform.localScale).ToString();
                }
                content += "\n";
            }
            return content;
        }

        public static string vGetMeshVerticesBoundsInfo(this GameObject obj)
        {
            MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
            string content = "MeshFilter数量：" + meshFilters.Length.ToString();

            foreach (MeshFilter mf in meshFilters)
            {
                if (mf.sharedMesh == null)
                {
                    content += "Mesh 数据丢失";
                }
                else
                {
                    content += " 顶点数： ";
                    content += mf.sharedMesh.vertexCount.ToString();
                    content += "\n";
                    content += "Mesh Bounds大小为： ";
                    content += mf.sharedMesh.bounds.size.Multiply(obj.transform.localScale).ToString();

                }
                content += "\n";
            }
            return content;
        }

        public static Vector3 vGetMeshBoundSize(this GameObject obj)
        {
            MeshFilter meshFilter = obj.GetComponentInChildren<MeshFilter>();
            if (meshFilter == null) { return new Vector3(-999, -999, -999); }
            if (meshFilter.sharedMesh == null)
            {
                return new Vector3(-999, -999, -999);
            }
            else
            {
                return meshFilter.sharedMesh.bounds.size.Multiply(obj.transform.localScale);
            }    
        }

        #endregion
        //==========================================================================
        #region Renderer

        public static bool vCheckAndCache_RenderMat(this GameObject _obj, ref Renderer _renderer, ref Material _mat)
        {
            if (_renderer != null && _mat != null)
            {
                return true;
            }
            else
            {
                _renderer = _obj.GetComponent<Renderer>();
                if (_renderer == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Mesh Renderer", _obj.name);
                    return false;
                }

                _mat = _renderer.sharedMaterial;

                if (_mat == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Material", _obj.name);
                    return false;
                }
                return true;
            }
        }

        public static bool vCheckAndCache_RenderMats(this GameObject _obj, ref Renderer _renderer, ref Material[] _mat)
        {
            if (_renderer != null && _mat != null)
            {
                return true;
            }
            else
            {
                _renderer = _obj.GetComponent<Renderer>();
                if (_renderer == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Mesh Renderer", _obj.name);
                    return false;
                }

                _mat = _renderer.sharedMaterials;

                if (_mat == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Material", _obj.name);
                    return false;
                }
                return true;
            }
        }

        public static bool vCheckAndCache_RenderMat(this GameObject _obj, ref Renderer _renderer, ref Material _mat, System.Func<bool> callBack)
        {
            if (_renderer != null && _mat != null)
            {
                return true;
            }
            else
            {
                _renderer = _obj.GetComponent<Renderer>();
                if (_renderer == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Mesh Renderer", _obj.name);
                    return false;
                }

                _mat = _renderer.sharedMaterial;

                if (_mat == null)
                {
                    Debug.LogWarningFormat("GameObject ::<0> don't have a Material", _obj.name);
                    return false;
                }

                if (callBack())
                {
                    return true;
                }
                else
                {
                    Debug.LogFormat("反调函数 {0} 发生错误", callBack.ToString());
                    return false;
                }
            }
        }

        #endregion
        //==========================================================================
        #region Camera

        public static float vDistanceToMainCamera(this GameObject obj)
        {
            if (Camera.main == null) {
                Debug.Log("当前场景没有 Main Camera");
                return -999.9999f;
            }
            else
                return (obj.transform.position - Camera.main.transform.position).magnitude;
        }

        public static bool vCheckAndCache_Camera(this GameObject obj, ref Camera camera)
        {
            if (camera == null)
            {
                camera = obj.GetComponent<Camera>();
                if (camera == null)
                {
                    Debug.Log("当前物体没有Camera 部件"); return false;
                }
                return true;
            }
            else {
                return true;
            }
        }

        #endregion

        //==========================================================================
        #region Component

        public static bool vCheckAndCache_Component<T>(this GameObject obj,ref T component) where T : Component
        {
            if (component == null)
            {
                component = obj.GetComponent<T>();
                if (component == null) {
                    Debug.LogFormat("物件 {0} 没有所需要部件 : {1}", obj.name, typeof(T).ToString());
                    return false;
                }
                return true;
            }
            return true;
        }

        public static bool vCheckAndCache_Component<T>(this GameObject obj, ref T component,System.Func<bool> callback) where T : Component
        {
            if (component == null)
            {
                component = obj.GetComponent<T>();
                if (component == null)
                {
                    Debug.LogFormat("物件 {0} 没有所需要部件 : {1}", obj.name, typeof(T).ToString());
                    return false;
                }
                if (callback())
                {
                    return true;
                }
                else
                {
                    Debug.LogWarningFormat("反调函数 {0} 发生错误", callback.ToString());
                }
            }
            return true;
        }

        public static bool vCheckAndCache_Components_InChildren<T>(this GameObject obj,ref T[] components) where T : Component
        {
            if (components == null || components.Length <= 0)
            {
                components = obj.GetComponentsInChildren<T>();
                if (components.Length <= 0)
                {
                    Debug.LogFormat("物件 {0} 以及 其子物件没有所需要部件 : {1}", obj.name, typeof(T).ToString());
                    return false;
                }
                else {
                    return true;
                }
            }
            return true;
        }

        public static bool vCheckAndCache_Components_InChildren<T>(this GameObject obj, ref T[] components, System.Func<bool> callBack) where T : Component
        {
            if (components == null || components.Length <= 0)
            {
                components = obj.GetComponentsInChildren<T>();
                if (components.Length <= 0)
                {
                    Debug.LogFormat("物件 {0} 以及 其子物件没有所需要部件 : {1}", obj.name, typeof(T).ToString());
                    return false;
                }
                else
                {
                    if (callBack())
                    {
                        return true;
                    }
                    else
                    {
                        Debug.LogFormat("反调函数 {0} 发生错误", callBack.ToString());
                        Debug.LogWarningFormat("反调函数 {0} 发生错误", callBack.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

#if UNITY_EDITOR

        public static void vCreateNew_BillbardAsset_FromObjects(this GameObject obj,string directory,string billBoardAssetExtendname = "",bool applyNewToObj = true)
        {
            BillboardRenderer billboardRenderer = obj.GetComponentInChildren<BillboardRenderer>();
            if (billboardRenderer != null)
            {
                if (billboardRenderer.billboard == null) { Debug.Log("Billboard is Null"); return; }
                BillboardAsset newBillboardAsset = new BillboardAsset();
                newBillboardAsset.width = billboardRenderer.billboard.width;
                newBillboardAsset.height = billboardRenderer.billboard.height;
                newBillboardAsset.bottom = billboardRenderer.billboard.bottom;
                newBillboardAsset.material = billboardRenderer.sharedMaterial;

                string billboardAssetPath = directory + "/" + obj.name + billBoardAssetExtendname + "_Billboard.asset";
                AssetDatabase.CreateAsset(newBillboardAsset, billboardAssetPath);
                AssetDatabase.Refresh();
                if(applyNewToObj)
                    billboardRenderer.billboard = newBillboardAsset;
            }
        }
#endif

#endregion

    }
}
