using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

namespace Streets
{
    [ExecuteInEditMode]
    public class StreetNet : MonoBehaviour
    {
        public List<Street> streets;
        public GameObject junctions;
        public GameObject nodes;


        bool _dirty;

        void Update()
        {
            if (!_dirty) return;

            ClearBranches();

            //flatten tree
            var exits = new Dictionary<Street.Exit, Street>();
            foreach (var s in streets)
            foreach (var e in s.exitLanes)
            {
                exits.Add(e, s);
            }

            foreach (var (e, s) in exits)
            foreach (var t in e.targets)
            {
                var go = new GameObject($"Branch{e.lane}{t.street.name}", typeof(SplineContainer));
                go.transform.SetParent(junctions.transform);

                var spline = go.GetComponent<SplineContainer>()[0];
                spline.Clear();
                var knot1 = s.GetPointAtIndex(e.lane, e.index);
                var knot3 = t.street.GetPointAtIndex(t.lane, t.idx);

                spline.Add(knot1);
                spline.Add(knot3);
            }

            _dirty = false;
        }

        void OnValidate()
        {
            _dirty = true;
        }

        void ClearBranches()
        {
            for (var i = junctions.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(junctions.transform.GetChild(i).gameObject);
            }
        }
    }
}
