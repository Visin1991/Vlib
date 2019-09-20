using UnityEngine;
using System.Collections;

namespace V
{
    public class CursorOnObj : MonoBehaviour
    {

        public enum MouseActionType { Eney, PickUp }

        public MouseActionType type;

        void OnMouseEnter()
        {
            if (type == MouseActionType.PickUp)
                CursorManager.GetInstance().setHand();
            if (type == MouseActionType.Eney)
                CursorManager.GetInstance().setAttack();
        }

        void OnMouseExit()
        {
            CursorManager.GetInstance().setMouse();
        }

        private void OnDestroy()
        {
            CursorManager.GetInstance().setMouse();
        }

        /*
        void OnMouseDown() {

        }

        void OnMouseUp() {

        }  */
    }
}