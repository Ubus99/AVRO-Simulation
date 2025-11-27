using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    [RequireComponent(typeof(GridLayoutGroup))]
    public class DynamicGrid : MonoBehaviour
    {
        public delegate void UpdatedEventHandler();

        public Vector2Int margin = Vector2Int.one;
        public Vector2Int spacing = Vector2Int.one;
        bool _dirty;
        GridLayoutGroup _gridLayout;
        int _lastChildren;

        public Vector2Int CellSize { get; private set; } = new(256, 256);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _gridLayout = gameObject.GetComponent<GridLayoutGroup>();
            _dirty = true;
        }

        // Update is called once per frame
        void Update()
        {
            var childCount = transform.childCount;
            if (_lastChildren != childCount)
            {
                _dirty = true;
                _lastChildren = childCount;
            }

            if (_dirty)
            {
                UpdateGrid();
                OnLayoutChanged?.Invoke();
            }
            _dirty = false;
        }

        void OnRectTransformDimensionsChange()
        {
            _dirty = true;
        }

        void OnValidate()
        {
            _dirty = true;
        }

        public event UpdatedEventHandler OnLayoutChanged;

        void UpdateGrid()
        {
            _gridLayout.spacing = spacing;
            _gridLayout.padding = new RectOffset(margin.x, margin.x, margin.y, margin.y);

            var cc = transform.childCount;
            if (cc < 1) return; //avoid divide by zero
            if (cc % 2 == 1 && cc != 1) cc++; // only even numbers
            var s = GetComponent<RectTransform>().rect.size;
            var bounds = Vector2Int.RoundToInt(s);

            var factors = FactorPairs(cc);
            var fOpt = factors[0];
            foreach (var f in factors.Where(f => fOpt.sqrMagnitude > f.sqrMagnitude))
            {
                fOpt = f;
            }

            bounds -= margin * 2;
            var cs = new Vector2Int(
            (bounds.x - spacing.x * (fOpt.x - 1)) / fOpt.x,
            (bounds.y - spacing.y * (fOpt.y - 1)) / fOpt.y
            );
            var csi = new Vector2Int( //inverse
            (bounds.x - spacing.x * (fOpt.y - 1)) / fOpt.y,
            (bounds.y - spacing.y * (fOpt.x - 1)) / fOpt.x
            );

            CellSize = cs.sqrMagnitude > csi.sqrMagnitude ? csi : cs;
            _gridLayout.cellSize = CellSize;
        }

        static List<Vector2Int> FactorPairs(int n)
        {
            var a = new List<Vector2Int>();
            for (var i = 1; i <= math.sqrt(n); i++)
            {
                a.Add(new Vector2Int(i, n / i));
            }
            return a;
        }
    }
}
