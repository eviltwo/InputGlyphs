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
        /// Returns a local path for the given input layout path.
        /// </summary>
        /// <remarks>
        /// &lt;gamepad&gt;/dpad/left -> dpad/left
        /// </remarks>
        public static string GetLocalPath(string inputLayoutPath)
        {
            if (string.IsNullOrEmpty(inputLayoutPath))
            {
                return null;
            }
            var pathComponents = InputControlPath.Parse(inputLayoutPath);
            var enumerator = pathComponents.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return null;
            }

            _stringBuilder.Clear();
            while (enumerator.MoveNext())
            {
                if (_stringBuilder.Length > 0)
                {
                    _stringBuilder.Append(InputControlPath.Separator);
                }
                _stringBuilder.Append(enumerator.Current.name);
            }
            return _stringBuilder.ToString();
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
    }
}
#endif
