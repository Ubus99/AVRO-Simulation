using car_logic;
using Dreamteck.Splines;
using UnityEngine;

namespace Streets
{
    [RequireComponent(typeof(CarAI))]
    public class CarFollower : MonoBehaviour
    {
        public bool visualize = true;
        public CarTarget target;
        public float baseSpeed;
        CarAI _agent;

        PathVisualizer _pathVisualizer;
        SplineFollower _splineFollower;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _agent = GetComponent<CarAI>();
            _agent.CustomDestination = target.transform;

            _pathVisualizer = GetComponentInChildren<PathVisualizer>();

            baseSpeed = _agent.MaxRPM;
            target.follower = transform;
        }

        void Update()
        {
            if (visualize && _pathVisualizer)
            {
                var path = _agent.FutureWaypoints;
                _pathVisualizer.SetPath(path);
            }
        }

        void OnDrawGizmos()
        {
            if (!target) return;
            Gizmos.DrawSphere(target.transform.position, 0.5f);
        }

        public float GetTargetSpeed()
        {
            return _agent.MaxRPM;
        }

        public float GetBaseSpeed()
        {
            return baseSpeed;
        }

        public void SetTargetSpeed(float speed)
        {
            _agent.MaxRPM = Mathf.RoundToInt(speed);
        }
    }
}
