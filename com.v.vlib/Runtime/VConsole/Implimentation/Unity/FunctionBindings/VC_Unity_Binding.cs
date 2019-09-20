using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public static class VC_Unity_Binding
    {
        static List<GameObject> V_Objects = new List<GameObject>();
        public static GameObject currentBind;

        [VC_Function(Alias = "GO.Create",Description = "Create a Game Object", Usage ="GameObject.Create [int]")]
        public static void CreateObject()
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "V_Object" + V_Objects.Count.ToString();
            currentBind = gameObject;
        }


        [VC_Function(Alias = "GO.DestroyAll",Description = "Destroy all GameObject....")]
        public static void DestroyAll()
        {
            for (int i = V_Objects.Count-1; i >= 0 ; i--)
            {
                GameObject.Destroy(V_Objects[i]);
            }
        }

        [VC_Function(Alias = "Light.Color",Description = "Set color for the Main Light")]
        public static void Light_SetColor(Vector3 color)
        {
           Light[] lights = GameObject.FindObjectsOfType<Light>();
            for (int i = 0; i < lights.Length; i++)
            {
                lights[i].color = new Color(color.x,color.y,color.z);
            }
        }

        [VC_Function(Alias = "Wind.Main", Description = "Set Wind MainForce")]
        public static void Wind_Intensity(float mainIntensity)
        {
            WindZone windZone = GameObject.FindObjectOfType<WindZone>();
            if (windZone)
            {
                windZone.windMain = mainIntensity;
            }
        }

        [VC_Function(Alias = "Camera.AddVCamera")]
        public static void Camera_Add_VCamera()
        {
            Camera camera = GameObject.FindObjectOfType<Camera>();
            if (camera)
            {
                if(camera.gameObject.GetComponent<VCamera>() == null)
                camera.gameObject.AddComponent<VCamera>();
            }
        }

        [VC_Function(Alias ="VCamera.ControlMovement")]
        public static void VCamera_ControlMovement(bool isControl)
        {
            VCamera vCamera = GameObject.FindObjectOfType<VCamera>();
            if (vCamera)
            {
                vCamera.controlFirstCameraMovement = isControl;
            }
        }


        //----------------------------------------------
        [VC_Function(Alias = "Help")]
        public static void Help()
        {
            foreach (KeyValuePair<string, VC_FunctionInfo> kvp in VC_FunctionInfo.functionInfos)
            {
                VConsole.Log("Command: " + kvp.Key + "------------| " +  kvp.Value.Method.ToString() );
            }
        }




    }
}
