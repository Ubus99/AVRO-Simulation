using System.Collections.Generic;
using Gameplay;
using UnityEngine;
using UnityEngine.Splines;

namespace Scenes.Scripts.Missions
{
    [ExecuteInEditMode]
    public class AlternativeRouteHelper : MonoBehaviour
    {
        public List<AlternativeRoute> alternativeRoutes = new();
        Mission _mission;

        void LateUpdate()
        {
            if (!_mission) return;

            alternativeRoutes.Clear();
            alternativeRoutes.AddRange(GetComponentsInChildren<AlternativeRoute>());
            _mission.alternativeRoutes.Clear();
            _mission.alternativeRoutes.AddRange(alternativeRoutes);

            foreach (var ar in alternativeRoutes)
            {
                var sc = ar.Route;
                if (!sc) continue;

                var spline = sc[0];
                if (spline.Count < 2) continue;

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
        }

        void OnValidate()
        {
            _mission = GetComponentInParent<Mission>();
        }
    }
}
