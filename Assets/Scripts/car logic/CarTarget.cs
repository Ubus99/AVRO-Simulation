using System.Collections.Generic;
using Dreamteck.Splines;
using Streets;
using UnityEngine;

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

        public SplineFollower SplineFollower { get; private set; }
        public float Distance { get; private set; }

        void Awake()
        {
            SplineFollower = GetComponent<SplineFollower>();
            SplineFollower.loopSamples = true;
        }

        void Update()
        {
            if (!follower) return;
            var posDif = transform.position - follower.position;
            Distance = posDif.magnitude;

            var a = Vector3.Angle(follower.forward, posDif);
            if (Mathf.Abs(a) > 90)
            {
                SplineFollower.followSpeed = pull;
            }
            else
            {
                if (Distance < maxDistance)
                    SplineFollower.followSpeed = pull * (1 - Mathf.Clamp01(Distance / maxDistance));
                else
                    SplineFollower.followSpeed = 0;
            }

            if (_junctionError)
            {
                //handle target reaching end without triggering node
                Debug.LogWarning("End reached without calling node. this should not happen");
                var p = SplineFollower.spline.pointCount;
                var n = SplineFollower.spline.GetNode(p - 1);
                OnNodePassed(n, p);
            }
        }

        void OnEnable()
        {
            SplineFollower.onNode += OnNodePassed;
            SplineFollower.onEndReached += OnEndReached;
        }

        void OnDisable()
        {
            SplineFollower.onNode -= OnNodePassed;
            SplineFollower.onEndReached -= OnEndReached;
        }

        public double GetFollowerPercent()
        {
            var result = new SplineSample();
            SplineFollower.Project(follower.position, ref result);
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
            if (_recursionLock || SplineFollower.spline == to.spline)
                return;

            _recursionLock = true;
            //Set the spline to the tracer
            SplineFollower.spline = to.spline;
            SplineFollower.RebuildImmediate();
            var startPercent = SplineFollower.ClipPercent(to.spline.GetPointPercent(to.pointIndex));
            SplineFollower.SetPercent(startPercent);
            _junctionError = false;
            _recursionLock = false;
        }
    }
}
