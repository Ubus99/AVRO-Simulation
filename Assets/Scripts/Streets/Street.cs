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
                var p1 = GetPointAtIndex(lane1, e.x);
                var p2 = GetPointAtIndex(lane2, e.y);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(p1, p2);
            }

            foreach (var e in exitLanes)
            {
                e.DrawGizmos(lane1, lane2);
            }
        }

        public static Vector3 GetPointAtIndex(SplineContainer container, int index)
        {
            if (index < 0 || index >= container.Splines[0].Count)
                return Vector3.zero;

            var knot = container.Splines[0][index];
            return container.transform.TransformPoint(knot.Position);
        }

        public Vector3 GetPointAtIndex(Lane lane, int idx)
        {
            return GetPointAtIndex(GetLane(lane), idx);
        }

        public SplineContainer GetLane(Lane lane)
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
            [SerializeField]
            public Lane lane;

            [SerializeField]
            public int index;

            public List<Address> targets = new();

            public void DrawGizmos(SplineContainer laneA, SplineContainer laneB)
            {
                Gizmos.color = Color.green;
                var p1 = GetPointAtIndex(lane == Lane.LaneA ? laneA : laneB, index);
                foreach (var address in targets)
                {
                    if (!address.street) continue;

                    var p2 = GetPointAtIndex(address.street.GetLane(address.lane), address.idx);
                    Gizmos.DrawLine(p1, p2);
                }
            }

            [Serializable]
            public struct Address
            {
                public Street street;
                public Lane lane;
                public int idx;
            }
        }
    }
}
