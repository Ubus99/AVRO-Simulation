using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    public SplineComputer spline;
    public SplineRenderer splineRenderer;
    readonly List<SplinePoint> _path = new();
    bool _dirty;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spline = GetComponent<SplineComputer>();
        splineRenderer = GetComponent<SplineRenderer>();
        splineRenderer.spline = spline;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_dirty) return;
        spline.SetPoints(_path.ToArray());
        spline.RebuildImmediate();
        _dirty = false;
    }

    public void SetPath(List<Vector3> path)
    {
        _dirty = true;
        _path.Clear();
        foreach (var p in path)
        {
            _path.Add(new SplinePoint(p));
        }
    }
}
