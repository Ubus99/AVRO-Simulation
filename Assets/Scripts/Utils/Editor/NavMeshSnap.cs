using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;

namespace Utils.Editor
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
        SplineContainer _splineContainer;
        Transform _target;

        void Start()
        {
            if (TryGetComponent(out _splineContainer)) _mode = Mode.Spline;
            else if (!TryGetComponent(out _target)) throw new MissingComponentException("Missing component");
        }

#if UNITY_EDITOR
        void Update()
        {
            if (!Application.isEditor || Application.isPlaying) return;

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
#endif

        void OnValidate()
        {
            if (TryGetComponent(out _splineContainer)) _mode = Mode.Spline;
            else if (!TryGetComponent(out _target)) throw new MissingComponentException("Missing component");
        }

        void UpdateSpline()
        {
            if (!_splineContainer) return;
            foreach (var spline in _splineContainer.Splines)
            {
                for (var i = 0; i < spline.Count; i++)
                {
                    var knot = spline[i];

                    var worldPos = _splineContainer.transform.TransformPoint(knot.Position);
                    if (!NavMesh.SamplePosition(
                        worldPos,
                        out var hit,
                        100.0f,
                        NavMesh.AllAreas))
                        continue;

                    var localPos = hit.position + offset;
                    knot.Position = _splineContainer.transform.InverseTransformPoint(localPos);
                    spline.SetKnot(i, knot);
                }
            }
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
                transform.rotation.SetLookRotation(transform.forward, hit.normal);
            }

        }
    }
}
