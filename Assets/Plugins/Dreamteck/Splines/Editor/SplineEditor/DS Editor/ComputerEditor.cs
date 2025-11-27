using Dreamteck.Editor;
using UnityEditor;
using UnityEngine;

namespace Dreamteck.Splines.Editor
{
    public class ComputerEditor : SplineEditorBase
    {
        readonly SerializedProperty _customNormalInterpolation;
        readonly SerializedProperty _customValueInterpolation;
        readonly SerializedProperty _knotParametrization;
        readonly SerializedProperty _linearAverageDirection;
        readonly ComputerEditorModule[] _modules = new ComputerEditorModule[0];
        readonly SerializedProperty _multithreaded;
        readonly Toolbar _operationsToolbar;
        readonly SerializedProperty _optimizeAngleThreshold;
        readonly DreamteckSplinesEditor _pathEditor;
        readonly SerializedProperty _sampleMode;
        readonly SerializedProperty _sampleRate;
        readonly SerializedProperty _space;
        readonly SplineComputer _spline;

        readonly SerializedProperty _splineProperty;
        readonly SplineComputer[] _splines = new SplineComputer[0];
        readonly Toolbar _transformToolbar;
        readonly SerializedProperty _type;
        readonly SerializedProperty _updateMode;
        readonly Toolbar _utilityToolbar;
        int _operation = -1, _module = -1, _transformTool = 1;
        bool _pathToolsFoldout, _interpolationFoldout;
        public bool drawComputer = true;
        public bool drawConnectedComputers = true;
        public bool drawPivot = true;


        public ComputerEditor(SplineComputer[] splines, SerializedObject serializedObject,
            DreamteckSplinesEditor pathEditor) : base(serializedObject)
        {
            _spline = splines[0];
            _splines = splines;
            _pathEditor = pathEditor;

            _splineProperty = serializedObject.FindProperty("_spline");
            _sampleRate = serializedObject.FindProperty("_spline").FindPropertyRelative("sampleRate");
            _type = serializedObject.FindProperty("_spline").FindPropertyRelative("type");
            _linearAverageDirection = _splineProperty.FindPropertyRelative("linearAverageDirection");
            _space = serializedObject.FindProperty("_space");
            _sampleMode = serializedObject.FindProperty("_sampleMode");
            _optimizeAngleThreshold = serializedObject.FindProperty("_optimizeAngleThreshold");
            _updateMode = serializedObject.FindProperty("updateMode");
            _multithreaded = serializedObject.FindProperty("multithreaded");
            _customNormalInterpolation = _splineProperty.FindPropertyRelative("customNormalInterpolation");
            _customValueInterpolation = _splineProperty.FindPropertyRelative("customValueInterpolation");
            _knotParametrization = _splineProperty.FindPropertyRelative("_knotParametrization");


            _modules = new ComputerEditorModule[2];
            _modules[0] = new ComputerMergeModule(_spline);
            _modules[1] = new ComputerSplitModule(_spline);
            GUIContent[] utilityContents = new GUIContent[_modules.Length],
                utilityContentsSelected = new GUIContent[_modules.Length];
            for (var i = 0; i < _modules.Length; i++)
            {
                utilityContents[i] = _modules[i].GetIconOff();
                utilityContentsSelected[i] = _modules[i].GetIconOn();
                _modules[i].undoHandler += OnRecordUndo;
                _modules[i].repaintHandler += OnRepaint;
            }
            _utilityToolbar = new Toolbar(utilityContents, utilityContentsSelected, 35f);
            _utilityToolbar.newLine = false;


            var index = 0;
            GUIContent[] transformContents = new GUIContent[4], transformContentsSelected = new GUIContent[4];
            transformContents[index] = new GUIContent("OFF");
            transformContentsSelected[index++] = new GUIContent("OFF");

            transformContents[index] = EditorGUIUtility.IconContent("MoveTool");
            transformContentsSelected[index++] = EditorGUIUtility.IconContent("MoveTool On");

            transformContents[index] = EditorGUIUtility.IconContent("RotateTool");
            transformContentsSelected[index++] = EditorGUIUtility.IconContent("RotateTool On");

            transformContents[index] = EditorGUIUtility.IconContent("ScaleTool");
            transformContentsSelected[index] = EditorGUIUtility.IconContent("ScaleTool On");

            _transformToolbar = new Toolbar(transformContents, transformContentsSelected, 35f);
            _transformToolbar.newLine = false;

            index = 0;
            GUIContent[] operationContents = new GUIContent[3], operationContentsSelected = new GUIContent[3];
            for (var i = 0; i < operationContents.Length; i++)
            {
                operationContents[i] = new GUIContent("");
                operationContentsSelected[i] = new GUIContent("");
            }
            _operationsToolbar = new Toolbar(operationContents, operationContentsSelected, 64f);
            _operationsToolbar.newLine = false;
        }

        void OnRecordUndo(string title)
        {
            if (undoHandler != null) undoHandler(title);
        }

        void OnRepaint()
        {
            if (repaintHandler != null) repaintHandler();
        }

        protected override void Load()
        {
            base.Load();
            _pathToolsFoldout = LoadBool("DreamteckSplinesEditor.pathToolsFoldout");
            _interpolationFoldout = LoadBool("DreamteckSplinesEditor.interpolationFoldout");
            _transformTool = LoadInt("DreamteckSplinesEditor.transformTool");
        }

        protected override void Save()
        {
            base.Save();
            SaveBool("DreamteckSplinesEditor.pathToolsFoldout", _pathToolsFoldout);
            SaveBool("DreamteckSplinesEditor.interpolationFoldout", _interpolationFoldout);
            SaveInt("DreamteckSplinesEditor.transformTool", _transformTool);
        }

        public override void Destroy()
        {
            base.Destroy();
            for (var i = 0; i < _modules.Length; i++) _modules[i].Deselect();
        }

        public override void DrawInspector()
        {
            base.DrawInspector();
            if (_spline == null) return;
            SplineEditorGUI.SetHighlightColors(SplinePrefs.highlightColor, SplinePrefs.highlightContentColor);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUI.BeginChangeCheck();
            _operationsToolbar.SetContent(0, new GUIContent(_spline.isClosed ? "Break" : "Close"));
            _operationsToolbar.SetContent(1, new GUIContent("Reverse"));
            _operationsToolbar.SetContent(2, new GUIContent(_spline.is2D ? "3D Mode" : "2D Mode"));
            _operationsToolbar.Draw(ref _operation);
            if (EditorGUI.EndChangeCheck())
            {
                PerformOperation();
            }
            EditorGUI.BeginChangeCheck();
            if (_splines.Length == 1)
            {
                var mod = _module;
                _utilityToolbar.Draw(ref mod);
                if (EditorGUI.EndChangeCheck())
                {
                    ToggleModule(mod);
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (_module >= 0 && _module < _modules.Length)
            {
                _modules[_module].DrawInspector();
            }
            EditorGUILayout.Space();
            DreamteckEditorGUI.DrawSeparator();

            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            var lastType = (Spline.Type)_type.intValue;
            EditorGUILayout.PropertyField(_type);
            if (lastType == Spline.Type.CatmullRom && _type.intValue == (int)Spline.Type.Bezier)
            {
                if (EditorUtility.DisplayDialog("Hermite to Bezier",
                    "Would you like to retain the Catmull Rom shape in Bezier mode?",
                    "Yes",
                    "No"))
                {
                    for (var i = 0; i < _splines.Length; i++)
                    {
                        Undo.RecordObject(_splines[i], "CatToBezierTangents");
                        _splines[i].CatToBezierTangents();
                        EditorUtility.SetDirty(_splines[i]);
                    }
                    _pathEditor.SetPointsArray(_spline.GetPoints());
                    _pathEditor.ApplyModifiedProperties();
                }
            }

            if (_spline.type == Spline.Type.CatmullRom)
            {
                var type = Mathf.RoundToInt(_knotParametrization.floatValue * 2);
                var catmullTypeText = "Parametrization: ";
                switch (type)
                {
                    case 0: catmullTypeText += "Uniform"; break;
                    case 1: catmullTypeText += "Centripetal"; break;
                    case 2: catmullTypeText += "Chordal"; break;
                }
                EditorGUILayout.PropertyField(_knotParametrization, new GUIContent(catmullTypeText));
            }

            if (_spline.type == Spline.Type.Linear)
            {
                EditorGUILayout.PropertyField(_linearAverageDirection);
            }

            var lastSpace = _space.intValue;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_space, new GUIContent("Space"));
            if ((SplineComputer.Space)_space.enumValueIndex == SplineComputer.Space.Local)
            {
                EditTransformToolbar();
                if (_splines.Length == 1)
                {
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.PropertyField(_sampleMode, new GUIContent("Sample Mode"));
            if (_sampleMode.intValue == (int)SplineComputer.SampleMode.Optimized)
                EditorGUILayout.PropertyField(_optimizeAngleThreshold);
            EditorGUILayout.PropertyField(_updateMode);
            if (_updateMode.intValue == (int)SplineComputer.UpdateMode.None && Application.isPlaying)
            {
                if (GUILayout.Button("Rebuild"))
                {
                    for (var i = 0; i < _splines.Length; i++) _splines[i].RebuildImmediate(true, true);
                }
            }
            if (_spline.type != Spline.Type.Linear)
                EditorGUILayout.PropertyField(_sampleRate, new GUIContent("Sample Rate"));
            EditorGUILayout.PropertyField(_multithreaded);

            EditorGUI.indentLevel++;
            var curveUpdate = false;
            _interpolationFoldout = EditorGUILayout.Foldout(_interpolationFoldout, "Point Value Interpolation");
            if (_interpolationFoldout)
            {
                if (_customValueInterpolation.animationCurveValue == null ||
                    _customValueInterpolation.animationCurveValue.keys.Length == 0)
                {
                    if (GUILayout.Button("Size & Color Interpolation"))
                    {
                        var curve = new AnimationCurve();
                        curve.AddKey(new Keyframe(0, 0, 0, 0));
                        curve.AddKey(new Keyframe(1, 1, 0, 0));
                        for (var i = 0; i < _splines.Length; i++) _splines[i].customValueInterpolation = curve;
                        _serializedObject.Update();
                        _pathEditor.GetPointsFromSpline();
                        curveUpdate = true;
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_customValueInterpolation,
                    new GUIContent("Size & Color Interpolation"));
                    if (GUILayout.Button("x", GUILayout.MaxWidth(25)))
                    {
                        _customValueInterpolation.animationCurveValue = null;
                        for (var i = 0; i < _splines.Length; i++) _splines[i].customValueInterpolation = null;
                        _serializedObject.Update();
                        _pathEditor.GetPointsFromSpline();
                        curveUpdate = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (_customNormalInterpolation.animationCurveValue == null ||
                    _customNormalInterpolation.animationCurveValue.keys.Length == 0)
                {
                    if (GUILayout.Button("Normal Interpolation"))
                    {
                        var curve = new AnimationCurve();
                        curve.AddKey(new Keyframe(0, 0));
                        curve.AddKey(new Keyframe(1, 1));
                        for (var i = 0; i < _splines.Length; i++) _splines[i].customNormalInterpolation = curve;
                        _serializedObject.Update();
                        _pathEditor.GetPointsFromSpline();
                        curveUpdate = true;
                    }
                }
                else
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(_customNormalInterpolation, new GUIContent("Normal Interpolation"));
                    if (GUILayout.Button("x", GUILayout.MaxWidth(25)))
                    {
                        _customNormalInterpolation.animationCurveValue = null;
                        for (var i = 0; i < _splines.Length; i++) _splines[i].customNormalInterpolation = null;
                        _serializedObject.Update();
                        _pathEditor.GetPointsFromSpline();
                        curveUpdate = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUI.indentLevel--;

            if (EditorGUI.EndChangeCheck() || curveUpdate)
            {
                if (_sampleRate.intValue < 2)
                {
                    _sampleRate.intValue = 2;
                }

                var forceUpdateAll = false;
                if (lastSpace != _space.intValue)
                {
                    forceUpdateAll = true;
                }

                _pathEditor.ApplyModifiedProperties(true);

                for (var i = 1; i < _splines.Length; i++)
                {
                    _splines[i].RebuildImmediate(true, forceUpdateAll);
                }
            }

            if (_pathEditor.currentModule != null)
            {
                _transformTool = 0;
            }
        }

        void EditTransformToolbar()
        {
            if (_splines.Length > 1)
            {
                GUILayout.Label("Edit Transform unavailable with multiple splines");
                return;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Edit Transform - Only");
            GUILayout.FlexibleSpace();
            var lastTool = _transformTool;
            _transformToolbar.Draw(ref _transformTool);
            if (lastTool != _transformTool && _transformTool > 0)
            {
                _pathEditor.UntoggleCurrentModule();
                Tools.current = Tool.None;
            }
            EditorGUILayout.EndHorizontal();

            switch (_transformTool)
            {
                case 1:
                    var position = _spline.transform.position;

                    position = EditorGUILayout.Vector3Field("Position", position);
                    if (position != _spline.transform.position)
                    {
                        Undo.RecordObject(_spline.transform, "Move spline computer");
                        _spline.transform.position = position;
                        _pathEditor.ApplyModifiedProperties(true);
                    }
                    break;
                case 2:
                    var rotation = _spline.transform.rotation;
                    rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", rotation.eulerAngles));
                    if (rotation != _spline.transform.rotation)
                    {
                        Undo.RecordObject(_spline.transform, "Rotate spline computer");
                        _spline.transform.rotation = rotation;
                        _pathEditor.ApplyModifiedProperties(true);
                    }
                    break;
                case 3:
                    var scale = _spline.transform.localScale;
                    scale = EditorGUILayout.Vector3Field("Scale", scale);
                    if (scale != _spline.transform.localScale)
                    {
                        Undo.RecordObject(_spline.transform, "Scale spline computer");
                        _spline.transform.localScale = scale;
                        _pathEditor.ApplyModifiedProperties(true);
                    }
                    break;
            }
        }

        void PerformOperation()
        {
            switch (_operation)
            {
                case 0:
                    if (_spline.isClosed)
                    {
                        BreakSpline();
                    }
                    else
                    {
                        CloseSpline();
                    }
                    _operation = -1;
                    break;
                case 1:
                    ReversePointOrder();
                    _operation = -1;
                    break;
                case 2:
                {
                    _pathEditor.is2D = !_pathEditor.is2D;

                    if (_pathEditor.is2D)
                    {
                        for (var i = 0; i < _pathEditor.points.Length; i++)
                        {
                            FlattenPoint(ref _pathEditor.points[i], LinearAlgebraUtility.Axis.Z);
                        }
                    }

                    _pathEditor.ApplyModifiedProperties();
                    _operation = -1;
                }
                    break;
            }
        }

        void FlattenPoint(ref SerializedSplinePoint point, LinearAlgebraUtility.Axis axis, float flatValue = 0f)
        {
            point.position = LinearAlgebraUtility.FlattenVector(point.position, axis, flatValue);
            point.tangent = LinearAlgebraUtility.FlattenVector(point.tangent, axis, flatValue);
            point.tangent2 = LinearAlgebraUtility.FlattenVector(point.tangent2, axis, flatValue);
            switch (axis)
            {
                case LinearAlgebraUtility.Axis.X: point.normal = Vector3.right; break;
                case LinearAlgebraUtility.Axis.Y: point.normal = Vector3.up; break;
                case LinearAlgebraUtility.Axis.Z: point.normal = Vector3.forward; break;
            }
        }

        void ToggleModule(int index)
        {
            if (_module >= 0 && _module < _modules.Length) _modules[_module].Deselect();
            if (_module == index) index = -1;
            _module = index;
            if (_module >= 0 && _module < _modules.Length) _modules[_module].Select();
        }

        public void BreakSpline()
        {
            RecordUndo("Break path");
            if (_splines.Length == 1 && _pathEditor.selectedPoints.Count == 1)
            {
                _spline.Break(_pathEditor.selectedPoints[0]);
                EditorUtility.SetDirty(_spline);
                _pathEditor.selectedPoints.Clear();
                _pathEditor.selectedPoints.Add(0);

            }
            else
            {
                for (var i = 0; i < _splines.Length; i++)
                {
                    _splines[i].Break();
                    EditorUtility.SetDirty(_splines[i]);
                }
            }
        }

        public void CloseSpline()
        {
            RecordUndo("Close path");
            for (var i = 0; i < _splines.Length; i++)
            {
                _splines[i].Close();
            }
        }

        void ReversePointOrder()
        {
            for (var i = 0; i < _splines.Length; i++)
            {
                ReversePointOrder(_splines[i]);
            }
        }

        void ReversePointOrder(SplineComputer spline)
        {
            var points = spline.GetPoints();
            for (var i = 0; i < Mathf.FloorToInt(points.Length / 2); i++)
            {
                var temp = points[i];
                points[i] = points[points.Length - 1 - i];
                var tempTan = points[i].tangent;
                points[i].tangent = points[i].tangent2;
                points[i].tangent2 = tempTan;
                var opposideIndex = points.Length - 1 - i;
                points[opposideIndex] = temp;
                tempTan = points[opposideIndex].tangent;
                points[opposideIndex].tangent = points[opposideIndex].tangent2;
                points[opposideIndex].tangent2 = tempTan;
            }
            if (points.Length % 2 != 0)
            {
                var tempTan = points[Mathf.CeilToInt(points.Length / 2)].tangent;
                points[Mathf.CeilToInt(points.Length / 2)].tangent =
                    points[Mathf.CeilToInt(points.Length / 2)].tangent2;
                points[Mathf.CeilToInt(points.Length / 2)].tangent2 = tempTan;
            }
            spline.SetPoints(points);
        }

        public override void DrawScene(SceneView current)
        {
            base.DrawScene(current);

            if (_spline == null)
            {
                return;
            }

            for (var i = 0; i < _splines.Length; i++)
            {
                if (drawComputer)
                {
                    DSSplineDrawer.DrawSplineComputer(_splines[i]);
                }

                if (drawPivot)
                {
                    var trs = _splines[i].transform;
                    var size = HandleUtility.GetHandleSize(trs.position);
                    Handles.color = Color.red;
                    Handles.DrawLine(trs.position, trs.position + trs.right * size * 0.5f);
                    Handles.color = Color.green;
                    Handles.DrawLine(trs.position, trs.position + trs.up * size * 0.5f);
                    Handles.color = Color.blue;
                    Handles.DrawLine(trs.position, trs.position + trs.forward * size * 0.5f);
                }
            }

            if (drawConnectedComputers)
            {
                for (var i = 0; i < _splines.Length; i++)
                {
                    var computers = _splines[i].GetConnectedComputers();
                    for (var j = 1; j < computers.Count; j++)
                    {
                        DSSplineDrawer.DrawSplineComputer(computers[j], 0.0, 1.0, 0.5f);
                    }
                }
            }


            if (_pathEditor.currentModule == null)
            {
                if (_splines.Length > 1 || Tools.current != Tool.None)
                {
                    _transformTool = 0;
                }
                switch (_transformTool)
                {
                    case 1:
                        var position = _spline.transform.position;
                        position = Handles.PositionHandle(position, _spline.transform.rotation);
                        if (position != _spline.transform.position)
                        {
                            Undo.RecordObject(_spline.transform, "Move spline computer");
                            _spline.transform.position = position;
                            _pathEditor.ApplyModifiedProperties(true);
                        }
                        break;
                    case 2:
                        var rotation = _spline.transform.rotation;
                        rotation = Handles.RotationHandle(rotation, _spline.transform.position);
                        if (rotation != _spline.transform.rotation)
                        {
                            Undo.RecordObject(_spline.transform, "Rotate spline computer");
                            _spline.transform.rotation = rotation;
                            _pathEditor.ApplyModifiedProperties(true);
                        }
                        break;
                    case 3:
                        var scale = _spline.transform.localScale;
                        scale = Handles.ScaleHandle(scale,
                        _spline.transform.position,
                        _spline.transform.rotation,
                        HandleUtility.GetHandleSize(_spline.transform.position));
                        if (scale != _spline.transform.localScale)
                        {
                            Undo.RecordObject(_spline.transform, "Scale spline computer");
                            _spline.transform.localScale = scale;
                            _pathEditor.ApplyModifiedProperties(true);
                        }
                        break;
                }
                if (_transformTool > 0)
                {
                    var screenPosition = HandleUtility.WorldToGUIPoint(_spline.transform.position);
                    screenPosition.y += 20f;
                    Handles.BeginGUI();
                    DreamteckEditorGUI.Label(new Rect(screenPosition.x - 120 + _spline.name.Length * 4,
                    screenPosition.y,
                    120,
                    25),
                    _spline.name);
                    Handles.EndGUI();
                }
            }
            if (_module >= 0 && _module < _modules.Length) _modules[_module].DrawScene();
        }
    }
}
