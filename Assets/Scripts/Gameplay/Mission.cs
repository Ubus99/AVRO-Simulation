using System;
using System.Collections.Generic;
using car_logic;
using Scenes.Scripts.Missions;
using UI;
using UnityEngine;
using Utils.Lucide;

namespace Gameplay
{
    [Serializable]
    public abstract class Mission : MonoBehaviour
    {
        [Header("Prefabs")]
        public ADSV_AI carPrefab;

        [Header("Key Points")]
        public Transform startPoint;

        public Transform endPoint;
        public AlternativeRouteHelper alternativeRoutes;

        [SerializeField]
        List<HistoryListElement> history;

        protected bool Active;
        public ADSV_AI carInstance { protected set; get; }
        public bool completed { get; protected set; }

        void Awake()
        {
            OnLoad();
        }

        public event EventHandler<Mission> OnCompleted;
        public event EventHandler OnActivated;
        public event EventHandler OnDeactivated;

        protected void OnLoad()
        {
            Deactivate();
        }

        public void Activate()
        {
            SpawnCar(true);
            Setup();
            foreach (var sc in alternativeRoutes.routes)
                sc.gameObject.SetActive(true);
            OnActivated?.Invoke(this, EventArgs.Empty);
            Active = true;
        }

        void SpawnCar(bool startErrored)
        {
            carInstance = Instantiate(carPrefab, startPoint.position + Vector3.up, startPoint.rotation);
            carInstance.currentMission = this;
            if (startErrored)
                carInstance.state = States.ErrorDetected;
        }

        protected abstract void Setup();

        public void Deactivate()
        {
            CleanUp();
            if (carInstance) Destroy(carInstance.gameObject);
            foreach (var sc in alternativeRoutes.routes)
                sc.gameObject.SetActive(false);
            OnDeactivated?.Invoke(this, EventArgs.Empty);
            Active = false;
        }

        protected abstract void CleanUp();

        public void SelectRoute(AlternativeRoute route)
        {
            alternativeRoutes.SelectRoute(route);
        }

        public abstract IEnumerable<ObstacleActionListElement> GetObstacleData();

        public IEnumerable<HistoryListElement> GetHistory()
        {
            return history;
        }

        [Serializable]
        public class HistoryListElement : IListElement
        {
            public enum Type
            {
                Start,
                Waypoint,
                Goal,
                Error
            }

            [SerializeField]
            Type type;

            [SerializeField]
            string timestamp;

            [SerializeField]
            string description;

            IconAtlas _iconAtlas = IconAtlas.instance;

            public bool selectable
            {
                get
                {
                    return type switch
                    {
                        Type.Start or Type.Waypoint or Type.Goal => false,
                        Type.Error => true,
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public GlyphData leftIcon
            {
                get
                {
                    return type switch
                    {
                        Type.Start => _iconAtlas.glyphs["map-pin"],
                        Type.Waypoint => _iconAtlas.glyphs["route"],
                        Type.Goal => _iconAtlas.glyphs["map-pin"],
                        Type.Error => _iconAtlas.glyphs["triangle-alert"],
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            public GlyphData rightIcon
            {
                get { return type == Type.Error ? _iconAtlas.glyphs["pencil"] : null; }
            }

            public string titleText
            {
                get { return $"[{timestamp}] {type.ToString()}"; }
            }

            public string labelText
            {
                get { return description; }
            }
        }

        [Serializable]
        public class ObstacleActionListElement : IListElement
        {
            AdsObstacle _obstacle;
            AdsObstacle.State _state;

            public ObstacleActionListElement(AdsObstacle.State state, AdsObstacle obstacle)
            {
                _obstacle = obstacle;
                _state = state;
            }

            public bool selectable
            {
                get { return true; }
            }

            public GlyphData leftIcon
            {
                get { return null; }
            }

            public GlyphData rightIcon
            {
                get { return null; }
            }

            public string titleText
            {
                get { return _state.ToString(); }
            }

            public string labelText
            {
                get { return ""; }
            }

            public void Apply()
            {
                _obstacle.SetState(_state);
            }
        }
    }
}
