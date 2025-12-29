using UnityEngine;
using UnityEngine.Splines;

namespace Utils
{
    public static class SplineHelper
    {
        public static void GetClosestPoint(SplineContainer splineContainer, Vector3 worldPoint,
            out Vector3 closestPoint, out float distance, out float t)
        {
            var localPoint = splineContainer.transform.InverseTransformPoint(worldPoint);
            distance = SplineUtility.GetNearestPoint(
            splineContainer[0],
            localPoint,
            out var nearestPoint,
            out t,
            SplineUtility.PickResolutionDefault,
            3);

            closestPoint = splineContainer.transform.TransformPoint(nearestPoint);
        }
    }
}
