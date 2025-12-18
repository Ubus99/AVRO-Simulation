using UnityEngine;

namespace Utils
{
    public static class ObjectManager
    {
        public static void KillAllChildren(Transform parent)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(parent.GetChild(i).gameObject);
            }
        }
    }
}
