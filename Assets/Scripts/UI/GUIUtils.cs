using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public static class GUIUtils
    {
        public static void PlaceIn(RectTransform element, Vector2 position, RectTransform container,
            bool clampToPanel = true)
        {
            // parent already set by Instantiate(..., videoFeed.transform, false) — ensure layout is up-to-date
            LayoutRebuilder.ForceRebuildLayoutImmediate(element);

            // convert screen -> panel-local (returns point relative to panel.pivot)
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, position, null, out var localPoint);
            
            if (clampToPanel)
            {
                var panelSize = container.rect.size;
                var popupSize = element.rect.size;

                var min = -panelSize * container.pivot + Vector2.Scale(popupSize, element.pivot);
                var max = panelSize * (Vector2.one - container.pivot) -
                          Vector2.Scale(popupSize, Vector2.one - element.pivot);

                position.x = Mathf.Clamp(position.x, min.x, max.x);
                position.y = Mathf.Clamp(position.y, min.y, max.y);
            }

            // apply position: anchoredPosition if fixed anchors, otherwise localPosition for stretched anchors
            if (element.anchorMin == element.anchorMax)
            {
                element.anchoredPosition = position;
            }
            else
            {
                element.localPosition = new Vector3(position.x, position.y, element.localPosition.z);
            }
            
            element.SetParent(container, true);
        }
    }
}
