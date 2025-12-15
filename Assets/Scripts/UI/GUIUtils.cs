using UnityEngine;

namespace UI
{
    public static class GUIUtils
    {
        public static void PlaceAt(RectTransform element, Vector2 screenPoint, Canvas canvas,
            RectTransform parent, bool clampToPanel = false)
        {
            // Canvas.ForceUpdateCanvases();
            // LayoutRebuilder.ForceRebuildLayoutImmediate(parent);
            // LayoutRebuilder.ForceRebuildLayoutImmediate(element);

            var localPoint = screenPoint ;// / canvas.transform.lossyScale;

            if (clampToPanel)
            {
                var panelSize = parent.rect.size;
                var popupSize = element.rect.size;

                var min = -panelSize * parent.pivot + Vector2.Scale(popupSize, element.pivot);
                var max = panelSize * (Vector2.one - parent.pivot) -
                          Vector2.Scale(popupSize, Vector2.one - element.pivot);

                localPoint.x = Mathf.Clamp(localPoint.x, min.x, max.x);
                localPoint.y = Mathf.Clamp(localPoint.y, min.y, max.y);
            }

            // apply position: anchoredPosition if fixed anchors, otherwise localPosition for stretched anchors
            if (element.anchorMin == element.anchorMax)
            {
                element.anchoredPosition = localPoint;
            }
            else
            {
                element.localPosition = new Vector3(localPoint.x, localPoint.y, element.localPosition.z);
            }
            element.SetParent(parent, true);
        }
    }
}
