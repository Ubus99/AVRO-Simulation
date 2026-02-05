using UnityEngine;
using UnityEngine.AI;

namespace car_logic
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshFollower : AbstractFollower
    {
        NavMeshAgent _agent;

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

            _agent = GetComponent<NavMeshAgent>();

            _pathVisualizer = GetComponentInChildren<PathVisualizer>();

            baseSpeed = _agent.speed;
            target.follower = transform;
        }

        void Update()
        {
            if (visualize && _pathVisualizer)
            {
                var path = _agent.path.corners.ToList();
                _pathVisualizer.SetPath(path);
            }
        }

        void FixedUpdate()
        {
            _agent.SetDestination(target.transform.position);
        }

        void OnDrawGizmos()
        {
            if (!target) return;
            Gizmos.DrawSphere(target.transform.position, 0.5f);
        }

        public override float GetTargetSpeed()
        {
            return _agent.speed;
        }

        public float GetBaseSpeed()
        {
            return baseSpeed;
        }

        public override void SetTargetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public override void SetTargetLocation(Vector3 position)
        {
        }

        public override void Halt()
        {
            _agent.isStopped = true;
        }
    }
}
