#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace InputGlyphs.Utils
{
    public static class InputLayoutPathUtility
    {
        private static StringBuilder _stringBuilder = new StringBuilder();
        private static List<int> _bindingIndexBuffer = new List<int>();

        /// <summary>
        /// Remove the root (probably the device name) from the path.
        /// </summary>
        /// <remarks>
        /// Example: &lt;gamepad&gt;/dpad/left -> dpad/left
        /// </remarks>
        public static string RemoveRoot(string inputControlPath)
        {
            if (string.IsNullOrEmpty(inputControlPath))
            {
                return string.Empty;
            }
            var startIndex = inputControlPath[0] == InputControlPath.Separator ? 1 : 0;
            var separationIndex = inputControlPath.IndexOf(InputControlPath.Separator, startIndex);
            if (separationIndex == -1)
            {
                return inputControlPath;
            }

            if (separationIndex == inputControlPath.Length)
            {
                return string.Empty;
            }

            return inputControlPath.Substring(separationIndex + 1);
        }

        /// <summary>
        /// Get the parent path of the input layout path.
        /// </summary>
        /// <remarks>
        /// Example: /leftStick/x -> /leftStick
        /// </remarks>
        public static string GetParent(string inputLayoutPath)
        {
            if (string.IsNullOrEmpty(inputLayoutPath))
            {
                return string.Empty;
            }
            var lastSeparatorIndex = inputLayoutPath.LastIndexOf(InputControlPath.Separator);
            if (lastSeparatorIndex == -1)
            {
                return string.Empty;
            }
            return inputLayoutPath.Substring(0, lastSeparatorIndex);
        }

        /// <summary>
        /// Searches for bindings within actions that match the control scheme and returns the effective paths.
        /// </summary>
        /// <param name="action">Target action</param>
        /// <param name="controlScheme">Control scheme for masks</param>
        /// <param name="results">Effective paths of detected bindings</param>
        public static bool TryGetActionBindingPath(InputAction action, string controlScheme, List<string> results)
        {
            results.Clear();
            if (action == null)
            {
                return false;
            }
            _bindingIndexBuffer.Clear();
            action.GetBindingIndexes(InputBinding.MaskByGroup(controlScheme), _bindingIndexBuffer);
            for (int i = 0; i < _bindingIndexBuffer.Count; i++)
            {
                var bindingIndex = _bindingIndexBuffer[i];
                if (bindingIndex < 0)
                {
                    continue;
                }
                results.Add(action.bindings[bindingIndex].effectivePath);
            }
            return results.Count > 0;
        }

        public static bool HasPathComponent(string path)
        {
            return path.IndexOf('<') >= 0
                || path.IndexOf('{') >= 0
                || path.IndexOf('(') >= 0;
        }
    }
}
#endif
