using Streets;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace car_navigation
{
    [RequireComponent(typeof(SplineAnimate))]
    public class CarTarget : MonoBehaviour
    {
        public Transform follower;
        public float pull = 100;
        public float maxDistance = 50;
        bool _junctionError;
        bool _recursionLock;

        public SplineAnimate splineFollower { get; private set; }
        public float distance { get; private set; }

        void Awake()
        {
            splineFollower = GetComponent<SplineAnimate>();
            splineFollower.Loop = SplineAnimate.LoopMode.Loop;
        }

        void Update()
        {
            if (!follower) return;
            if (!splineFollower.Container)
                if (ServiceLocator.Instance.TryGet<StreetManager>(out var streetManager))
                {
                    var (container, progress) = streetManager.GetClosestSpline(transform.position);
                    splineFollower.Container = container;
                    splineFollower.NormalizedTime = progress;

                }

            var posDif = transform.position - follower.position;
            distance = posDif.magnitude;

            var a = Vector3.Angle(follower.forward, posDif);
            if (Mathf.Abs(a) > 90)
            {
                splineFollower.MaxSpeed = pull;
            }
            else
            {
                if (distance < maxDistance)
                    splineFollower.MaxSpeed = pull * (1 - Mathf.Clamp01(distance / maxDistance));
                else
                    splineFollower.MaxSpeed = 0;
            }
        }
    }
}
