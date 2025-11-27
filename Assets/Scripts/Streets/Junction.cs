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
    }

    [ExecuteAlways]
    [RequireComponent(typeof(Node))]
    public class Junction : MonoBehaviour
    {
        public SerializedDictionary<Node.Connection, JunctionData> connections = new();
        readonly Dictionary<Node.Connection, JunctionData> _exits = new();

        Node _node;

        void Start()
        {
            _node = GetComponent<Node>();
            RebuildNodeLinks();
            UpdateJunction();
        }

        void OnValidate()
        {
            RebuildNodeLinks();
            UpdateJunction();
        }

        void RebuildNodeLinks()
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

        void UpdateJunction()
        {
            if (!_node) return;

            var connIn = _node.GetConnections().ToList();
            for (var i = 0; i < connections.Count; i++)
            {
                var conn = connections.Keys.ElementAt(i);
                if (!connIn.Contains(conn))
                {
                    // cache contains dead item
                    connections.Remove(conn);
                }
                else
                {
                    // remove exising item from incoming
                    connIn.Remove(conn);
                }
            }
            foreach (var c in connIn)
            {
                connections.Add(c, new JunctionData());
            }

            _exits.Clear();
            foreach (var kvp in connections.Where(kvp => kvp.Value.type is JunctionType.Both or JunctionType.Exit))
            {
                _exits.Add(kvp.Key, kvp.Value);
            }

        }

        public Node.Connection GetRandomExit() // todo untested
        {
            if (_exits.Count == 1) return _exits.First().Key;

            var r = _exits.Sum(kvp => kvp.Value.priority);
            var v = Random.Range(0, r);
            var b = 0f;
            foreach (var kvp in _exits)
            {
                if (v <= kvp.Value.priority + b && v > b)
                {
                    return kvp.Key;
                }
                b += kvp.Value.priority;
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
