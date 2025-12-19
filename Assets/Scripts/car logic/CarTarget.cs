using Streets;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace car_logic
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
                    splineFollower.Play();

                }

            var dirFollower = transform.position - follower.position;
            distance = dirFollower.magnitude;

            Debug.DrawRay(transform.position, dirFollower.normalized, Color.cyan);
            Debug.DrawRay(transform.position, transform.forward, Color.orange);

            var a = Vector3.Angle(follower.forward, dirFollower);
            if (Mathf.Abs(a) < 90) // behind car
            {
                ResetPosition();
            }
            else // in front of car
            {
                if (distance < maxDistance)
                {
                    splineFollower.MaxSpeed = pull * (1 - Mathf.Clamp01(distance / maxDistance));
                }
                else if (distance > 100)
                {
                    splineFollower.MaxSpeed = 0;
                }
                else
                {
                    ResetPosition();
                }
            }
        }

        void ResetPosition()
        {
            SplineUtility.GetNearestPoint(
            splineFollower.Container[0],
            follower.position + follower.forward * 2,
            out var point,
            out var t);

            splineFollower.NormalizedTime = t;
        }
    }
}
