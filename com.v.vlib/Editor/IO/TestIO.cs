//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.IO;

//namespace V
//{
//    public class TestGeneric : EditorWindow
//    {
//        [MenuItem("V/Test/Generic")]
//        public static void DoTest()
//        {
//            var window = EditorWindow.GetWindow<TestGeneric>();
//            window.titleContent = new GUIContent("Test");
//            window.Show();
//        }

//        private GameObject testObject;
        

//        private void OnGUI()
//        {
//            testObject = (GameObject)EditorGUILayout.ObjectField("Test Object", testObject, typeof(GameObject), false);

//            if (GUILayout.Button("Asset Path"))
//            {
//                if (testObject != null)
//                {
//                    string assetPath = AssetDatabase.GetAssetPath(testObject);
//                    Debug.Log(assetPath);
//                }
//            }

//            if (GUILayout.Button("Unity Directory"))
//            {
//                if (testObject != null)
//                {
//                    string assetPath = AssetDatabase.GetAssetPath(testObject);
//                    string directory = Path.GetDirectoryName(assetPath);
//                    Debug.Log(directory);
//                }
//            }

//            if (GUILayout.Button("Unity Save Panel Path"))
//            {
//                string defaultDirectory = V.VIO.ToUnity(EditorUtility.SaveFilePanel("储存CubeMap", "Assets", "_Cube", "png"));
//                Debug.Log(defaultDirectory);
//            }

//            if (GUILayout.Button("Unity Directory -- Curren Selected"))
//            {
//                Debug.Log(VIO.Current_Slected_Asset_Directory());
//            }


//            if (GUILayout.Button("Full Path"))
//            {
//                if (testObject != null)
//                {
//                    Debug.Log(VIO.Full_Path(testObject));
//                }
//            }

//            if (GUILayout.Button("Full Directory"))
//            {
//                if (testObject != null)
//                {
//                    Debug.Log(VIO.Full_Directory(testObject));
//                }
//            }

//            if (GUILayout.Button("Work Space"))
//            {
//                Debug.Log(VIO.WorkSpace());
//            }



//            if (GUILayout.Button("File Exist"))
//            {
//                string workPath = VIO.Full_Path(testObject);
//                if (File.Exists(workPath))
//                {
//                    Debug.Log(" File:  " + workPath + " already exist");
//                }
//            }

//            if (GUILayout.Button("Texture Exist"))
//            {
//                string assetPath = AssetDatabase.GetAssetPath(testObject);
//                string directory = Path.GetDirectoryName(assetPath);
//                string filename = testObject.name;
//                Debug.Log("Work Directory : " + directory);
//                System.IO.DirectoryInfo di = new DirectoryInfo(directory);
//                FileInfo[] fis = di.GetFiles();
//                foreach (FileInfo fi in fis)
//                {
//                    //Debug.Log("File name: " +fi.Name + " ObjectName: " + filename);
//                    if (string.Compare(Path.GetFileNameWithoutExtension(fi.Name), filename, true) == 0)
//                    {
//                        Debug.Log("Find Target: " + fi.Name);
//                        TextureImporter ti = AssetImporter.GetAtPath(directory+"/" + fi.Name) as TextureImporter;
//                        if (ti != null)
//                        {
//                            Debug.Log("Texture with the same Name with GameObject Exsit");
//                            Debug.Log("Texture with Extention : " +fi.Extension);
//                            break;
//                        }
//                    }
//                }
//            }

//            if (GUILayout.Button("寻找Scene Script"))
//            {
//                //LOD_GROUP_TEST comp = GameObject.FindObjectOfType<LOD_GROUP_TEST>();
//                //if(comp!=null)
//                //    Selection.activeGameObject = comp.gameObject;
//            }

//            if (GUILayout.Button("Save File Test"))
//            {
//                VIO.UnitySaveFile();
//            }
//        }


//    }
//}
