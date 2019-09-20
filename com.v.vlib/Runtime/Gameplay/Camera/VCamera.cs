using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


namespace V
{
    public class VCamera : MonoBehaviour
    {
        public enum CamerMode
        {
            FirstPerson,
            ThridPerson
        }

        public CamerMode camerMode = CamerMode.FirstPerson;
        public bool controlFirstCameraMovement = false;
        public bool controlThridPersonCameraTarget_Movement = false;


        [Range(0.1f, 50.0f)]
        public float moveSpeed = 20.0f;


        [SerializeField] private Vector2 pitchMinMax = new Vector2(-85, 85);
        [SerializeField] private float rotationSmoothTime = 0.1f;
        private Vector3 rotationSmoothVelocity;
        private Vector3 currentRotation;

        public Transform target;
        [SerializeField] private Vector2 rangeToTarget = new Vector2(2, 10);
        [SerializeField] private float cameraMoveSensitivity = 270;

        public float dstToTarget = 10;

        private float yaw;  //Rotation around Y Axis
        public float pitch = 55;//Rotation around X Axis
        public bool isFixedYaw = true;


        private void Update()
        {
            VInput.GetCameraDelta(this);
        }

        private void LateUpdate()
        {
            if (camerMode == CamerMode.FirstPerson)
            {
                LateUpdate_FirstCameraMode();
                if (controlFirstCameraMovement)
                {
                    MoveFirstPersonCamera();
                }

            }
            else
            {
                LateUpdate_ThirdCameraMode();
                if (controlThridPersonCameraTarget_Movement)
                {
                    MoveThirdPersonCameraTarget();
                }
            }
        }

        void MoveFirstPersonCamera()
        {
            if (VInput.GetKey_W())
            {
                transform.position += transform.forward * Time.deltaTime * moveSpeed;
            }
            if (VInput.GetKey_S())
            {
                transform.position -= transform.forward * Time.deltaTime * moveSpeed;
            }
            if (VInput.GetKey_A())
            {
                transform.position -= transform.right * Time.deltaTime * moveSpeed;
            }
            if (VInput.GetKey_D())
            {
                transform.position += transform.right * Time.deltaTime * moveSpeed;
            }
        }

        void MoveThirdPersonCameraTarget()
        {
            if (target == null) {
                Debug.LogError("Third Person Camera Target is NULL");
                return;
            }

            if (VInput.GetKey_W())
            {
                Vector3 cameraPos = target.position;
                cameraPos.y = transform.position.y;
                Vector3 c2ObjDir = (transform.position - cameraPos).normalized;
                transform.position += c2ObjDir * Time.deltaTime * moveSpeed;
            }

            if (VInput.GetKey_S())
            {
                Vector3 cameraPos = target.position;
                cameraPos.y = transform.position.y;
                Vector3 c2ObjDir = (transform.position - cameraPos).normalized;
                transform.position -= c2ObjDir * Time.deltaTime * moveSpeed;
            }

            if (VInput.GetKey_A())
            {
                Vector3 cameraPos = target.position;
                cameraPos.y = transform.position.y;
                Vector3 c2ObjDir = (transform.position - cameraPos).normalized;
                c2ObjDir = c2ObjDir.RotateAroundYAxis(-90);
                transform.position += c2ObjDir * Time.deltaTime * moveSpeed;
            }

            if (VInput.GetKey_D())
            {
                Vector3 cameraPos = target.position;
                cameraPos.y = transform.position.y;
                Vector3 c2ObjDir = (transform.position - cameraPos).normalized;
                c2ObjDir = c2ObjDir.RotateAroundYAxis(-90);
                transform.position -= c2ObjDir * Time.deltaTime * moveSpeed;
            }
        }



        float maxOffset = 0.2f;

        Vector3 offsetPos;

        Vector3 Anti_Vibration_Pos()
        {
            if ((offsetPos - target.position).sqrMagnitude > maxOffset)
            {
                offsetPos = target.position;
            }
            return offsetPos - transform.forward * dstToTarget;
        }


        float detal_yaw;
        float detal_pitch;
        float detal_dstToTarget;

        public float Yaw
        {
            get { return yaw; }
        }

        public float Pitch
        {
            get { return pitch; }
        }

        public void SetCameraDetal(float dy, float dp, float dd)
        {
            detal_yaw = dy; detal_pitch = dp; detal_dstToTarget = dd;
        }

        void LateUpdate_ThirdCameraMode()
        {
            //Add detal Camera Value each frame
            yaw += detal_yaw * cameraMoveSensitivity;
            pitch += detal_pitch * cameraMoveSensitivity;
            dstToTarget += detal_dstToTarget * cameraMoveSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 0), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;

            transform.position = target.position - transform.forward * dstToTarget;

            //Reset detal value to zero each frame
            SetCameraDetal(0, 0, 0);
        }

        void LateUpdate_FirstCameraMode()
        {
            //Add detal Camera Value each frame
            yaw += detal_yaw * cameraMoveSensitivity;
            pitch += detal_pitch * cameraMoveSensitivity;

            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);



            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw, 0), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;
            //Reset detal value to zero each frame
            SetCameraDetal(0, 0, 0);
        }

    }
}
