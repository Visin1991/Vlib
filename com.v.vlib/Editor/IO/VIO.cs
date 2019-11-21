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

        public static string Full_Path(GameObject obj)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            return Path.GetFullPath(assetPath).Replace("\\", "/");
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


        //-----------------------------------------------------------------------------------------------------------------------------------

        public static class MeshUtil
        {
            public static Mesh CopyMeshToDst(MeshFilter mf, string directory, bool auto2UV)
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

                        if (auto2UV)
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
        }

        public static class MaterialUtil
        {
            public static Material[] CopyMaterialsToDst(MeshRenderer mr, string dstDirectory, System.Func<MaterialInfo, Material> func)
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
                                else
                                {
                                    material = func(info);
                                }

                                string sourceD = VIO.UnitySlash(info.path_D);
                                Texture diffuseTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceD);
                                material.SetTexture("_MainTex", diffuseTex);

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

            public static Material[] CopySPBRMaterialsAndTexturesToDst(MeshRenderer mr, string dstDirectory, System.Func<MaterialInfo, Material> func)
            {
                if (mr)
                {
                    MaterialInfo[] matInfos = GraphicsUtili.Get_MaterialInfo(mr, " ");
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

                                material.SetTexture("_MainTex", TextureUtil.CopyTextureToDest(info.path_D, dstDirectory));
                                material.SetTexture("_BumpMap", TextureUtil.CopyTextureToDest(info.path_N, dstDirectory));
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
        }

        public static class AssetUtil
        {
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
        }
        
        public static class TextureUtil
        {
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

            public static string GetTexturePath(Texture2D texture)
            {
                return AssetDatabase.GetAssetPath(texture);
            }


            public static Texture2D CreateTexture_Linear(RenderTexture renderTextrue)
            {
                if (renderTextrue == null) { return null; }

                TextureFormat format = TextureFormat.RGBA32;
                Texture2D texture = new Texture2D(renderTextrue.width, renderTextrue.height, format, false, true);
                RenderTexture.active = renderTextrue;
                texture.ReadPixels(new Rect(0, 0, renderTextrue.width, renderTextrue.height), 0, 0);
                texture.Apply();
                return texture;
            }

            public static Texture2D CreateTexture(RenderTexture renderTextrue)
            {
                if (renderTextrue == null) { return null; }

                TextureFormat format = TextureFormat.RGBA32;
                Texture2D texture = new Texture2D(renderTextrue.width, renderTextrue.height, format, false, false);
                RenderTexture.active = renderTextrue;
                texture.ReadPixels(new Rect(0, 0, renderTextrue.width, renderTextrue.height), 0, 0);
                texture.Apply();
                return texture;
            }

            

            public static Texture CopyTextureToDest(string path, string dstDirectory)
            {
                string sourceD = VIO.UnitySlash(path);
                Texture sourceDTex = AssetDatabase.LoadAssetAtPath<Texture>(sourceD);
                if (sourceDTex != null)
                {
                    string dstD = dstDirectory + "/" + System.IO.Path.GetFileName(sourceD);
                    VIO.CopyBinaryFile(sourceD, dstD);
                    AssetDatabase.Refresh();
                    Texture dstTex = VIO.TextureUtil.GetTexture(dstD);
                    return dstTex;
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

            public static Texture2D WriteTextureToDisk(Texture2D textureCache, string targetPath)
            {
                if (textureCache == null) { return null; }
                System.IO.File.WriteAllBytes(targetPath, ImageConversion.EncodeToPNG(textureCache));
                AssetDatabase.ImportAsset(targetPath);
                Object.DestroyImmediate(textureCache);

                //TextureImporter textureImporter = AssetImporter.GetAtPath(targetPath) as TextureImporter;
                //PlatFormTextureSettings_RGBA(textureImporter);

                return AssetDatabase.LoadMainAssetAtPath(targetPath) as Texture2D;
            }

            private static void PlatformTextureSettings_Default(TextureImporter Importer, bool sRGB, VTextureFormat vformat)
            {
                if (vformat == VTextureFormat.R8 || vformat == VTextureFormat.RGB)
                {
                    Importer.alphaSource = TextureImporterAlphaSource.None;
                }
                else
                {
                    Importer.alphaSource = Importer.DoesSourceTextureHaveAlpha() ? TextureImporterAlphaSource.FromInput : TextureImporterAlphaSource.None;
                }

                Importer.textureType = TextureImporterType.Default;
                Importer.textureShape = TextureImporterShape.Texture2D;
                Importer.sRGBTexture = sRGB;
                Importer.isReadable = false;

                Importer.alphaIsTransparency = Importer.DoesSourceTextureHaveAlpha();
                Importer.npotScale = TextureImporterNPOTScale.ToNearest;
                Importer.mipmapEnabled = true;
                Importer.wrapMode = TextureWrapMode.Repeat;
                Importer.filterMode = FilterMode.Bilinear;
                Importer.anisoLevel = 1;
                Importer.maxTextureSize = 4096;



                //===========================================iOS Setting============================================================
                Importer.textureCompression = TextureImporterCompression.Compressed;
                TextureImporterPlatformSettings iosSetting = Importer.GetPlatformTextureSettings("iPhone");
                iosSetting.overridden = true;
                iosSetting.maxTextureSize = 4096;
                if (vformat == VTextureFormat.HDR)
                {
                    iosSetting.format = TextureImporterFormat.ASTC_HDR_4x4;
                    iosSetting.compressionQuality = 100;
                }
                else if (vformat == VTextureFormat.RGB)
                {
                    iosSetting.format = TextureImporterFormat.ASTC_8x8;
                    iosSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R8)
                {
                    iosSetting.format = TextureImporterFormat.R8;
                    iosSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R16)
                {
                    iosSetting.format = TextureImporterFormat.R16;
                    iosSetting.compressionQuality = 50;
                }
                else
                {
                    iosSetting.format = TextureImporterFormat.ASTC_4x4;
                }
                Importer.SetPlatformTextureSettings(iosSetting);

                //===========================================Android Setting============================================================
                TextureImporterPlatformSettings androidSetting = Importer.GetPlatformTextureSettings("Android");
                androidSetting.overridden = true;
                androidSetting.maxTextureSize = 4096;
                if (vformat == VTextureFormat.HDR)
                {
                    //TODO 
                    // On Android Platform, We ETC2_RGBA8 for HDR and Lightmap.  However this is a not proper way to do this.
                    // And we should do something else.
                    iosSetting.format = TextureImporterFormat.ETC2_RGBA8;
                    androidSetting.compressionQuality = 100;
                }
                else if (vformat == VTextureFormat.RGB)
                {
                    iosSetting.format = TextureImporterFormat.ETC2_RGB4;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R8)
                {
                    iosSetting.format = TextureImporterFormat.R8;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R16)
                {
                    iosSetting.format = TextureImporterFormat.R16;
                    androidSetting.compressionQuality = 50;
                }
                else
                {
                    iosSetting.format = TextureImporterFormat.ETC2_RGBA8;
                    androidSetting.compressionQuality = 100;
                }

                Importer.SetPlatformTextureSettings(androidSetting);


                //===========================================Default Setting============================================================
                TextureImporterPlatformSettings defaultSetting = Importer.GetPlatformTextureSettings("Default");
                defaultSetting.overridden = true;
                defaultSetting.maxTextureSize = 4096;
                if (vformat == VTextureFormat.HDR)
                {
                    iosSetting.format = TextureImporterFormat.RGBAHalf;
                    androidSetting.compressionQuality = 100;
                }
                else if (vformat == VTextureFormat.RGB)
                {
                    iosSetting.format = TextureImporterFormat.RGB24;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R8)
                {
                    iosSetting.format = TextureImporterFormat.R8;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R16)
                {
                    iosSetting.format = TextureImporterFormat.R16;
                    androidSetting.compressionQuality = 50;
                }
                else
                {
                    iosSetting.format = TextureImporterFormat.RGBA32;
                    androidSetting.compressionQuality = 100;
                }
                Importer.SetPlatformTextureSettings(defaultSetting);


                TextureImporterPlatformSettings standaloneSetting = Importer.GetPlatformTextureSettings("Standalone");
                standaloneSetting.overridden = true;
                standaloneSetting.maxTextureSize = 4096;
                if (vformat == VTextureFormat.HDR)
                {
                    iosSetting.format = TextureImporterFormat.RGBAHalf;
                    androidSetting.compressionQuality = 100;
                }
                else if (vformat == VTextureFormat.RGB)
                {
                    iosSetting.format = TextureImporterFormat.DXT1;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R8)
                {
                    iosSetting.format = TextureImporterFormat.R8;
                    androidSetting.compressionQuality = 50;
                }
                else if (vformat == VTextureFormat.R16)
                {
                    iosSetting.format = TextureImporterFormat.R16;
                    androidSetting.compressionQuality = 50;
                }
                else
                {
                    iosSetting.format = TextureImporterFormat.DXT5;
                    androidSetting.compressionQuality = 100;
                }
                Importer.SetPlatformTextureSettings(standaloneSetting);

                Importer.SaveAndReimport();
            }

            public static void PlatFormTextureSettings_R8(TextureImporter Importer)
            {
                PlatformTextureSettings_Default(Importer, false, VTextureFormat.R8);
            }

            public static void PlatFormTextureSettings_R16(TextureImporter importer)
            {
                PlatformTextureSettings_Default(importer, false, VTextureFormat.R16);
            }

            public static void PlatFormTextureSettings_RGBA(TextureImporter importer)
            {
                PlatformTextureSettings_Default(importer, false, VTextureFormat.RGBA);
            }

            enum VTextureFormat
            {
                RGB,
                RGBA,
                R8,
                R16,
                HDR
            }

        }

        public static class PrefabUtil
        {
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
        }

    }


}