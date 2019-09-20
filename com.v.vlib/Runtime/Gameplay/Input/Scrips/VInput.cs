#if UNITY_STANDALONE || UNITY_EDITOR

#else
    #define MOBILE
#endif

//#define MOBILE


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class VInput : MonoBehaviour
    {

        public static Vector2 g_WASD = Vector2.zero;
        public static Vector2 g_MouseXY = Vector2.zero;
        public static float dd = 0;

        // Update is called once per frame
        private void Update()
        {
        #if MOBILE
                Mobile_Axis();
        #else
            Standard_MouseAxis();
        #endif
        }

        private void LateUpdate()
        {
            g_MouseXY.x = g_MouseXY.y = dd = 0;
        }

        public static void GetCameraDelta(VCamera camera)
        {
            camera.SetCameraDetal(g_MouseXY.x, g_MouseXY.y, dd);        
        }

        public static bool GetKey_W()
        {
        #if MOBILE
            return g_WASD.y >= 0.5f;
        #else
             return Input.GetKey(KeyCode.W);
        #endif
        }

        public static bool GetKey_S()
        {
        #if MOBILE
            return g_WASD.y <= -0.5f;
        #else
            return Input.GetKey(KeyCode.S);        
        #endif
        }

        public static bool GetKey_A()
        {
        #if MOBILE
            return g_WASD.x <= -0.5f; ;
        #else
            return Input.GetKey(KeyCode.A);
        #endif
        }

        public static bool GetKey_D()
        {
        #if MOBILE
            return g_WASD.x >= 0.5f; ;
        #else
            return Input.GetKey(KeyCode.D);
        #endif
        }

        static void Standard_MouseAxis()
        {
            ///Mouse Grab & .....
            if (Input.GetMouseButton(1))
            {
                g_MouseXY.x = Input.GetAxis("Mouse X") * Time.deltaTime;
                g_MouseXY.y = -Input.GetAxis("Mouse Y") * Time.deltaTime;
            }
            dd = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        }

        static void Mobile_Axis()
        {

            //Vector2 swipe = TouchLib.GetSwipe2D();
            //dy = swipe.x * Time.deltaTime;
            //dp = -swipe.y * Time.deltaTime;

            /// Get some error in the TouchLib.  touch Index Outof range......
            //dd = TouchLib.GetDeltaMagnitudeDifferent() * Time.deltaTime;
        }
    }
}
