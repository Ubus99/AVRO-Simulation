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

        readonly List<ISubScreen> subScreens = new();

        protected override void HandleIsDirty()
        {
            RefreshComponents();
            overviewManager.Show();
            overviewManager.onViewSelected.AddListener(car =>
            {
                ActivateScreen(povManager);
                povManager.LoadData(car);
            });
            povManager.Hide();
        }

        protected override void RefreshComponents()
        {
            subScreens.Clear();
            subScreens.Add(overviewManager.GetComponent<ISubScreen>());
            subScreens.Add(povManager.GetComponent<ISubScreen>());
        }

        public void ActivateScreen(ISubScreen canvas)
        {
            foreach (var c in subScreens)
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
