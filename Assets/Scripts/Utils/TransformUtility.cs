using UnityEngine;

namespace Utils
{
    public static class TransformUtility
    {
        public static Vector2 TransformPoint(Vector2 point, Transform from, Transform to)
        {
            var worldPoint = from.TransformPoint(point);
            return to.InverseTransformPoint(worldPoint);
        }

        public static Vector3 TransformPoint(Vector3 point, Transform from, Transform to)
        {
            var worldPoint = from.TransformPoint(point);
            return to.InverseTransformPoint(worldPoint);
        }
    }
}
