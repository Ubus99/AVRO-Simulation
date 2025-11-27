using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Streets
{
    [RequireComponent(typeof(SplineProjector))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshFollower : MonoBehaviour
    {
        NavMeshAgent _agent;
        float _baseSpeed;
        SplineProjector _projector;
        Vector3 _targetPosition;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            _projector = GetComponent<SplineProjector>();
            _agent = GetComponent<NavMeshAgent>();
            _baseSpeed = _agent.speed;
        }

        void Update()
        {
            var proj = new SplineSample();
            _projector.Project(transform.position, ref proj);
            var lead = 10 / _projector.CalculateLength();
            var prog = (float)(proj.percent + lead);
            if (prog >= 1) prog -= 1;

            var pos = _projector.EvaluatePosition(prog);

            _targetPosition = pos;
            _agent.destination = pos;
        }

        void OnEnable()
        {
            _projector.onNode += OnNode;
        }

        void OnDisable()
        {
            _projector.onNode -= OnNode;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_targetPosition, 0.5f);
        }

        public float GetTargetSpeed()
        {
            return _agent.speed;
        }

        public float GetBaseSpeed()
        {
            return _baseSpeed;
        }

        public void SetTargetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        void OnNode(List<SplineTracer.NodeConnection> passed)
        {
            Debug.Log("Reached node " + passed[0].node.name + " connected at point " + passed[0].point);
            var connections = passed[0].node.GetConnections();
            if (connections.Length == 1) return;
            var currentConnection = 0;

            for (var i = 0; i < connections.Length; i++)
            {
                if (connections[i].spline != _projector.spline
                    || connections[i].pointIndex != passed[0].point)
                    continue;

                currentConnection = i;
                break;
            }

            var newConnection = Random.Range(0, connections.Length);
            if (newConnection == currentConnection)
            {
                newConnection++;

                if (newConnection >= connections.Length) newConnection = 0;
            }

            SwitchSpline(connections[currentConnection], connections[newConnection]);
        }

        void SwitchSpline(Node.Connection from, Node.Connection to)
        {
            //Set the spline to the tracer
            _projector.spline = to.spline;
            _projector.RebuildImmediate();
        }
    }
}
