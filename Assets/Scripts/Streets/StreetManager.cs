using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace Streets
{
    public class StreetManager : MonoBehaviour
    {
        readonly List<SplineContainer> _splineContainers = new();
        bool _dirty;

        void Awake()
        {
            ServiceLocator.Instance.TryRegister<StreetManager>(this);
            _dirty = true;
        }

        void Start()
        {
            _splineContainers.Clear();
            _splineContainers.AddRange(GetComponentsInChildren<SplineContainer>());
        }

        public (SplineContainer container, float progress) GetClosestSpline(Vector3 point)
        {
            var minDistance = float.MaxValue;
            SplineContainer minSpline = null;
            var mint = float.MaxValue;

            foreach (var splineContainer in _splineContainers)
            {
                var dist = SplineUtility.GetNearestPoint(splineContainer[0], point, out var nearest, out var t);
                if (!(dist < minDistance))
                    continue;

                minDistance = dist;
                mint = t;
                minSpline = splineContainer;
            }

            return (minSpline, mint);
        }
    }
}
