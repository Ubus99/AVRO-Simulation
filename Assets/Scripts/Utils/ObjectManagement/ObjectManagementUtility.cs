using UnityEngine;
using Object = UnityEngine.Object;

namespace Utils.ObjectManagement
{
    public static class ObjectManagementUtility
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
