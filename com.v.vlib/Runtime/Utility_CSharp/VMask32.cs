using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public struct VMask32
    {
        System.Int32 maskValue;

        public void AddMask(int value)
        {
            maskValue |= value;
        }

        public void AddMaskPos(int pos)
        {
            maskValue |= (1 << pos);
        }

        public bool Contains(int value)
        {
            return (maskValue & value) == 0 ? false : true;
        }

        public bool ComtainsPos(int pos)
        {
            return (maskValue & (1 << pos)) == 0 ? false : true;
        }

        public void RemoveMask(int value)
        {
            maskValue &= ~value; // ~value equal to  i ^0xffffffff
        }

        public void RemoveMaksPos(int pos)
        {
            maskValue &= ~(1 << pos);
        }

        public bool Composition_Empty(int value)
        {
            int t = value ^ maskValue;
            t &= value;
            return t == value;
        }

        public int Array_Get_First_Empty_Pos(int[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                if ((maskValue & values[i]) == 0) return i;
            }
            return -1;
        }

        //public int Get_Frist_Empty_Pos(int value)
        //{
        //	for (int i = 0; i < 32; i++) {
        //		//if(value 
        //	}
        //}

        public static bool ContainsValueOnPos(int value, int pos)
        {
            return (value & (1 << pos)) == 0 ? false : true;

        }
    }
}