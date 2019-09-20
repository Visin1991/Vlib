using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public static class CopyFile
    {
        public static bool CopyBinaryFile(string srcfilename, string destfilename)
        {
            if (System.IO.File.Exists(srcfilename) == false)
            {
                Debug.Log("Could not find The Source file " + srcfilename);
                return false;
            }
            if (System.IO.File.Exists(destfilename) == true)
            {
                string message = "Source File: " + destfilename + " 已经存在！！！ 是否选择覆盖";
                if (!EditorUtility.DisplayDialog("警告！！！", message, "是的", "取消"))
                {
                    return false;
                }
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

        public static bool CopyTextFile(string srcfilename, string destfilename)
        {

            if (System.IO.File.Exists(srcfilename) == false)
            {
                return false;

            }

            System.IO.StreamReader f1 = new System.IO.StreamReader(srcfilename);
            System.IO.StreamWriter f2 = new System.IO.StreamWriter(destfilename);

            while (true)
            {

                char[] buf = new char[1024];
                int sz = f1.Read(buf, 0, 1024);
                if (sz <= 0)
                    break;
                f2.Write(buf, 0, sz);
                if (sz < 1024)
                    break; // eof reached
            }
            f1.Close();
            f2.Close();
            return true;
        }
    }
}
