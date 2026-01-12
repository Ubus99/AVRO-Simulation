using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
using Utils;
using Utils.Editor;
using Utils.Objects;

namespace Streets
{
    [ExecuteInEditMode]
    public class StreetNet : MonoBehaviour
    {
        public List<Street> streets;
        public GameObject junctions;
        public GameObject nodes;
        public JunctionTrigger triggerPrefab;
        public GameObject streetVizPrefab;

        private readonly Dictionary<Street.Address, JunctionTrigger> _junctionTriggers = new();

        private bool _dirty;

        private void Start()
        {
            UpdateStreets();
        }

        private void Update()
        {
            UpdateStreets();
        }

        private void OnValidate()
        {
            _dirty = true;
        }

        private void UpdateStreets()
        {
#if UNITY_EDITOR
            if (!_dirty) return;

            //flatten tree
            var exits = new Dictionary<Street.Exit, Street>();
            foreach (var s in streets)
            foreach (var e in s.exitLanes)
                exits.TryAdd(e, s);

            ObjectManagementUtility.KillAllChildren(junctions.transform);
            ObjectManagementUtility.KillAllChildren(nodes.transform);
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

        private SplineContainer AddBranch(string name, BezierKnot knot1, BezierKnot knot2)
        {
            if (!streetVizPrefab) return null; 
            
            var go = Instantiate(streetVizPrefab, junctions.transform, true);
            go.name = name;

            var sc = go.GetComponent<SplineContainer>();
            var spline = sc[0];
            spline.Clear();

            spline.Add(knot1);
            spline.Add(knot2);
            spline.Closed = false;
            
            var ns = go.GetComponent<NavMeshSnap>();
            ns.offset = Vector3.down;

            return sc;
        }

        private void AddTrigger(Vector3 pos, Street.Address a, SplineContainer spline = null)
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
            if (spline) junction.junctionData.TryAdd(spline, 0);
        }
    }
}