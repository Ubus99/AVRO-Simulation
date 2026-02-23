using UnityEngine.InputSystem;

namespace Utils
{
    public static class InputUtils
    {
        public static int DigitPressed()
        {
            var keyboard = Keyboard.current;
            if (keyboard.digit1Key.wasPressedThisFrame)
            {
                return 0;
            }
            if (keyboard.digit2Key.wasPressedThisFrame)
            {
                return 1;
            }
            if (keyboard.digit3Key.wasPressedThisFrame)
            {
                return 2;
            }
            if (keyboard.digit4Key.wasPressedThisFrame)
            {
                return 3;
            }
            if (keyboard.digit5Key.wasPressedThisFrame)
            {
                return 4;
            }
            if (keyboard.digit6Key.wasPressedThisFrame)
            {
                return 5;
            }
            if (keyboard.digit7Key.wasPressedThisFrame)
            {
                return 6;
            }
            if (keyboard.digit8Key.wasPressedThisFrame)
            {
                return 7;
            }
            if (keyboard.digit9Key.wasPressedThisFrame)
            {
                return 8;
            }
            if (keyboard.digit0Key.wasPressedThisFrame)
            {
                return 9;
            }
            return -1;
        }

        public static void SwitchToScheme(string scheme)
        {
            var asset = InputSystem.actions;
            if (asset == null) return;

            var uiMap = asset.FindActionMap("UI");
            if (uiMap == null) return;

            if (string.IsNullOrEmpty(scheme))
            {
                uiMap.bindingMask = InputBinding.MaskByGroups(string.Empty); // matches no groups -> disables bindings
                return;
            }

            var found = asset.FindControlScheme(scheme);
            if (found == null || string.IsNullOrEmpty(found.Value.bindingGroup))
            {
                uiMap.bindingMask = InputBinding.MaskByGroups(string.Empty); // matches no groups -> disables bindings
                return;
            }

            uiMap.bindingMask = InputBinding.MaskByGroups(found.Value.bindingGroup);
        }
    }
}
