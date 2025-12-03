using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using Dreamteck.Splines;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Streets
{
    public enum JunctionType
    {
        Entry,
        Exit,
        Both
    }

    [Serializable]
    public struct JunctionData
    {
        public JunctionType type;
        public float priority;

        public JunctionData(JunctionType type, float priority)
        {
            this.type = type;
            this.priority = priority;
        }
    }

    [ExecuteAlways]
    [RequireComponent(typeof(Node))]
    public class Junction : MonoBehaviour
    {
        public SerializedDictionary<Node.Connection, JunctionData> connections = new();
        private readonly Dictionary<Node.Connection, JunctionData> _exits = new();

        private Node _node;

        private void Start()
        {
            _node = GetComponent<Node>();
            RebuildNodeLinks();
            UpdateJunction();
        }

        private void OnValidate()
        {
            RebuildNodeLinks();
            UpdateJunction();
        }

        private void RebuildNodeLinks()
        {
            if (!_node) return;

            _node.UpdateConnectedComputers();
            foreach (var c in _node.GetConnections()) // treat node as source of truth
            {
                if (c.spline.GetNode(c.pointIndex))
                    continue; // node exists already

                c.spline.ConnectNode(_node, c.pointIndex);
            }
        }

        private void UpdateJunction()
        {
            if (!_node) return;

            //update list of connections
            var connIn = _node.GetConnections().ToList();
            for (var i = 0; i < connections.Count; i++)
            {
                var conn = connections.Keys.ElementAt(i);
                if (!connIn.Contains(conn))
                    // cache contains dead item
                    connections.Remove(conn);
                else
                    // remove exising item from incoming
                    connIn.Remove(conn);
            }

            foreach (var c in connIn) connections.Add(c, new JunctionData());

            // assign connection type
            for (var i = 0; i < connections.Count; i++)
            {
                var (connection, value) = connections.ElementAt(i);
                var type = JunctionType.Both;
                if (!connection.spline.isClosed) // has ends
                    if (connection.pointIndex == 0) type = JunctionType.Exit;
                    else if (connection.pointIndex == connection.spline.pointCount - 1) type = JunctionType.Entry;

                if (value.priority == 0) value.priority = float.Epsilon; // ensure random exit never fails
                connections[connection] = new JunctionData(type, value.priority);
            }

            // update exit cache
            _exits.Clear();
            foreach (var kvp in connections.Where(kvp => kvp.Value.type is JunctionType.Both or JunctionType.Exit))
                _exits.Add(kvp.Key, kvp.Value);
        }

        public Node.Connection GetRandomExit() // todo untested
        {
            if (_exits.Count == 1) return _exits.First().Key;

            var r = _exits.Sum(kvp => kvp.Value.priority);
            var v = Random.Range(0, r);
            var b = 0f;
            foreach (var kvp in _exits)
            {
                if (v <= kvp.Value.priority + b && v > b) return kvp.Key;
                b += kvp.Value.priority;
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}