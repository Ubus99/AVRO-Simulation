using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Streets
{
    [ExecuteInEditMode]
    public class Street : MonoBehaviour
    {
        public enum Lane
        {
            LaneA,
            LaneB
        }

        [SerializeField]
        SplineContainer lane1;

        [SerializeField]
        SplineContainer lane2;

        [Header("Nodes")]
        public List<Vector2Int> exits = new();

        [Header("Exits")]
        public List<Exit> exitLanes = new();


        public Spline spline1
        {
            get { return lane1[0]; }
        }

        public Spline spline2
        {
            get { return lane2[0]; }
        }

        void OnDrawGizmos()
        {
            foreach (var e in exits)
            {
                var p1 = GetWorldPointAtIndex(lane1, e.x);
                var p2 = GetWorldPointAtIndex(lane2, e.y);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(p1, p2);
            }

            foreach (var e in exitLanes)
            {
                e.DrawGizmos(lane1, lane2);
            }
        }

        void OnValidate()
        {
            foreach (var exit in exitLanes)
            {
                exit.myAddress.street = this;
            }
        }

        static Vector3 GetWorldPointAtIndex(SplineContainer container, int index)
        {
            if (index < 0 || index >= container.Splines[0].Count)
                return Vector3.zero;

            var knot = container.Splines[0][index];
            return container.transform.TransformPoint(knot.Position);
        }

        Vector3 GetWorldPointAtIndex(Lane lane, int idx)
        {
            return GetWorldPointAtIndex(GetLane(lane), idx);
        }

        BezierKnot GetKnotAtIndex(Lane lane, int idx)
        {
            return GetLane(lane)[0][idx];
        }

        SplineContainer GetLane(Lane lane)
        {
            return lane switch
            {
                Lane.LaneA => lane1,
                Lane.LaneB => lane2,
                _ => throw new ArgumentOutOfRangeException(nameof(lane), lane, null)
            };
        }

        [Serializable]
        public class Exit
        {
            public Address myAddress;

            public List<Address> targets = new();

            public void DrawGizmos(SplineContainer laneA, SplineContainer laneB)
            {
                Gizmos.color = Color.green;
                var p1 = GetWorldPointAtIndex(myAddress.lane == Lane.LaneA ? laneA : laneB, myAddress.idx);
                foreach (var address in targets)
                {
                    if (!address.street) continue;

                    var p2 = GetWorldPointAtIndex(address.street.GetLane(address.lane), address.idx);
                    Gizmos.DrawLine(p1, p2);
                }
            }
        }

        [Serializable]
        public struct Address : IEquatable<Address>
        {
            public Street street;
            public Lane lane;
            public int idx;

            public bool Equals(Address other)
            {
                return Equals(street, other.street) && lane == other.lane && idx == other.idx;
            }

            public override bool Equals(object obj)
            {
                return obj is Address other && Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(street, (int)lane, idx);
            }

            public BezierKnot GetKnot()
            {
                return street.GetKnotAtIndex(lane, idx);
            }

            public Vector3 GetWorldPoint()
            {
                return street.GetWorldPointAtIndex(lane, idx);
            }

            public SplineContainer GetSpline()
            {
                return street.GetLane(lane);
            }
        }
    }
}
