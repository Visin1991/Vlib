using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace V
{
    public static class StaticFlagSettings
    {

        static public void SetStaticEditorFlags(GameObject go, StaticEditorFlags flags)
        {
            StaticEditorFlags f = GameObjectUtility.GetStaticEditorFlags(go);
            f |= flags;
            GameObjectUtility.SetStaticEditorFlags(go, f);

            for (int c = 0; c < go.transform.childCount; ++c)
            {
                SetStaticEditorFlags(go.transform.GetChild(c).gameObject, flags);
            }
        }

        static public void UnsetStaticEditorFlags(GameObject go, StaticEditorFlags flags)
        {
            StaticEditorFlags f = GameObjectUtility.GetStaticEditorFlags(go);
            f &= ~flags;
            GameObjectUtility.SetStaticEditorFlags(go, f);

            for (int c = 0; c < go.transform.childCount; ++c)
            {
                UnsetStaticEditorFlags(go.transform.GetChild(c).gameObject, flags);
            }
        }
    }
}