using System.Collections.Generic;
using UI;
using UnityEngine;
using Utils.Types;

namespace Scenes.Scripts.UI
{
    public class ScreenManager : EditorBehavior
    {
        [SerializeField]
        public OverviewManager overviewManager;

        [SerializeField]
        public POVManager povManager;

        readonly List<ISubScreen> _subScreens = new();

        protected override void HandleIsDirty()
        {
            RefreshComponents();
            overviewManager.Show();
            overviewManager.onViewSelected.AddListener(car =>
            {
                ActivateScreen(povManager);
                povManager.LoadMission(car);
            });
            povManager.Hide();
        }

        protected override void RefreshComponents()
        {
            _subScreens.Clear();
            _subScreens.Add(overviewManager.GetComponent<ISubScreen>());
            _subScreens.Add(povManager.GetComponent<ISubScreen>());
        }

        public void ActivateScreen(ISubScreen canvas)
        {
            Debug.Log($"trying to activate screen {canvas}");
            foreach (var c in _subScreens)
            {
                if (c == canvas)
                {
                    c.Show();
                }
                else
                {
                    c.Hide();
                }
            }
        }

        public void ActivateScreen(Canvas canvas)
        {
            if (!canvas.TryGetComponent(out ISubScreen screen))
                return;
            ActivateScreen(screen);
        }
    }
}
