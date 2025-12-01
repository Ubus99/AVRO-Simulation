using System.Collections.Generic;
using Dreamteck.Splines;
using Streets;
using UnityEngine;
using Utils;

namespace car_logic
{
    [RequireComponent(typeof(SplineFollower))]
    public class CarTarget : MonoBehaviour
    {
        public Transform follower;
        public float pull = 100;
        public float maxDistance = 50;
        bool _junctionError;
        bool _recursionLock;

        public SplineFollower splineFollower { get; private set; }
        public float distance { get; private set; }

        void Awake()
        {
            splineFollower = GetComponent<SplineFollower>();
            splineFollower.loopSamples = true;
        }

        void Update()
        {
            if (!follower) return;
            if (!splineFollower.spline)
                if (ServiceLocator.Instance.TryGet<StreetManager>(out var streetManager))
                {
                    var (sc, ss) = streetManager.ClosestSpline(transform.position);
                    splineFollower.spline = sc;
                    splineFollower.SetPercent(ss.percent);
                    splineFollower.RebuildImmediate();
                }

            var posDif = transform.position - follower.position;
            distance = posDif.magnitude;

            var a = Vector3.Angle(follower.forward, posDif);
            if (Mathf.Abs(a) > 90)
            {
                splineFollower.followSpeed = pull;
            }
            else
            {
                if (distance < maxDistance)
                    splineFollower.followSpeed = pull * (1 - Mathf.Clamp01(distance / maxDistance));
                else
                    splineFollower.followSpeed = 0;
            }

            if (_junctionError)
            {
                //handle target reaching end without triggering node
                Debug.LogWarning("End reached without calling node. this should not happen");
                var p = splineFollower.spline.pointCount;
                var n = splineFollower.spline.GetNode(p - 1);
                OnNodePassed(n, p);
            }
        }

        void OnEnable()
        {
            splineFollower.onNode += OnNodePassed;
            splineFollower.onEndReached += OnEndReached;
        }

        void OnDisable()
        {
            splineFollower.onNode -= OnNodePassed;
            splineFollower.onEndReached -= OnEndReached;
        }

        public double GetFollowerPercent()
        {
            var result = new SplineSample();
            splineFollower.Project(follower.position, ref result);
            return result.percent;
        }

        void OnEndReached(double d)
        {
            _junctionError = true;
        }

        void OnNodePassed(Node passed, int point)
        {
            Debug.Log($"Reached node {passed.name} connected at point {point}");
            if (passed.TryGetComponent<Junction>(out var junction))
            {
                SwitchSpline(junction.GetRandomExit(), point);
            }
        }

        void OnNodePassed(List<SplineTracer.NodeConnection> passed)
        {
            OnNodePassed(passed[0].node, passed[0].point);
        }

        void SwitchSpline(Node.Connection to, int idx)
        {
            if (_recursionLock || splineFollower.spline == to.spline)
                return;

            _recursionLock = true;
            //Set the spline to the tracer
            splineFollower.spline = to.spline;
            splineFollower.RebuildImmediate();
            var startPercent = splineFollower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            splineFollower.SetPercent(startPercent);
            _junctionError = false;
            _recursionLock = false;
        }
    }
}
