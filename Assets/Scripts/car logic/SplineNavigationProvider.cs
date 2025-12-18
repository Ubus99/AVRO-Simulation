using car_logic;
using UnityEngine;

namespace car_navigation
{
    [RequireComponent(typeof(CarAI))]
    public class SplineNavigationProvider : NavigationProvider
    {
        CarAI _agent;

        PathVisualizer _pathVisualizer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            if (!targetPrefab)
            {
                Debug.LogError("No target prefab assigned!");
            }
            target = Instantiate(targetPrefab, transform.position + transform.forward, Quaternion.identity);
            target.name = $"{gameObject.name}_Target";

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

        public override float GetTargetSpeed()
        {
            return _agent.MaxRPM;
        }

        public float GetBaseSpeed()
        {
            return baseSpeed;
        }

        public override void SetTargetSpeed(float speed)
        {
            _agent.MaxRPM = Mathf.RoundToInt(speed);
        }

        public override void SetTargetLocation(Vector3 position)
        {
        }
    }
}
