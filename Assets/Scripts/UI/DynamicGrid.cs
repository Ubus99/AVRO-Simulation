using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [ExecuteAlways]
    public class DynamicGrid : LayoutGroup
    {
        public Vector2Int spacing = Vector2Int.one;
        Vector2Int _grid;

        public Vector2Int cellSize { get; private set; } = new(256, 256);

        public event Action OnLayoutChanged;

        void UpdateGrid()
        {
            var cc = rectChildren.Count;
            if (cc < 1) return; //avoid divide by zero
            if (cc % 2 == 1 && cc != 1) cc++; // round to even number

            var factors = FactorPairs(cc);
            _grid = factors[0]; // pick lowest

            var s = rectTransform.sizeDelta;
            var bounds = Vector2Int.RoundToInt(s) - new Vector2Int(padding.horizontal, padding.vertical);

            var cs = new Vector2Int(
            (bounds.x - spacing.x * (_grid.x - 1)) / _grid.x,
            (bounds.y - spacing.y * (_grid.y - 1)) / _grid.y);
            var csi = new Vector2Int( //inverted
            (bounds.x - spacing.x * (_grid.y - 1)) / _grid.y,
            (bounds.y - spacing.y * (_grid.x - 1)) / _grid.x);

            if (cs.sqrMagnitude > csi.sqrMagnitude)
            {
                cellSize = csi;
                _grid = new Vector2Int(_grid.y, _grid.x);
            }
            else
            {
                cellSize = cs;
            }
        }

        /// <summary>
        ///     Calculates all possible factor pairs for a given integer
        /// </summary>
        /// <param name="n">Integer to dissect</param>
        /// <returns>List of possible factor pairs</returns>
        static List<Vector2Int> FactorPairs(int n)
        {
            var a = new List<Vector2Int>();
            for (var i = 1; i <= math.sqrt(n); i++)
            {
                a.Add(new Vector2Int(i, n / i));
            }
            return a.OrderBy(v => v.sqrMagnitude).ToList();
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            UpdateGrid();

            var width = padding.horizontal + (cellSize.x + spacing.x) * _grid.x - spacing.x;
            SetLayoutInputForAxis(
            width,
            width,
            -1,
            0);
        }

        public override void CalculateLayoutInputVertical()
        {
            UpdateGrid();

            var height = padding.vertical + (cellSize.y + spacing.y) * _grid.y - spacing.y;
            SetLayoutInputForAxis(
            height,
            height,
            -1,
            0);
        }

        public override void SetLayoutHorizontal()
        {
            SetChildSize();
        }

        public override void SetLayoutVertical()
        {
            SetChildSize();

            for (int i = 0, x = 0; x < _grid.x; x++)
            for (var y = 0; y < _grid.y; y++, i++)
            {
                if (i >= rectChildren.Count) return;

                var rect = rectChildren[i];
                SetChildAlongAxis(rect, 0, padding.left + (cellSize.x + spacing.x) * x);
                SetChildAlongAxis(rect, 1, padding.top + (cellSize.y + spacing.y) * y);
            }
        }

        void SetChildSize()
        {
            foreach (var rect in rectChildren)
            {
                rect.anchorMin = Vector2.up;
                rect.anchorMax = Vector2.up;
                rect.sizeDelta = cellSize;
            }
        }
    }
}
