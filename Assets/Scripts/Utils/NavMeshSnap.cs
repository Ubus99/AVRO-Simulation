using System;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.AI;

namespace Scripts
{
    [ExecuteInEditMode]
    public class NavMeshSnap : MonoBehaviour
    {
        public enum Mode
        {
            Spline,
            Point
        }

        public Vector3 offset;

        public Mode _mode = Mode.Point;
        SplineComputer _spline;
        Transform _target;

        void Start()
        {
            if (TryGetComponent(out _spline))
            {
                _mode = Mode.Spline;
            }
            else if (!TryGetComponent(out _target))
            {
                throw new MissingComponentException("Missing component");
            }
        }

        void Update()
        {
            switch (_mode)
            {
                case Mode.Spline:
                    UpdateSpline();
                    break;
                case Mode.Point:
                    UpdatePoint();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void UpdateSpline()
        {
            var points = _spline.GetPoints();
            for (var i = 0; i < points.Length; i++)
            {
                if (NavMesh.SamplePosition(
                    points[i].position,
                    out var hit,
                    100.0f,
                    NavMesh.AllAreas))
                {
                    points[i].SetPosition(hit.position + offset);
                }
            }
            _spline.SetPoints(points);
        }

        void UpdatePoint()
        {
            if (NavMesh.SamplePosition(
                transform.position,
                out var hit,
                100.0f,
                NavMesh.AllAreas))
            {
                transform.position = hit.position + offset;
            }
        }
    }
}
