using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;
using System.Text;

public class V_Unity_PackagCreator : EditorWindow
{
    static V_Unity_PackagCreator window;
    [MenuItem("V/Package/Create Package")]
    static void CreateWindow()
    {
        window = EditorWindow.GetWindow<V_Unity_PackagCreator>();
        window.maxSize = new Vector2(700, 100);
        window.Show();
    }

    string NameSpace = "V";
    string PackageDescription = "this is a template package";
    string ProjectName = "Template";
    string UnityVersion = "2019.1";
    string PackageVersion = "0.0.1";
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("NameSpace : ", GUILayout.MaxWidth(100));
        NameSpace = GUILayout.TextField(NameSpace);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Description : ", GUILayout.MaxWidth(100));
        PackageDescription = GUILayout.TextField(PackageDescription);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("DisplayName : ", GUILayout.MaxWidth(100));
        ProjectName = GUILayout.TextField(ProjectName);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("UnityVersion : ", GUILayout.MaxWidth(100));
        UnityVersion = GUILayout.TextField(UnityVersion);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("PackageVersion : ", GUILayout.MaxWidth(100));
        PackageVersion = GUILayout.TextField(PackageVersion);
        GUILayout.EndHorizontal();





        if (GUILayout.Button("Create Package"))
        {
            string path = EditorUtility.SaveFolderPanel("Select Package Location", "F:/VUnityPakage", "");
            if (String.IsNullOrEmpty(path))
            {
                Debug.Log("Invalid Directory Path");
                return;
            }

            CreatePackage(path);

            window.Close();
        }
    }

    void CreatePackage(string path)
    {
        string packageName = ("com." + NameSpace + "." + ProjectName).ToLower();
        string directoryPath = path + "/" + packageName;
        DirectoryInfo directoryInfo = Directory.CreateDirectory(directoryPath);
        if (directoryInfo.Exists)
        {
            StringBuilder packageInfo = new StringBuilder();


            packageInfo.Append("{" + "\n");

            //Dependencies
            //-----------------------------------------------
            packageInfo.Append("    \"dependencies\":\n");
            packageInfo.Append("    {\n");
            //packageInfo.Append("        \"com.v.vlib\" \n");
            packageInfo.Append("    },\n");
            //-----------------------------------------------

            packageInfo.Append("    " + StringColonPare_n("description", PackageDescription) + ",\n");
            packageInfo.Append("    " + StringColonPare_n("displayName", ProjectName) + ",\n");
            packageInfo.Append("    " + StringColonPare_n("name", packageName) + ",\n");
            packageInfo.Append("    " + StringColonPare_n("repoPackagePath", packageName) + ",\n");

            packageInfo.Append("    " + StringColonPare_n("unity", UnityVersion) + ",\n");
            packageInfo.Append("    " + StringColonPare_n("version", PackageVersion) + "\n");


            packageInfo.Append("}");

            V.VIO.WriteToFile(packageInfo.ToString(), directoryPath + "/" + "package.json");

            Directory.CreateDirectory(directoryPath + "/" + "Documentation");

            DirectoryInfo runtimeDirectory = Directory.CreateDirectory(directoryPath + "/" + "Runtime");
            if (runtimeDirectory.Exists)
            {
                CreateRuntimeAssemblyDefnition(directoryPath + "/" + "Runtime");
            }
            DirectoryInfo editorDirectory = Directory.CreateDirectory(directoryPath + "/" + "Editor");
            if (editorDirectory.Exists)
            {
                CreateEditorAssemblyDefenition(directoryPath + "/" + "Editor");
            }
            DirectoryInfo testsDirectory = Directory.CreateDirectory(directoryPath + "/" + "Tests");
            if (testsDirectory.Exists)
            {
                CreateTestFolderContent(directoryPath + "/" + "Tests");
            }

            string initLog =NameSpace + " " + "Project: " +  System.DateTime.Now.ToString();

            StringBuilder sb = new StringBuilder();
            sb.Append(NameSpace);

            V.VIO.WriteToFile(sb.ToString(), directoryPath + "/" + "CHANGELOG.txt");
            V.VIO.WriteToFile(sb.ToString(), directoryPath + "/" + "LICENSE.md");
            V.VIO.WriteToFile(sb.ToString(), directoryPath + "/" + "README.txt");

        }
    }

    void CreateRuntimeAssemblyDefnition(string path)
    {
        StringBuilder runtimeAsdef = new StringBuilder();
        runtimeAsdef.Append("{\n");

        runtimeAsdef.Append("   " + StringColonPare_n("name", NameSpace + "." + ProjectName + "." + "Runtime") + ",\n");
        string content = "   \"references\": [],\n" +
                         "   \"optionalUnityReferences\": [],\n" +
                         "   \"includePlatforms\": [],\n" +
                         "   \"excludePlatforms\": [],\n" +
                         "   \"allowUnsafeCode\": true,\n" +
                         "   \"overrideReferences\": false,\n" +
                         "   \"precompiledReferences\": [],\n" +
                         "   \"autoReferenced\": true,\n" +
                         "   \"defineConstraints\": [],\n" +
                         "   \"versionDefines\": []\n";
        runtimeAsdef.Append(content);
        runtimeAsdef.Append("}");
        V.VIO.WriteToFile(runtimeAsdef.ToString(), path + "/" + NameSpace + "." + ProjectName + "." + "Runtime." + "asmdef");
    }

    void CreateEditorAssemblyDefenition(string path)
    {
        StringBuilder runtimeAsdef = new StringBuilder();
        runtimeAsdef.Append("{\n");

        runtimeAsdef.Append("   " + StringColonPare_n("name", NameSpace + "." + ProjectName + "." + "Editor") + ",\n");

        runtimeAsdef.Append("   " + "\"references\":\n");
        runtimeAsdef.Append("   [\n");
        runtimeAsdef.Append("       " + "\"" + NameSpace + "." + ProjectName + ".Runtime" + "\"\n");
        runtimeAsdef.Append("   ],\n");

        string content = 
                         "   \"optionalUnityReferences\": [],\n" +
                         "   \"includePlatforms\": [\"Editor\"],\n" +
                         "   \"excludePlatforms\": [],\n" +
                         "   \"allowUnsafeCode\": true,\n" +
                         "   \"overrideReferences\": false,\n" +
                         "   \"precompiledReferences\": [],\n" +
                         "   \"autoReferenced\": true,\n" +
                         "   \"defineConstraints\": [],\n" +
                         "   \"versionDefines\": []\n";
        runtimeAsdef.Append(content);
        runtimeAsdef.Append("}");
        V.VIO.WriteToFile(runtimeAsdef.ToString(), path + "/" + NameSpace + "." + ProjectName + "." + "Editor." + "asmdef");
    }

    void CreateTestFolderContent(string path)
    {
        DirectoryInfo editorDirectory = Directory.CreateDirectory(path + "/Editor");
        if (editorDirectory.Exists)
        {
            CreateTestEditorAssemblyDefenition(path + "/Editor");
        }
        DirectoryInfo runtimeDirectory = Directory.CreateDirectory(path + "/Runtime");
        if (runtimeDirectory.Exists)
        {
            CreateTestRuntimeAssemblyDefenition(path + "/Runtime");
        }
    }

    void CreateTestEditorAssemblyDefenition(string path)
    {
        StringBuilder test = new StringBuilder();
        test.Append("{\n");

        test.Append("   " + StringColonPare_n("name", NameSpace + "." + ProjectName + ".Editor.Tests") + ",\n");
        test.Append("   " + "\"references\":\n");
        test.Append("   [\n");
        test.Append("       " + "\"" + NameSpace + "." + ProjectName + ".Runtime" + "\",\n");
        test.Append("       " + "\"" + NameSpace + "." + ProjectName + ".Editor" + "\"\n");
        test.Append("   ],\n");

        string content = "   \"optionalUnityReferences\": [],\n" +
                         "   \"includePlatforms\": [\"Editor\"],\n" +
                         "   \"excludePlatforms\": [],\n" +
                         "   \"allowUnsafeCode\": true,\n" +
                         "   \"overrideReferences\": true,\n" +
                         "   \"precompiledReferences\": [],\n" +
                         "   \"autoReferenced\": false,\n" +
                         "   \"defineConstraints\": [],\n" +
                         "   \"versionDefines\": []\n";
        test.Append(content);
        test.Append("}");
        V.VIO.WriteToFile(test.ToString(), path + "/" + NameSpace + "." + ProjectName + ".Editor.Tests.asmdef");
    }

    void CreateTestRuntimeAssemblyDefenition(string path)
    {
        StringBuilder test = new StringBuilder();
        test.Append("{\n");

        test.Append("   " + StringColonPare_n("name", NameSpace + "." + ProjectName + ".Runtime.Tests") +",\n");
        test.Append("   " + "\"references\":\n");
        test.Append("   [\n");
        test.Append("       " + "\"" + NameSpace + "." + ProjectName + ".Runtime" + "\"\n");
        test.Append("   ],\n");

        string content = "   \"optionalUnityReferences\": [],\n" +
                         "   \"includePlatforms\": [],\n" +
                         "   \"excludePlatforms\": [],\n" +
                         "   \"allowUnsafeCode\": true,\n" +
                         "   \"overrideReferences\": true,\n" +
                         "   \"precompiledReferences\": [],\n" +
                         "   \"autoReferenced\": false,\n" +
                         "   \"defineConstraints\": [],\n" +
                         "   \"versionDefines\": []\n";
        test.Append(content);
        test.Append("}");
        V.VIO.WriteToFile(test.ToString(), path + "/" + NameSpace + "." + ProjectName + ".Runtime.Tests.asmdef");
    }


    string StringColonPare_n(string A,string B)
    {
        string result = "\"" + A + "\"" + " : " + "\"" + B + "\"";
        return result;
    }
}
