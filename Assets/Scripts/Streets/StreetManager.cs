using System.Collections.Generic;
using System.Linq;
using Dreamteck.Splines;
using UnityEngine;
using Utils;

namespace Streets
{
    public class StreetManager : MonoBehaviour
    {
        public List<GameObject> streets = new();
        readonly List<SplineComputer> _splines = new();

        void Awake()
        {
            _splines.Clear();
            foreach (var sc in streets.Select(street =>
                         GetComponentInChildren<SplineComputer>()).Where(sc => sc))
                _splines.Add(sc);


            ServiceLocator.Instance.TryRegister<StreetManager>(this);
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
