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

        public (SplineContainer container, float progress) GetClosestSpline(Vector3 worldPoint)
        {
            var minDistance = float.MaxValue;
            SplineContainer minSpline = null;
            var mint = float.MaxValue;

            foreach (var splineContainer in _splineContainers)
            {
                SplineHelper.GetClosestPoint(
                splineContainer,
                worldPoint,
                out _,
                out var dist,
                out var t);
                
                if (!(dist < minDistance)) // skip if not minimum
                    continue;

                minDistance = dist;
                mint = t;
                minSpline = splineContainer;
            }

            return (minSpline, mint);
        }
    }
}
