using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Splines;
using Utils.Types;

namespace Scenes.Scripts.Missions
{
    [ExecuteInEditMode]
    public class AlternativeRouteHelper : EditorBehavior
    {
        Mission _mission;
        public List<AlternativeRoute> routes { get; } = new();

        void LateUpdate()
        {
            if (!_mission) return;

            routes.Clear();
            routes.AddRange(GetComponentsInChildren<AlternativeRoute>());
            foreach (var route in routes) route.parent = this;

            _mission.alternativeRoutes = this;

            foreach (var ar in routes)
            {
                SnapRoute(ar);
            }
        }

        protected override void HandleIsDirty()
        {
            RefreshComponents();
        }

        public void SelectRoute(AlternativeRoute route)
        {
            foreach (var ar in routes)
            {
                ar.SetSelected(ar == route);
            }
        }

        protected override void RefreshComponents()
        {
            _mission = GetComponentInParent<Mission>();
        }

        void SnapRoute(AlternativeRoute alternativeRoute)
        {
            var sc = alternativeRoute.route;
            if (!sc) return;

            var spline = sc[0];
            if (spline.Count < 2) return;

            var p1 = sc.transform.InverseTransformPoint(_mission.startPoint.position);
            spline[0] = new BezierKnot(
            p1,
            spline[0].TangentIn,
            spline[0].TangentOut,
            _mission.startPoint.rotation
            );

            var rotation = Quaternion.Lerp(_mission.startPoint.rotation, _mission.endPoint.rotation, 0.5f);
            for (var i = 1; i < spline.Count - 1; i++)
                spline[i] = new BezierKnot(
                spline[i].Position,
                spline[i].TangentIn,
                spline[i].TangentOut,
                rotation
                );

            var p2 = sc.transform.InverseTransformPoint(_mission.endPoint.position);
            spline[^1] = new BezierKnot(
            p2,
            spline[^1].TangentIn,
            spline[^1].TangentOut,
            _mission.endPoint.rotation
            );

        }

        public void SetActiveRoute(AlternativeRoute route)
        {
            foreach (var ar in routes)
            {
                ar.SetSelected(ar == route);
            }
        }
    }
}
