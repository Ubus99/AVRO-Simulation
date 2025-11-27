using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Dreamteck.Splines
{
    [AddComponentMenu("Dreamteck/Splines/Users/Object Bender")]
    public class ObjectBender : SplineUser
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        public enum ForwardMode
        {
            Spline,
            Custom
        }

        public enum NormalMode
        {
            Spline,
            Auto,
            Custom
        }

        [SerializeField]
        [HideInInspector]
        bool _bend;

        [HideInInspector]
        public BendProperty[] bendProperties = new BendProperty[0];

        [SerializeField]
        [HideInInspector]
        bool _parentIsTheSpline;

        [SerializeField]
        [HideInInspector]
        TS_Bounds bounds;

        [SerializeField]
        [HideInInspector]
        Axis _axis = Axis.Z;

        [SerializeField]
        [HideInInspector]
        NormalMode _normalMode = NormalMode.Auto;

        [SerializeField]
        [HideInInspector]
        ForwardMode _forwardMode = ForwardMode.Spline;

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("_upVector")]
        Vector3 _customNormal = Vector3.up;

        [SerializeField]
        [HideInInspector]
        Vector3 _customForward = Vector3.forward;

        Quaternion bendRotation = Quaternion.identity;
        Matrix4x4 normalMatrix;

        public bool bend
        {
            get { return _bend; }
            set
            {
                if (_bend != value)
                {
                    _bend = value;
                    if (value)
                    {
                        UpdateReferences();
                        Rebuild();
                    }
                    else
                    {
                        Revert();
                    }
                }
            }
        }

        public Axis axis
        {
            get { return _axis; }
            set
            {
                if (spline != null && value != _axis)
                {
                    _axis = value;
                    UpdateReferences();
                    Rebuild();
                }
                else
                {
                    _axis = value;
                }
            }
        }

        public NormalMode upMode
        {
            get { return _normalMode; }
            set
            {
                if (spline != null && value != _normalMode)
                {
                    _normalMode = value;
                    Rebuild();
                }
                else
                {
                    _normalMode = value;
                }
            }
        }

        public Vector3 customNormal
        {
            get { return _customNormal; }
            set
            {
                if (spline != null && value != _customNormal)
                {
                    _customNormal = value;
                    Rebuild();
                }
                else
                {
                    _customNormal = value;
                }
            }
        }

        public ForwardMode forwardMode
        {
            get { return _forwardMode; }
            set
            {
                if (spline != null && value != _forwardMode)
                {
                    _forwardMode = value;
                    Rebuild();
                }
                else
                {
                    _forwardMode = value;
                }
            }
        }

        public Vector3 customForward
        {
            get { return _customForward; }
            set
            {
                if (spline != null && value != _customForward)
                {
                    _customForward = value;
                    Rebuild();
                }
                else
                {
                    _customForward = value;
                }
            }
        }

        void GetTransformsRecursively(Transform current, ref List<Transform> transformList)
        {
            transformList.Add(current);
            foreach (Transform child in current)
            {
                GetTransformsRecursively(child, ref transformList);
            }
        }

        void GetObjects()
        {
            var found = new List<Transform>();
            GetTransformsRecursively(transform, ref found);
            var newProperties = new BendProperty[found.Count];
            for (var i = 0; i < found.Count; i++)
            {
                CreateProperty(ref newProperties[i], found[i]);
            }
            bendProperties = newProperties;
            var splineComponent = GetComponent<SplineComputer>();
            _parentIsTheSpline = splineComponent == spline;
        }

        public TS_Bounds GetBounds()
        {
            return new TS_Bounds(bounds.min, bounds.max, bounds.center);
        }

#if UNITY_EDITOR
        public void EditorGenerateLightmapUVs()
        {
            for (var i = 0; i < bendProperties.Length; i++)
            {
                if (bendProperties[i].bendMesh)
                {
                    if (bendProperties[i].filter == null) continue;
                    if (bendProperties[i].filter.sharedMesh == null) continue;
                    EditorUtility.DisplayProgressBar("Generating Lightmap UVS",
                    bendProperties[i].filter.sharedMesh.name,
                    (float)i / (bendProperties.Length - 1));
                    Unwrapping.GenerateSecondaryUVSet(bendProperties[i].filter.sharedMesh);
                }
            }
            EditorUtility.ClearProgressBar();
        }
#endif

        void CreateProperty(ref BendProperty property, Transform t)
        {
            property = new BendProperty(t, t == transform); //Create a new bend property for each child
            for (var i = 0; i < bendProperties.Length; i++)
            {
                //Search for properties that have the same trasform and copy their settings
                if (bendProperties[i].transform.transform == t)
                {
                    property.enabled = bendProperties[i].enabled;
                    property.applyRotation = bendProperties[i].applyRotation;
                    property.applyScale = bendProperties[i].applyScale;
                    property.bendMesh = bendProperties[i].bendMesh;
                    property.bendCollider = bendProperties[i].bendCollider;
                    property.generateLightmapUVs = bendProperties[i].generateLightmapUVs;
                    property.colliderUpdateRate = bendProperties[i].colliderUpdateRate;
                    break;
                }
            }

            if (t.transform != trs)
            {
                property.originalPosition = trs.InverseTransformPoint(t.position);
                property.originalRotation = Quaternion.Inverse(trs.rotation) * t.rotation;
            }
        }

        void CalculateBounds()
        {
            if (bounds == null) bounds = new TS_Bounds(Vector3.zero, Vector3.zero);
            bounds.min = bounds.max = Vector3.zero;
            for (var i = 0; i < bendProperties.Length; i++)
            {
                CalculatePropertyBounds(ref bendProperties[i]);
            }
            for (var i = 0; i < bendProperties.Length; i++)
            {
                CalculatePercents(bendProperties[i]);
            }
        }

        void CalculatePropertyBounds(ref BendProperty property)
        {
            if (!property.enabled) return;
            if (property.isParent && _parentIsTheSpline) return;
            if (property.transform.transform == trs)
            {
                if (0f < bounds.min.x) bounds.min.x = 0f;
                if (0f < bounds.min.y) bounds.min.y = 0f;
                if (0f < bounds.min.z) bounds.min.z = 0f;
                if (0f > bounds.max.x) bounds.max.x = 0f;
                if (0f > bounds.max.y) bounds.max.y = 0f;
                if (0f > bounds.max.z) bounds.max.z = 0f;
            }
            else
            {
                if (property.originalPosition.x < bounds.min.x) bounds.min.x = property.originalPosition.x;
                if (property.originalPosition.y < bounds.min.y) bounds.min.y = property.originalPosition.y;
                if (property.originalPosition.z < bounds.min.z) bounds.min.z = property.originalPosition.z;
                if (property.originalPosition.x > bounds.max.x) bounds.max.x = property.originalPosition.x;
                if (property.originalPosition.y > bounds.max.y) bounds.max.y = property.originalPosition.y;
                if (property.originalPosition.z > bounds.max.z) bounds.max.z = property.originalPosition.z;
            }
            if (property.editMesh != null)
            {
                for (var n = 0; n < property.editMesh.vertices.Length; n++)
                {
                    var localPos = property.transform.TransformPoint(property.editMesh.vertices[n]);
                    localPos = trs.InverseTransformPoint(localPos);
                    if (localPos.x < bounds.min.x) bounds.min.x = localPos.x;
                    if (localPos.y < bounds.min.y) bounds.min.y = localPos.y;
                    if (localPos.z < bounds.min.z) bounds.min.z = localPos.z;
                    if (localPos.x > bounds.max.x) bounds.max.x = localPos.x;
                    if (localPos.y > bounds.max.y) bounds.max.y = localPos.y;
                    if (localPos.z > bounds.max.z) bounds.max.z = localPos.z;
                }
            }

            if (property.editColliderMesh != null)
            {
                for (var n = 0; n < property.editColliderMesh.vertices.Length; n++)
                {
                    var localPos = property.transform.TransformPoint(property.editColliderMesh.vertices[n]);
                    localPos = trs.InverseTransformPoint(localPos);
                    if (localPos.x < bounds.min.x) bounds.min.x = localPos.x;
                    if (localPos.y < bounds.min.y) bounds.min.y = localPos.y;
                    if (localPos.z < bounds.min.z) bounds.min.z = localPos.z;
                    if (localPos.x > bounds.max.x) bounds.max.x = localPos.x;
                    if (localPos.y > bounds.max.y) bounds.max.y = localPos.y;
                    if (localPos.z > bounds.max.z) bounds.max.z = localPos.z;
                }
            }

            if (property.originalSpline != null)
            {
                for (var n = 0; n < property.originalSpline.points.Length; n++)
                {
                    var localPos = trs.InverseTransformPoint(property.originalSpline.points[n].position);
                    if (localPos.x < bounds.min.x) bounds.min.x = localPos.x;
                    if (localPos.y < bounds.min.y) bounds.min.y = localPos.y;
                    if (localPos.z < bounds.min.z) bounds.min.z = localPos.z;
                    if (localPos.x > bounds.max.x) bounds.max.x = localPos.x;
                    if (localPos.y > bounds.max.y) bounds.max.y = localPos.y;
                    if (localPos.z > bounds.max.z) bounds.max.z = localPos.z;
                }
            }
            bounds.CreateFromMinMax(bounds.min, bounds.max);
        }

        public void CalculatePercents(BendProperty property)
        {
            if (property.transform.transform != trs)
                property.positionPercent = GetPercentage(trs.InverseTransformPoint(property.transform.position));
            else property.positionPercent = GetPercentage(Vector3.zero);
            if (property.editMesh != null)
            {
                if (property.vertexPercents.Length != property.editMesh.vertexCount)
                    property.vertexPercents = new Vector3[property.editMesh.vertexCount];
                if (property.editColliderMesh != null)
                {
                    if (property.colliderVertexPercents.Length != property.editMesh.vertexCount)
                        property.colliderVertexPercents = new Vector3[property.editColliderMesh.vertexCount];
                }
                for (var i = 0; i < property.editMesh.vertexCount; i++)
                {
                    var localVertex = property.transform.TransformPoint(property.editMesh.vertices[i]);
                    localVertex = trs.InverseTransformPoint(localVertex);
                    property.vertexPercents[i] = GetPercentage(localVertex);
                }
                if (property.editColliderMesh != null)
                {
                    for (var i = 0; i < property.editColliderMesh.vertexCount; i++)
                    {
                        var localVertex = property.transform.TransformPoint(property.editColliderMesh.vertices[i]);
                        localVertex = trs.InverseTransformPoint(localVertex);
                        property.colliderVertexPercents[i] = GetPercentage(localVertex);
                    }
                }
            }
            if (property.splineComputer != null)
            {
                var points = property.splineComputer.GetPoints();
                property.splinePointPercents = new Vector3[points.Length];
                property.primaryTangentPercents = new Vector3[points.Length];
                property.secondaryTangentPercents = new Vector3[points.Length];
                for (var i = 0; i < points.Length; i++)
                {
                    property.splinePointPercents[i] = GetPercentage(trs.InverseTransformPoint(points[i].position));
                    property.primaryTangentPercents[i] = GetPercentage(trs.InverseTransformPoint(points[i].tangent));
                    property.secondaryTangentPercents[i] = GetPercentage(trs.InverseTransformPoint(points[i].tangent2));
                }
            }
        }

        void Revert()
        {
            for (var i = 0; i < bendProperties.Length; i++)
            {
                bendProperties[i].Revert();
            }
        }


        public void UpdateReferences()
        {
            if (!hasTransform)
            {
                CacheTransform();
            }
            if (_bend)
            {
                for (var i = 0; i < bendProperties.Length; i++) bendProperties[i].Revert();
            }
            GetObjects();
            CalculateBounds();
            if (_bend)
            {
                Bend();
                for (var i = 0; i < bendProperties.Length; i++)
                {
                    bendProperties[i].Apply(i > 0 || trs != spline.transform);
                    bendProperties[i].Update();
                }
            }
        }

        void GetevalResult(Vector3 percentage)
        {
            switch (axis)
            {
                case Axis.X: Evaluate(percentage.x, ref evalResult); break;
                case Axis.Y: Evaluate(percentage.y, ref evalResult); break;
                case Axis.Z: Evaluate(percentage.z, ref evalResult); break;
            }
            switch (_normalMode)
            {
                case NormalMode.Auto: evalResult.up = Vector3.Cross(evalResult.forward, evalResult.right); break;
                case NormalMode.Custom: evalResult.up = _customNormal; break;
            }
            if (_forwardMode == ForwardMode.Custom) evalResult.forward = customForward;
            var right = evalResult.right;

            var axisRotation = Quaternion.identity;

            switch (axis)
            {
                case Axis.Z:
                    evalResult.position +=
                        right * Mathf.Lerp(bounds.min.x, bounds.max.x, percentage.x) * evalResult.size;
                    evalResult.position += evalResult.up * Mathf.Lerp(bounds.min.y, bounds.max.y, percentage.y) *
                                           evalResult.size;
                    break;
                case Axis.X:
                    axisRotation = Quaternion.Euler(0f, -90f, 0f);
                    evalResult.position +=
                        right * Mathf.Lerp(bounds.max.z, bounds.min.z, percentage.z) * evalResult.size;
                    evalResult.position += evalResult.up * Mathf.Lerp(bounds.min.y, bounds.max.y, percentage.y) *
                                           evalResult.size;
                    break;
                case Axis.Y:
                    axisRotation = Quaternion.Euler(90f, 0f, 0f);
                    evalResult.position +=
                        right * Mathf.Lerp(bounds.min.x, bounds.max.x, percentage.x) * evalResult.size;
                    evalResult.position += evalResult.up * Mathf.Lerp(bounds.min.z, bounds.max.z, percentage.z) *
                                           evalResult.size;
                    break;
            }

            bendRotation = evalResult.rotation * axisRotation;
            normalMatrix = Matrix4x4.TRS(evalResult.position, bendRotation, Vector3.one * evalResult.size).inverse
                .transpose;
        }

        Vector3 GetPercentage(Vector3 point)
        {
            point.x = Mathf.InverseLerp(bounds.min.x, bounds.max.x, point.x);
            point.y = Mathf.InverseLerp(bounds.min.y, bounds.max.y, point.y);
            point.z = Mathf.InverseLerp(bounds.min.z, bounds.max.z, point.z);
            return point;
        }

        protected override void Build()
        {
            base.Build();
            if (_bend) Bend();
        }

        void Bend()
        {
            if (sampleCount <= 1) return;
            if (bendProperties.Length == 0) return;
            for (var i = 0; i < bendProperties.Length; i++)
            {
                BendObject(bendProperties[i]);
            }
        }

        public void BendObject(BendProperty p)
        {
            if (!p.enabled) return;
            if (p.isParent && _parentIsTheSpline) return;
            GetevalResult(p.positionPercent);
            p.transform.position = evalResult.position;
            if (p.applyRotation)
            {
                //p.transform.rotation = evalResult.rotation * axisRotation * p.originalRotation;
                p.transform.rotation = bendRotation * (Quaternion.Inverse(p.parentRotation) * p.originalRotation);
            }
            else
            {
                p.transform.rotation = p.originalRotation;
            }
            if (p.applyScale) p.transform.scale = p.originalScale * evalResult.size;

            var toLocalMatrix = Matrix4x4.TRS(p.transform.position, p.transform.rotation, p.transform.scale).inverse;
            if (p.editMesh != null)
            {
                BendMesh(p.vertexPercents, p.normals, p.editMesh, toLocalMatrix);
                p.editMesh.hasUpdate = true;
            }

            if (p._editColliderMesh != null)
            {
                BendMesh(p.colliderVertexPercents, p.colliderNormals, p.editColliderMesh, toLocalMatrix);
                p.editColliderMesh.hasUpdate = true;
            }

            if (p.originalSpline != null && !p.isParent)
            {
                for (var n = 0; n < p.splinePointPercents.Length; n++)
                {
                    var point = p.originalSpline.points[n];
                    GetevalResult(p.splinePointPercents[n]);
                    point.position = evalResult.position;
                    GetevalResult(p.primaryTangentPercents[n]);
                    point.tangent = evalResult.position;
                    GetevalResult(p.secondaryTangentPercents[n]);
                    point.tangent2 = evalResult.position;
                    switch (axis)
                    {
                        case Axis.X:
                            point.normal = Quaternion.LookRotation(evalResult.forward, evalResult.up) *
                                           Quaternion.FromToRotation(Vector3.up, evalResult.up) * point.normal; break;
                        case Axis.Y:
                            point.normal = Quaternion.LookRotation(evalResult.forward, evalResult.up) *
                                           Quaternion.FromToRotation(Vector3.up, evalResult.up) * point.normal; break;
                        case Axis.Z:
                            point.normal = Quaternion.LookRotation(evalResult.forward, evalResult.up) * point.normal;
                            break;
                    }
                    p.destinationSpline.points[n] = point;
                }
            }
        }

        void BendMesh(Vector3[] vertexPercents, Vector3[] originalNormals, TS_Mesh mesh, Matrix4x4 worldToLocalMatrix)
        {
            if (mesh.vertexCount != vertexPercents.Length)
            {
                Debug.LogError("Vertex count mismatch");
                return;
            }
            for (var i = 0; i < mesh.vertexCount; i++)
            {
                var percent = vertexPercents[i];
                if (axis == Axis.Y) percent.z = 1f - percent.z;
                GetevalResult(percent);
                mesh.vertices[i] = worldToLocalMatrix.MultiplyPoint3x4(evalResult.position);
                mesh.normals[i] = worldToLocalMatrix.MultiplyVector(normalMatrix.MultiplyVector(originalNormals[i]));
            }
        }

        protected override void PostBuild()
        {
            base.PostBuild();
            if (!_bend) return;
            for (var i = 0; i < bendProperties.Length; i++)
            {
                bendProperties[i].Apply(i > 0 || trs != spline.transform);
                bendProperties[i].Update();
            }
        }

        protected override void LateRun()
        {
            base.LateRun();
            for (var i = 0; i < bendProperties.Length; i++)
            {
                bendProperties[i].Update();
            }
        }


        [Serializable]
        public class BendProperty
        {
            public bool enabled = true;
            public TS_Transform transform;
            public bool applyRotation = true;
            public bool applyScale = true;
            public bool generateLightmapUVs;

            [SerializeField]
            [HideInInspector]
            bool _bendMesh = true;

            [SerializeField]
            [HideInInspector]
            bool _bendSpline = true;

            [SerializeField]
            [HideInInspector]
            bool _bendCollider = true;

            public float colliderUpdateRate = 0.2f;

            public Vector3 originalPosition = Vector3.zero;
            public Vector3 originalScale = Vector3.one;
            public Quaternion originalRotation = Quaternion.identity;
            public Quaternion parentRotation = Quaternion.identity;
            public Vector3 positionPercent;

            public Vector3[] vertexPercents = new Vector3[0];
            public Vector3[] normals = new Vector3[0];
            public Vector3[] colliderVertexPercents = new Vector3[0];
            public Vector3[] colliderNormals = new Vector3[0];

            [SerializeField]
            [HideInInspector]
            Mesh originalMesh;

            [SerializeField]
            [HideInInspector]
            Mesh originalColliderMesh;

            [SerializeField]
            [HideInInspector]
            Mesh destinationMesh;

            [SerializeField]
            [HideInInspector]
            Mesh destinationColliderMesh;

            public Spline destinationSpline;

            public MeshFilter filter;
            public MeshCollider collider;
            public SplineComputer splineComputer;

            public Vector3[] splinePointPercents = new Vector3[0];
            public Vector3[] primaryTangentPercents = new Vector3[0];
            public Vector3[] secondaryTangentPercents = new Vector3[0];

            [SerializeField]
            [HideInInspector]
            bool parent;

            public TS_Mesh _editColliderMesh;

            public TS_Mesh _editMesh;

            Spline _originalSpline;

            float colliderUpdateDue;
            bool updateCollider;


            public BendProperty(Transform t, bool parent = false)
            {
                this.parent = parent;
                transform = new TS_Transform(t);
                originalPosition = t.localPosition;
                originalScale = t.localScale;
                originalRotation = t.localRotation;
                parentRotation = t.transform.rotation;
                if (t.transform.parent != null) parentRotation = t.transform.parent.rotation;
                filter = t.GetComponent<MeshFilter>();
                collider = t.GetComponent<MeshCollider>();
                if (filter != null && filter.sharedMesh != null)
                {
                    originalMesh = filter.sharedMesh;
                    normals = originalMesh.normals;
                    for (var i = 0; i < normals.Length; i++)
                        normals[i] = transform.transform.TransformDirection(normals[i]).normalized;
                }

                if (collider != null && collider.sharedMesh != null)
                {
                    originalColliderMesh = collider.sharedMesh;
                    colliderNormals = originalColliderMesh.normals;
                    for (var i = 0; i < colliderNormals.Length; i++)
                        colliderNormals[i] = transform.transform.TransformDirection(colliderNormals[i]);
                }
                if (!parent) splineComputer = t.GetComponent<SplineComputer>();
                if (splineComputer != null)
                {
                    if (splineComputer.isClosed) originalSpline.Close();
                    destinationSpline = new Spline(originalSpline.type);
                    destinationSpline.points = new SplinePoint[originalSpline.points.Length];
                    destinationSpline.points = splineComputer.GetPoints();
                    if (splineComputer.isClosed) destinationSpline.Close();
                }
            }

            public bool isValid
            {
                get { return transform != null && transform.transform != null; }
            }

            public bool bendMesh
            {
                get { return _bendMesh; }
                set
                {
                    if (value != _bendMesh)
                    {
                        _bendMesh = value;
                        if (value)
                        {
                            if (filter != null && filter.sharedMesh != null)
                            {
                                normals = originalMesh.normals;
                                for (var i = 0; i < normals.Length; i++)
                                    normals[i] = transform.transform.TransformDirection(normals[i]);
                            }
                        }
                        else
                        {
                            RevertMesh();
                        }
                    }
                }
            }

            public bool bendCollider
            {
                get { return _bendCollider; }
                set
                {
                    if (value != _bendCollider)
                    {
                        _bendCollider = value;
                        if (value)
                        {
                            if (collider != null && collider.sharedMesh != null && collider.sharedMesh != originalMesh)
                                colliderNormals = originalColliderMesh.normals;
                        }
                        else
                        {
                            RevertCollider();
                        }
                    }
                }
            }

            public bool bendSpline
            {
                get { return _bendSpline; }
                set
                {
                    _bendSpline = value;
                    if (value)
                    {

                    }
                }
            }

            public TS_Mesh editMesh
            {
                get
                {
                    if (!bendMesh || originalMesh == null) _editMesh = null;
                    else if (_editMesh == null && originalMesh != null) _editMesh = new TS_Mesh(originalMesh);
                    return _editMesh;
                }
            }

            public TS_Mesh editColliderMesh
            {
                get
                {
                    if (!bendCollider || originalColliderMesh == null) _editColliderMesh = null;
                    else if (_editColliderMesh == null && originalColliderMesh != null &&
                             originalColliderMesh != originalMesh)
                        _editColliderMesh = new TS_Mesh(originalColliderMesh);
                    return _editColliderMesh;
                }
            }

            public Spline originalSpline
            {
                get
                {
                    if (!bendSpline || splineComputer == null)
                    {
                        _originalSpline = null;
                    }
                    else if (_originalSpline == null && splineComputer != null)
                    {
                        _originalSpline = new Spline(splineComputer.type);
                        _originalSpline.points = splineComputer.GetPoints();
                    }
                    return _originalSpline;
                }
            }

            public bool isParent
            {
                get { return parent; }
            }

            public void Revert()
            {
                if (!isValid) return;
                RevertTransform();
                RevertCollider();
                RevertMesh();
                if (splineComputer != null) splineComputer.SetPoints(_originalSpline.points);
            }

            void RevertMesh()
            {
                if (filter != null) filter.sharedMesh = originalMesh;
                destinationMesh = null;
            }

            void RevertTransform()
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    transform.transform.localPosition = originalPosition;
                    transform.transform.localRotation = originalRotation;
                }
                else
                {
                    transform.localPosition = originalPosition;
                    transform.localRotation = originalRotation;
                    transform.Update();
                }
#else
                transform.localPosition = originalPosition;
                transform.localRotation = originalRotation;
                transform.Update();
#endif
                transform.scale = originalScale;
                transform.Update();
            }

            void RevertCollider()
            {
                if (collider != null) collider.sharedMesh = originalColliderMesh;
                destinationColliderMesh = null;
            }

            public void Apply(bool applyTransform)
            {
                if (!enabled) return;
                if (!isValid) return;
                if (applyTransform) transform.Update();
                if (editMesh != null && editMesh.hasUpdate) ApplyMesh();
                if (bendCollider && collider != null)
                {
                    if (!updateCollider)
                    {
                        if (editColliderMesh == null && editMesh != null || editColliderMesh != null)
                        {
                            updateCollider = true;
                            if (Application.isPlaying) colliderUpdateDue = Time.time + colliderUpdateRate;
                        }
                    }
                }
                if (splineComputer != null) ApplySpline();
            }

            public void Update()
            {
                if (Time.time >= colliderUpdateDue && updateCollider)
                {
                    updateCollider = false;
                    ApplyCollider();
                }
            }

            void ApplyMesh()
            {
                if (filter == null) return;
                MeshUtility.CalculateTangents(editMesh);
                if (destinationMesh == null)
                {
                    destinationMesh = new Mesh();
                    destinationMesh.name = originalMesh.name;
                }

                editMesh.WriteMesh(ref destinationMesh);
                destinationMesh.RecalculateBounds();
                filter.sharedMesh = destinationMesh;
            }

            void ApplyCollider()
            {
                if (collider == null) return;
                if (originalColliderMesh == originalMesh)
                {
                    collider.sharedMesh =
                        filter.sharedMesh; //if the collider has the same mesh as the filter - just copy it
                }
                else
                {
                    MeshUtility.CalculateTangents(editColliderMesh);
                    if (destinationColliderMesh == null)
                    {
                        destinationColliderMesh = new Mesh();
                        destinationColliderMesh.name = originalColliderMesh.name;
                    }
                    editColliderMesh.WriteMesh(ref destinationColliderMesh);
                    destinationColliderMesh.RecalculateBounds();
                    collider.sharedMesh = destinationColliderMesh;
                }
            }

            void ApplySpline()
            {
                if (destinationSpline == null) return;
                splineComputer.SetPoints(destinationSpline.points);
            }
        }
    }
}
