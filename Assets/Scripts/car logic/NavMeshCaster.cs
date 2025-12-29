using Streets;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines;
using Utils;

namespace car_logic
{
    [ExecuteAlways]
    public class NavMeshCaster : NavigationProvider
    {
        public float evaluationDistance = 10;

        NavMeshAgent _agent;

        PathVisualizer _pathVisualizer;

        SplineContainer _splineContainer;

        StreetManager _streetManager;

        Vector3 evaluationPoint
        {
            get { return gameObject.transform.position + gameObject.transform.forward * evaluationDistance; }
        }

        void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();

            _pathVisualizer = GetComponentInChildren<PathVisualizer>();
        }

        void Start()
        {
            ServiceLocator.Instance.TryGet(out _streetManager);
        }

        void FixedUpdate()
        {
            if (!_splineContainer && _streetManager)
            {
                var (sc, p) = _streetManager.GetClosestSpline(transform.position);
                _splineContainer = sc;
            }

            UpdateTarget();
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(gameObject.transform.position, gameObject.transform.forward * evaluationDistance);
        }

        void UpdateTarget()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif

            var dist = _agent.remainingDistance;
            if (!(dist <= _agent.stoppingDistance * 2) && _agent.hasPath)
                return;


            SplineHelper.GetClosestPoint(
            _splineContainer,
            evaluationPoint,
            out var nearestWorldPoint,
            out _,
            out _);
            
            Debug.DrawLine(evaluationPoint, nearestWorldPoint, Color.pink);
            _agent.SetDestination(nearestWorldPoint);
        }

        public override float GetTargetSpeed()
        {
            return _agent.speed;
        }

        public override void SetTargetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public override void SetTargetLocation(Vector3 position)
        {
            _agent.SetDestination(position);
        }
    }
}
