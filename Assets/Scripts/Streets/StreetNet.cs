using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Utils;

namespace Streets
{
    [ExecuteInEditMode]
    public class StreetNet : MonoBehaviour
    {
        public List<Street> streets;
        public GameObject junctions;
        public GameObject nodes;
        public JunctionTrigger triggerPrefab;

        readonly Dictionary<Street.Address, JunctionTrigger> _junctionTriggers = new();

        bool _dirty;

        void Update()
        {
            if (!_dirty) return;

            //flatten tree
            var exits = new Dictionary<Street.Exit, Street>();
            foreach (var s in streets)
            foreach (var e in s.exitLanes)
            {
                exits.TryAdd(e, s);
            }

            ObjectManager.KillAllChildren(junctions.transform);
            ObjectManager.KillAllChildren(nodes.transform);
            _junctionTriggers.Clear();

            foreach (var (e, s) in exits) // iterate exits
            {
                // entry knot
                var knot1 = e.myAddress.GetKnot();
                knot1.Position = e.myAddress.GetWorldPoint();

                foreach (var a in e.targets) // iterate endpoints
                {
                    // exit knot
                    var knot2 = a.GetKnot();
                    knot2.Position = a.GetWorldPoint();

                    // apply
                    AddBranch($"Branch_{e.myAddress.lane}{a.street.name}", knot1, knot2);
                    AddTrigger(a, knot2.Position);
                }
            }

            _dirty = false;
        }

        void OnValidate()
        {
            _dirty = true;
        }

        void AddBranch(string name, BezierKnot knot1, BezierKnot knot2)
        {
            var go = new GameObject(name, typeof(SplineContainer));
            go.transform.SetParent(junctions.transform);

            var spline = go.GetComponent<SplineContainer>()[0];
            spline.Clear();

            spline.Add(knot1);
            spline.Add(knot2);
        }

        void AddTrigger(Street.Address a, Vector3 pos)
        {
            // load from cache
            if (!_junctionTriggers.TryGetValue(a, out var junction) || !junction)
            {
                if (!triggerPrefab) return;

                var go = PrefabUtility.InstantiatePrefab(triggerPrefab.gameObject) as GameObject;
                if (!go) return;

                go.transform.SetParent(nodes.transform);
                go.transform.position = pos;

                _junctionTriggers[a] = junction = go.GetComponent<JunctionTrigger>();
            }

            // set data
            junction.junctionData.TryAdd(a, 0);
        }
    }
}
