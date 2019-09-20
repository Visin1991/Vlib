using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;

namespace V
{
    public static class VObjExporter {

        static bool makeSubmeshes = true;
        
        [MenuItem("Tools/V/Export To Obj")]
        public static void ExportObj()
        {
            if (EditorUtility.DisplayDialog("V","你是否要将物件Mesh存为 Obj 格式，并将Mesh合并","OK"))
            {

                if (Selection.gameObjects.Length == 0) { Debug.Log("Didn't Export Any Meshes; Nothing was Selected!"); return; }
                string meshName = Selection.gameObjects[0].name;
                makeSubmeshes = false;
                VFile.SaveFile(SaveOBJFile, "obj");
            }
        }

        [MenuItem("Tools/V/Export To Obj_subMesh")]
        public static void ExportObj_Submesh()
        {
            if (VDialog.DisplayDialog("你是否要将物件Mesh存为 OBJ 格式，并存为多个Sub-Mesh"))
            {
                if (Selection.gameObjects.Length == 0) { Debug.Log("Didn't Export Any Meshes; Nothing was Selected!"); return; }
                string meshName = Selection.gameObjects[0].name;
                makeSubmeshes = true;
                VFile.SaveFile(SaveOBJFile, "obj");
            }
        }

        [MenuItem("Tools/V/Print_Sub_MeshAcount")]
        public static void ExportObj_Sub_mesh()
        {
            if (VDialog.DisplayDialog("你是否要将物件Mesh存为 OBJ 格式，并存为多个Sub-Mesh"))
            {
                if (Selection.gameObjects.Length == 0) { Debug.Log("Didn't Export Any Meshes; Nothing was Selected!"); return; }
                string meshName = Selection.gameObjects[0].name;
                makeSubmeshes = true;
                VFile.SaveFile(SaveOBJFile, "obj");
            }
        }

        public static void SaveOBJFile(string path)
        {
            MeshToObjString.Start();
            StringBuilder meshString = new StringBuilder();

            Transform t = Selection.gameObjects[0].transform;

            Vector3 originalPosition = t.position;
            t.position = Vector3.zero;

            if (!makeSubmeshes)
            {
                meshString.Append("g ").Append(t.name).Append("\n");
            }
            meshString.Append(processTransform(t, makeSubmeshes));

            VIO.WriteToFile(meshString.ToString(), path);

            t.position = originalPosition;

            MeshToObjString.End();
        }

        static string processTransform(Transform t,bool makeSubmeshes)
        {
            StringBuilder meshString = new StringBuilder();
 
		    meshString.Append("#" + t.name + "\n#-------" 
						      + "\n");
 
		    if (makeSubmeshes)
		    {
			    meshString.Append("g ").Append(t.name).Append("\n");
		    }
 
		    MeshFilter mf = t.GetComponent<MeshFilter>();

		    if (mf)
		    {

			    meshString.Append(MeshToObjString.Do(mf, t));
		    }
 
		    for(int i = 0; i < t.childCount; i++)
		    {
			    meshString.Append(processTransform(t.GetChild(i), makeSubmeshes));
		    }
 
		    return meshString.ToString();
        }

        static class VDialog
        {
            public static bool DisplayDialog(string message)
            {
                return EditorUtility.DisplayDialog("V", message, "Yes", "Cancel");
            }
        }

        static class VFile
        {
            public static void SaveFile(System.Action<string> action, string extention = "txt", string defaultName = "Non")
            {
                string path = EditorUtility.SaveFilePanel("V", "", defaultName, extention);
                if (path.Length != 0)
                {
                    action(path);
                }
            }

            public static string SaveFile(string extention = "txt", string defaultName = "Non")
            {
                return EditorUtility.SaveFilePanel("V", "", defaultName, extention);
            }

        }

    }
}
