using System.Collections.Generic;
using UnityEngine;

namespace Dreamteck.Splines
{
    [ExecuteInEditMode]
    [AddComponentMenu("Dreamteck/Splines/Users/Particle Controller")]
    public class ParticleController : SplineUser
    {
        public enum EmitPoint
        {
            Beginning,
            Ending,
            Random,
            Ordered
        }

        public enum MotionType
        {
            None,
            UseParticleSystem,
            FollowForward,
            FollowBackward,
            ByNormal,
            ByNormalRandomized
        }

        public enum Wrap
        {
            Default,
            Loop
        }

        [SerializeField]
        [HideInInspector]
        ParticleSystem _particleSystem;

        [HideInInspector]
        public bool pauseWhenNotVisible;

        [HideInInspector]
        public Vector2 offset = Vector2.zero;

        [HideInInspector]
        public bool volumetric;

        [HideInInspector]
        public bool emitFromShell;

        [HideInInspector]
        public bool apply3DRotation;

        [HideInInspector]
        public Vector2 scale = Vector2.one;

        [HideInInspector]
        public EmitPoint emitPoint = EmitPoint.Beginning;

        [HideInInspector]
        public MotionType motionType = MotionType.UseParticleSystem;

        [HideInInspector]
        public Wrap wrapMode = Wrap.Default;

        [HideInInspector]
        public float minCycles = 1f;

        [HideInInspector]
        public float maxCycles = 1f;

        int _birthIndex;
        Particle[] _controllers = new Particle[0];
        List<Vector4> _customParticleData = new();
        int _particleCount;

        ParticleSystem.Particle[] _particles = new ParticleSystem.Particle[0];

        ParticleSystemRenderer _renderer;

        public ParticleSystem particleSystemComponent
        {
            get { return _particleSystem; }
            set
            {
                _particleSystem = value;
                _renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
            }
        }

        protected override void Reset()
        {
            base.Reset();
            updateMethod = UpdateMethod.LateUpdate;
            if (_particleSystem == null) _particleSystem = GetComponent<ParticleSystem>();
        }

        protected override void LateRun()
        {
            if (_particleSystem == null) return;
            if (pauseWhenNotVisible)
            {
                if (_renderer == null)
                {
                    _renderer = _particleSystem.GetComponent<ParticleSystemRenderer>();
                }
                if (!_renderer.isVisible) return;
            }

            var maxParticles = _particleSystem.main.maxParticles;
            if (_particles.Length != maxParticles)
            {
                _particles = new ParticleSystem.Particle[maxParticles];
                _customParticleData = new List<Vector4>(maxParticles);
                var newControllers = new Particle[maxParticles];
                for (var i = 0; i < newControllers.Length; i++)
                {
                    if (i >= _controllers.Length) break;
                    newControllers[i] = _controllers[i];
                }
                _controllers = newControllers;
            }
            _particleCount = _particleSystem.GetParticles(_particles);
            _particleSystem.GetCustomParticleData(_customParticleData, ParticleSystemCustomData.Custom1);

            var isLocal = _particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local;

            var particleSystemTransform = _particleSystem.transform;

            for (var i = 0; i < _particleCount; i++)
            {
                if (_controllers[i] == null)
                {
                    _controllers[i] = new Particle();
                }
                if (isLocal)
                {
                    TransformParticle(ref _particles[i], particleSystemTransform);
                }
                if (_customParticleData[i].w < 1f)
                {
                    OnParticleBorn(i);
                }
                HandleParticle(i);
                if (isLocal)
                {
                    InverseTransformParticle(ref _particles[i], particleSystemTransform);
                }
            }

            _particleSystem.SetCustomParticleData(_customParticleData, ParticleSystemCustomData.Custom1);
            _particleSystem.SetParticles(_particles, _particleCount);
        }

        void TransformParticle(ref ParticleSystem.Particle particle, Transform trs)
        {
            particle.position = trs.TransformPoint(particle.position);
            if (apply3DRotation)
            {

            }
            particle.velocity = trs.TransformDirection(particle.velocity);
        }

        void InverseTransformParticle(ref ParticleSystem.Particle particle, Transform trs)
        {
            particle.position = trs.InverseTransformPoint(particle.position);
            particle.velocity = trs.InverseTransformDirection(particle.velocity);
        }

        void HandleParticle(int index)
        {
            var lifePercent = _particles[index].remainingLifetime / _particles[index].startLifetime;
            if (motionType == MotionType.FollowBackward || motionType == MotionType.FollowForward ||
                motionType == MotionType.None)
            {
                Evaluate(_controllers[index].GetSplinePercent(wrapMode, _particles[index], motionType), ref evalResult);
                var resultRight = evalResult.right;
                _particles[index].position = evalResult.position;
                if (apply3DRotation)
                {
                    _particles[index].rotation3D = evalResult.rotation.eulerAngles;
                }
                var finalOffset = offset;
                if (volumetric)
                {
                    if (motionType != MotionType.None)
                    {
                        finalOffset += Vector2.Lerp(_controllers[index].startOffset,
                        _controllers[index].endOffset,
                        1f - lifePercent);
                        finalOffset.x *= scale.x;
                        finalOffset.y *= scale.y;
                    }
                    else
                    {
                        finalOffset += _controllers[index].startOffset;
                    }
                }
                _particles[index].position += resultRight * (finalOffset.x * evalResult.size) +
                                              evalResult.up * (finalOffset.y * evalResult.size);
                _particles[index].velocity = evalResult.forward;
                _particles[index].startColor = _controllers[index].startColor * evalResult.color;
            }
        }

        void OnParticleBorn(int index)
        {
            var custom = _customParticleData[index];
            custom.w = 1;
            _customParticleData[index] = custom;
            var percent = 0.0;
            var emissionRate = Mathf.Lerp(_particleSystem.emission.rateOverTime.constantMin,
            _particleSystem.emission.rateOverTime.constantMax,
            0.5f);
            var expectedParticleCount = emissionRate * _particleSystem.main.startLifetime.constantMax;
            _birthIndex++;
            if (_birthIndex > expectedParticleCount)
            {
                _birthIndex = 0;
            }

            switch (emitPoint)
            {
                case EmitPoint.Beginning: percent = 0f; break;
                case EmitPoint.Ending: percent = 1f; break;
                case EmitPoint.Random: percent = Random.Range(0f, 1f); break;
                case EmitPoint.Ordered:
                    percent = expectedParticleCount > 0 ? _birthIndex / expectedParticleCount : 0f; break;
            }
            Evaluate(percent, ref evalResult);
            _controllers[index].startColor = _particles[index].startColor;
            _controllers[index].startPercent = percent;

            _controllers[index].cycleSpeed = Random.Range(minCycles, maxCycles);
            var circle = Vector2.zero;
            if (volumetric)
            {
                if (emitFromShell)
                    circle = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward) * Vector2.right;
                else circle = Random.insideUnitCircle;
            }
            _controllers[index].startOffset = circle * 0.5f;
            _controllers[index].endOffset = Random.insideUnitCircle * 0.5f;


            var right = Vector3.Cross(evalResult.forward, evalResult.up);
            _particles[index].position = evalResult.position +
                                         right * _controllers[index].startOffset.x * evalResult.size * scale.x +
                                         evalResult.up * _controllers[index].startOffset.y * evalResult.size * scale.y;

            var forceX = _particleSystem.forceOverLifetime.x.constantMax;
            var forceY = _particleSystem.forceOverLifetime.y.constantMax;
            var forceZ = _particleSystem.forceOverLifetime.z.constantMax;
            if (_particleSystem.forceOverLifetime.randomized)
            {
                forceX = Random.Range(_particleSystem.forceOverLifetime.x.constantMin,
                _particleSystem.forceOverLifetime.x.constantMax);
                forceY = Random.Range(_particleSystem.forceOverLifetime.y.constantMin,
                _particleSystem.forceOverLifetime.y.constantMax);
                forceZ = Random.Range(_particleSystem.forceOverLifetime.z.constantMin,
                _particleSystem.forceOverLifetime.z.constantMax);
            }

            var time = _particles[index].startLifetime - _particles[index].remainingLifetime;
            var forceDistance = new Vector3(forceX, forceY, forceZ) * 0.5f * (time * time);

            var startSpeed = _particleSystem.main.startSpeed.constantMax;

            if (motionType == MotionType.ByNormal)
            {
                _particles[index].position += evalResult.up * startSpeed *
                                              (_particles[index].startLifetime - _particles[index].remainingLifetime);
                _particles[index].position += forceDistance;
                _particles[index].velocity = evalResult.up * startSpeed + new Vector3(forceX, forceY, forceZ) * time;
            }
            else if (motionType == MotionType.ByNormalRandomized)
            {
                var normal = Quaternion.AngleAxis(Random.Range(0f, 360f), evalResult.forward) * evalResult.up;
                _particles[index].position += normal * startSpeed *
                                              (_particles[index].startLifetime - _particles[index].remainingLifetime);
                _particles[index].position += forceDistance;
                _particles[index].velocity = normal * startSpeed + new Vector3(forceX, forceY, forceZ) * time;
            }
            HandleParticle(index);
        }

        public class Particle
        {
            internal float cycleSpeed;
            internal Vector2 endOffset = Vector2.zero;
            internal Color startColor = Color.white;
            internal Vector2 startOffset = Vector2.zero;
            internal double startPercent;

            internal double GetSplinePercent(Wrap wrap, ParticleSystem.Particle particle, MotionType motionType)
            {
                var lifePercent = particle.remainingLifetime / particle.startLifetime;
                if (motionType == MotionType.FollowBackward)
                {
                    lifePercent = 1f - lifePercent;
                }
                switch (wrap)
                {
                    case Wrap.Default: return DMath.Clamp01(startPercent + (1f - lifePercent) * cycleSpeed);
                    case Wrap.Loop:
                        var loopPoint = startPercent + (1.0 - lifePercent) * cycleSpeed;
                        if (loopPoint > 1.0) loopPoint -= Mathf.FloorToInt((float)loopPoint);
                        return loopPoint;
                }
                return 0.0;
            }
        }
    }
}
