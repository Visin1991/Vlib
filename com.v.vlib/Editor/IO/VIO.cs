using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace V
{
    public static class VIO
    {
        //-----------------------------------------------------------------------------------------------------------------------------
        public static void MakePrefab_LOD0(string directory, string objName, Mesh mesh, Material[] mats)
        {
            string path = directory + "/" + objName;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                if (!EditorUtility.DisplayDialog("Warning!!!!", "Prefab 已经存在，事发选择覆盖", "YES", "No"))
                {
                    return;
                }
            }

            GameObject gameObject = new GameObject(objName);
            LODGroup group = gameObject.AddComponent<LODGroup>();
            GameObject lod0 = new GameObject("LOD0");
            lod0.transform.SetParent(gameObject.transform);
            MeshRenderer mrNew = lod0.AddComponent<MeshRenderer>();
            MeshFilter mfNew = lod0.AddComponent<MeshFilter>();
            mfNew.sharedMesh = mesh;
            mrNew.sharedMaterials = mats;

            Renderer[] renderers = lod0.GetComponents<Renderer>();
            LOD lod = new LOD(0.05f, renderers);
            LOD[] lods = new LOD[1];
            lods[0] = lod;
            group.SetLODs(lods);

            PrefabUtility.SaveAsPrefabAsset(gameObject, path + ".prefab");
        }

        public static Material[] CopyMaterialsToDst(MeshRenderer mr, string dstDirectory,System.Func<MaterialInfo,Material> func)
        {
            if (mr)
            {

                MaterialInfo[] materialInfos = GraphicsUtili.Get_MaterialInfo(mr, "_PBRTexture");

                if (materialInfos != null)
                {
                    Material[] materials = new Material[materialInfos.Length];
                    for (int i = 0; i < materials.Length; i++)
                    {
                        MaterialInfo info = materialInfos[i];
                        bool spbr = false;
                        if (info.path_P == null) { spbr = true; }
                        else
                        {
                            if (info.path_P.Length <= 0) { spbr = true; }
                        }

                        string materialPath = ToUnity(dstDirectory + "/" + info.matName + ".mat");
                        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
                        if (material == null)
                        {
                            if (func == null)
                            {
                                if (spbr)
                                {
                                    material = new Material(Shader.Find("MSO/Standard/SPBR"));
                                }
                                else
                                {
                                    material = new Material(Shader.Find("MSO/Standard/PBR"));
                                }
                                
                            }
                            else {
                                material = func(info);
                            }

                            string sourceD = VIO.UnitySlash(info.path_D);
                            Texture diffuseTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceD);
                            material.SetTexture("_MainTex",diffuseTex);

                            string sourceN = VIO.UnitySlash(info.path_N);
                            Texture normalTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceN);
                            material.SetTexture("_BumpMap", normalTex);

                            string sourceP = VIO.UnitySlash(info.path_P);
                            Texture pbrTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceP);
                            material.SetTexture("_MetallicGlossMap", pbrTex);

                            AssetDatabase.CreateAsset(material, materialPath);
                            AssetDatabase.Refresh();
                        }
                        materials[i] = material;
                    }
                    return materials;
                }
            }
            return null;
        }

        public static Material[] CopySPBRMaterialsAndTexturesToDst(MeshRenderer mr, string dstDirectory,System.Func<MaterialInfo,Material> func)
        {
            if (mr)
            {
                MaterialInfo[] matInfos = GraphicsUtili.Get_MaterialInfo(mr," ");
                if (matInfos != null)
                {
                    Material[] materials = new Material[matInfos.Length];
                    for (int i = 0; i < matInfos.Length; i++)
                    {
                        MaterialInfo info = matInfos[i];
                        string materialPath = ToUnity(dstDirectory + "/" + info.matName + ".mat");
                       
                        Material material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);

                        if (material == null)
                        {

                            if (func == null)
                            {
                                material = new Material(Shader.Find("MSO/Standard/SPBR"));

                            }
                            else
                            {
                                material = func(info);
                            }

                            material.SetTexture("_MainTex", CopyTextureToDest(info.path_D, dstDirectory));
                            material.SetTexture("_BumpMap", CopyTextureToDest(info.path_N, dstDirectory));
                            AssetDatabase.CreateAsset(material, materialPath);
                            AssetDatabase.Refresh();
                        }

                        materials[i] = material;
                    }
                    return materials;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }

        public static Texture CopyTextureToDest(string path,string dstDirectory)
        {
            string sourceD = VIO.UnitySlash(path);
            Texture sourceDTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceD);
            if (sourceDTex != null)
            {
                string dstD = dstDirectory + "/" + System.IO.Path.GetFileName(sourceD);
                VIO.CopyBinaryFile(sourceD, dstD);
                AssetDatabase.Refresh();
                Texture dstTex = VIO.GetTexture(dstD);
                return dstTex;
            }
            return null;
        }

        public static Mesh CopyMeshToDst(MeshFilter mf, string directory,bool auto2UV)
        {
            if (mf != null)
            {
                Mesh mesh = mf.sharedMesh;
                if (mesh != null)
                {
                    if (!directory.Contains(VIO.WorkSpace())) { return null; }

                    Mesh newMesh = new Mesh();
                    newMesh.vertices = mesh.vertices;
                    newMesh.uv = mesh.uv;
                    newMesh.normals = mesh.normals;
                    newMesh.triangles = mesh.triangles;
                    newMesh.bounds = mesh.bounds;
                    newMesh.subMeshCount = mesh.subMeshCount;

                    for (int i = 0; i < newMesh.subMeshCount; i++)
                    {
                        newMesh.SetTriangles(mesh.GetTriangles(i), i);
                    }

                    if(auto2UV)
                        Unwrapping.GenerateSecondaryUVSet(newMesh);

                    string unityPath = ToUnity(directory + "/" + mesh.name);
                    AssetDatabase.CreateAsset(newMesh, unityPath + ".asset");
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
                return mesh;
            }
            else
            {
                return null;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------
        public static string UnitySaveFile(string directory = "Asset",string defualtName = "Non",string extension = "asset")
        {
            string filePath = EditorUtility.SaveFilePanel("Save", directory, defualtName, extension);
            if (filePath.Contains(Path.GetFullPath("Assets").Replace('\\', '/')))
            {
                return filePath;
            }
            else
            {
                EditorUtility.DisplayDialog("错误","对不起，您选择的路径不是当前项目的工作路径","OK");
                return null;
            } 
        }

        public static string UnitySaveFolder(string directory = "Asset")
        {
            string directoryPath = EditorUtility.SaveFolderPanel("Save", directory,"");
            if (directoryPath.Contains(Path.GetFullPath("Assets").Replace('\\', '/')))
            {
                return directoryPath;
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "对不起，您选择的路径不是当前项目的工作路径", "OK");
                return null;
            }
        }

        public static string UnitySlash(string path)
        {
            if (path == null) { return ""; }
            return path.Replace("\\", "/");
        }

        public static string ToUnity(string path)
        {
            path = path.Replace('\\', '/');
            string workSpace = Path.GetFullPath("Assets").Replace('\\', '/');
            if (path.Contains(workSpace))
                return "Assets" + path.Substring(workSpace.Length);
            else
                return null;
        }

        public static void WriteToFile(string s, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.Write(s);
            }
        }

        public static string Current_Slected_Asset_Directory()
        {
            var path = "Assets";
            var obj = Selection.activeObject;
            if (obj == null) { return path; }
            path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
            if (path.Length > 0)
            {
                return Path.GetDirectoryName(path);
            }
            return path;
        }

        public static string Unity_Directory(GameObject prefab)
        {
            if (prefab == null) return "";
            string path = AssetDatabase.GetAssetPath(prefab.GetInstanceID());
            if (path.Length > 0)
            {
                return Path.GetDirectoryName(path);
            }
            return "";
        }

        public static string Full_Directory(GameObject prefab)
        {
            if (prefab == null) return "";
            string path = AssetDatabase.GetAssetPath(prefab.GetInstanceID());
            if (path.Length > 0)
            {
                path = Path.GetFullPath(path);
                return Path.GetDirectoryName(path).Replace("\\", "/");
            }
            return "";
        }

        public static string SelecteADirectory(string directory)
        {
            return EditorUtility.OpenFolderPanel("", directory, "");
        }

        public static string WorkSpace()
        {
            return Path.GetFullPath("Assets").Replace('\\', '/');
        }

        public static void DirectoryTaskTree(string directory, System.Action<string> dirCallback = null)
        {
            //First Call the Root Directory
            if (dirCallback != null) { dirCallback(directory); }
            string[] subs = Directory.GetDirectories(directory);
            foreach (string sub in subs)
            {
                string ssub = sub.Replace("\\", "/");
                DirectoryTaskTree(ssub, dirCallback);
            }
        }

        public static string GetFolderName(string folderPath)
        {
            string[] folders = folderPath.Split(new char[] { '/', '\\' });
            string foldername = "";
            if (folders.Length > 1)
                foldername = folders[folders.Length - 1];
            else if(folders.Length == 1)
            {
                foldername = folders[0];
            }
            return foldername;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------

        public static string Full_Path(GameObject obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            return Path.GetFullPath(assetPath).Replace("\\", "/");
        }

        [MenuItem("V/IO/CopyPath")]
        static void CopyAssetPath2Clipboard()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            TextEditor text2Editor = new TextEditor();
            text2Editor.text = path.ToLower();
            text2Editor.OnFocus();
            text2Editor.Copy();
        }

        public static string GetAssetSeletionPath()
        {
            string[] guids = Selection.assetGUIDs;
            if (guids != null)
            {
                return AssetDatabase.GUIDToAssetPath(guids[0]);
            }
            else
            {
                return "";
            }
        }

        public static List<string> GetAssetSeletionPaths()
        {
            string[] guids = Selection.assetGUIDs;
            List<string> paths = new List<string>();
            for (int i = 0; i < guids.Length; i++)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guids[i]));
            }
            return paths;
        }

        public static bool AssetPathTo_ModelImporter(ref ModelImporter _importer, string path)
        {
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
            {
                return false;
            }
            _importer = importer;
            return true;
        }

        public static bool CopyBinaryFile(string srcfilename, string destfilename)
        {
            if (System.IO.File.Exists(srcfilename) == false)
            {
                Debug.Log("Could not find The Source file " + srcfilename);
                return false;
            }
            if (System.IO.File.Exists(destfilename) == true)
            {
                return false;
            }

            System.IO.Stream s1 = System.IO.File.Open(srcfilename, System.IO.FileMode.Open);
            System.IO.Stream s2 = System.IO.File.Open(destfilename, System.IO.FileMode.Create);

            System.IO.BinaryReader f1 = new System.IO.BinaryReader(s1);
            System.IO.BinaryWriter f2 = new System.IO.BinaryWriter(s2);

            while (true)
            {
                byte[] buf = new byte[10240];
                int sz = f1.Read(buf, 0, 10240);

                if (sz <= 0)
                    break;

                f2.Write(buf, 0, sz);

                if (sz < 10240)
                    break; // eof reached
            }
            f1.Close();
            f2.Close();
            return true;
        }

        static public string GetTextureFullName(string directory, string filename)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(directory);
            FileInfo[] fis = di.GetFiles();
            foreach (FileInfo fi in fis)
            {
                //Debug.Log(" Compare : " + fi.Name);
                if (string.Compare(Path.GetFileNameWithoutExtension(fi.Name), filename, true) == 0)
                {
                    //Debug.Log("Find Target :" + fi.Name);
                    directory = directory.Replace("\\", "/");
                    string textureAssetPath = ToUnity(directory + "/" + fi.Name);
                    TextureImporter ti = AssetImporter.GetAtPath(textureAssetPath) as TextureImporter;
                    if (ti != null)
                    {
                        //Debug.Log("Find Texture Name :" + fi.Name);
                        return fi.Name;
                    }
                    else
                    {
                        Debug.LogError("Target Texture Not Load to Engine : " + textureAssetPath);
                    }

                }
            }
            return null;
        }

        static public Texture2D GetTexture(string path)
        {
            //Debug.Log("Try To Get Texture From the Path : " + path);
            path = path.Replace("\\", "/");
            path = ToUnity(path);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti != null)
            {
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            Debug.LogError("Path Error : " + path);
            return null;
        }

    }


}