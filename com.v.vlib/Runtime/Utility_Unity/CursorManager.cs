using UnityEngine;
using System.Collections;

namespace V
{
    public class CursorManager : MonoBehaviour
    {

        static private CursorManager instance = null;
        public static CursorManager GetInstance()
        {
            return instance;
        }

        //Make a singleTon
        void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            instance = this;
        }

        public Texture2D mouse;
        public Texture2D hand;
        public Texture2D attack;

        public CursorMode cursorMode = CursorMode.Auto;
        public Vector2 hotSpot = Vector2.zero;

        // Use this for initialization
        void Start()
        {
            setMouse();
        }

        public void setMouse()
        {
            Cursor.SetCursor(mouse, hotSpot, cursorMode);
        }

        public void setHand()
        {
            Debug.Log("Set Hand CursorManager");
            Cursor.SetCursor(hand, hotSpot, cursorMode);
        }

        public void setAttack()
        {
            Cursor.SetCursor(attack, hotSpot, cursorMode);
        }
    }
}
