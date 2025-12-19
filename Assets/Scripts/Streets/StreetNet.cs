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

        void Start()
        {
            UpdateStreets();
        }

        void Update()
        {
            UpdateStreets();
        }

        void OnValidate()
        {
            _dirty = true;
        }

        void UpdateStreets()
        {
#if UNITY_EDITOR
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
                var knotStart = e.myAddress.GetKnot();
                knotStart.Position = e.myAddress.GetWorldPoint();
                foreach (var a in e.targets) // iterate endpoints
                {
                    // exit knot
                    var knotTarget = a.GetKnot();
                    knotTarget.Position = a.GetWorldPoint();

                    // apply
                    var branch = AddBranch($"Branch_{e.myAddress.street.name}_{a.street.name}", knotStart, knotTarget);
                    AddTrigger(knotStart.Position, e.myAddress, branch);
                    AddTrigger(knotTarget.Position, a, branch);
                }
            }

            _dirty = false;
#endif
        }

        SplineContainer AddBranch(string name, BezierKnot knot1, BezierKnot knot2)
        {
            var go = new GameObject(name, typeof(SplineContainer));
            go.transform.SetParent(junctions.transform);

            var sc = go.GetComponent<SplineContainer>();
            var spline = sc[0];
            spline.Clear();

            spline.Add(knot1);
            spline.Add(knot2);

            return sc;
        }

        void AddTrigger(Vector3 pos, Street.Address a, SplineContainer spline = null)
        {
            // load from cache
            if (!_junctionTriggers.TryGetValue(a, out var junction) || !junction)
            {
                if (!triggerPrefab) return;

                var go = PrefabUtility.InstantiatePrefab(triggerPrefab.gameObject) as GameObject;
                if (!go) return;

                go.transform.SetParent(nodes.transform);
                go.transform.position = pos;
                go.name = $"Node_{a.street.name}_{a.idx}";

                _junctionTriggers[a] = junction = go.GetComponent<JunctionTrigger>();
            }

            // set data
            junction.junctionData.TryAdd(a.GetSpline(), 0);
            if (spline)
            {
                junction.junctionData.TryAdd(spline, 0);
            }
        }
    }
}
