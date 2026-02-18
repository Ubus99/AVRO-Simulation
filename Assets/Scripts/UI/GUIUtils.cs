using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
#endif

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

            var localPoint = screenPoint; // / canvas.transform.lossyScale;

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

        public static void ToggleHidden(List<VisualElement> elements, bool isHidden)
        {
            foreach (var ve in elements)
            {
                if (isHidden) ve.AddToClassList("hidden");
                else ve.RemoveFromClassList("hidden");
            }
        }

        public static void SwitchFocusTo(NavigationMoveEvent evt,
            (NavigationMoveEvent.Direction direction, VisualElement element) data)
        {
            if (evt.direction != data.direction)
                return;

            evt.StopPropagation();
            data.element.Focus();
        }

        static void UpdateImage(Image image, GeometryChangedEvent evt)
        {
            if (image.image is not Texture2D { width: > 0 } tex)
                return;

            var targetWidth = evt.newRect.width; // available width
            var scaledHeight = targetWidth * ((float)tex.height / tex.width);
            image.style.height = new StyleLength(new Length(scaledHeight, LengthUnit.Pixel));
        }

#if UNITY_EDITOR
        public static void AssignableImageSection(VisualElement root, SerializedProperty imageProperty)
        {
            var imageField = new ObjectField
            {
                objectType = typeof(Texture2D),
                allowSceneObjects = false,
                label = imageProperty.displayName
            };
            imageField.BindProperty(imageProperty);

            // draw map
            var image = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    maxHeight = 256,
                    flexShrink = 0
                },
                image = imageProperty.objectReferenceValue as Texture2D
            };

            imageField.RegisterValueChangedCallback(_ =>
                image.image = imageProperty.objectReferenceValue as Texture2D);

            root.RegisterCallback<GeometryChangedEvent, Image>((evt, img) =>
                UpdateImage(img, evt),
            image);

            root.Add(imageField);
            root.Add(image);
        }

        public static void ImageSection(VisualElement root, SerializedProperty imageProperty)
        {
            var routeImage = new Image
            {
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                    width = new StyleLength(Length.Percent(100)),
                    height = new StyleLength(StyleKeyword.Auto),
                    maxHeight = 128,
                    flexShrink = 0
                },
                image = imageProperty.objectReferenceValue as Texture2D
            };

            root.RegisterCallback<GeometryChangedEvent, Image>((evt, image) =>
                UpdateImage(image, evt),
            routeImage);

            root.Add(routeImage);
        }
#endif
    }
}
