using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{

    public static class TouchLib
    {
        static float startTime;
        static float endTime;

        static Vector2 startPos;
        static Vector2 endPos;
        static Vector2 move;

        static float minSwipDist = 20.0f;
        //static float maxSwipDist = 100.0f;
        static float swipeDistance;

        //static float maxTime = 1.0f;

         public static Vector2 GetSwipe2D()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    startTime = Time.time;
                    startPos = touch.position;
                    return Vector2.zero;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    //G_Messager.SendMessage("Phase End");
                    endTime = Time.time;
                    endPos = touch.position;
                    move = endPos - startPos;
                    swipeDistance = move.magnitude;

                  
                    float swipeTime = endTime - startTime;
                    if (swipeDistance > minSwipDist)
                    {
                        Vector2 wasd = new Vector2(Mathf.Clamp(move.x / swipeDistance, -1.0f, 1.0f),
                                            Mathf.Clamp(move.y / swipeDistance, -1.0f, 1.0f));
                        //G_Messager.SendMessage("SwapDistance : " + wasd.ToString());
                        return wasd;
                    }
                    return Vector2.zero;
                }
                return Vector2.zero;
            }
            return Vector2.zero;
        }

        static float perspectiveZoomSpeed = 0.5f;
        static float orthZoomSpeed = 0.5f;

        public static void ZoomInOut_ChangeFieldOfView()
        {
            if (Input.touchCount == 2)
            {
                float deltaMagnitudeDiff = GetDeltaMagnitudeDifferent();
                if (Camera.main.orthographic)
                {
                    Camera.main.orthographicSize += deltaMagnitudeDiff * orthZoomSpeed;
                    Camera.main.orthographicSize = Mathf.Max(Camera.main.orthographicSize, 0.1f);
                }
                else
                {
                    Camera.main.fieldOfView += deltaMagnitudeDiff * perspectiveZoomSpeed;
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
                }
            }
        }

        public static float GetDeltaMagnitudeDifferent()
        {
            Touch t1 = Input.GetTouch(0);
            Touch t2 = Input.GetTouch(1);
            Vector2 t1PrevPos = t1.position - t1.deltaPosition;
            Vector2 t2PrevPos = t2.position - t2.deltaPosition;

            float prevTouchMag = (t1PrevPos - t2PrevPos).magnitude;
            float currentMag = (t1.position - t2.position).magnitude;

            return prevTouchMag - currentMag;
        }
    }

    public static class GravitySensor
    {
        public static Vector3 GetAcceleration()
        {
            Vector3 dir = Vector3.zero;
            dir.x = -Input.acceleration.y;
            dir.z = Input.acceleration.x;
            if (dir.sqrMagnitude > 1)
                dir.Normalize();
            return dir;
        }
    }

}
