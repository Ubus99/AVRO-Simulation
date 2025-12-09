using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using Utils;

namespace Streets
{
    public class StreetManager : MonoBehaviour
    {
        readonly List<SplineComputer> _splines = new();

        bool _dirty;

        void Awake()
        {
            _splines.Clear();
            ServiceLocator.Instance.TryRegister<StreetManager>(this);
            _dirty = true;
        }

        void Update()
        {
            if (!_dirty) return;
            
            _splines.Clear();
            var streets = GameObject.FindGameObjectsWithTag("Street");
            foreach (var go in streets)
            {
                _splines.AddRange(go.GetComponentsInChildren<SplineComputer>());
            }
            
            _dirty = false;
        }

        public (SplineComputer, SplineSample) ClosestSpline(Vector3 point)
        {
            SplineComputer minSpline = null;
            var minSample = new SplineSample();
            var minDistance = float.MaxValue;

            foreach (var sc in _splines)
            {
                var ss = sc.Project(point);
                var dist = Vector3.Distance(point, ss.position);
                if (!(dist < minDistance))
                    continue;

                minSpline = sc;
                minDistance = dist;
                minSample = ss;
            }
            return (minSpline, minSample);
        }
    }
}
