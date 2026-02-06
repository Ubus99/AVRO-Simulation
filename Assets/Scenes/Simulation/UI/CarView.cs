using UnityEngine;
using UnityEngine.UIElements;


namespace Scenes.Simulation.UI
{
    public class CarView : MonoBehaviour
    {
        [SerializeField]
        VisualTreeAsset actionItemTemplate;

        void OnEnable()
        {
            var uiDocument = GetComponent<UIDocument>();

            var actionListController = new ListController();
            actionListController.InitializeList(uiDocument.rootVisualElement, actionItemTemplate);
        }

        public void LoadMission()
        {
            
        }
    }
}
