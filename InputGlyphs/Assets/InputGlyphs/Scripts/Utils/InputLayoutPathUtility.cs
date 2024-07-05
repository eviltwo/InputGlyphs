#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Text;
using UnityEngine.InputSystem;

namespace InputGlyphs.Utils
{
    public static class InputLayoutPathUtility
    {
        private static StringBuilder _stringBuilder = new StringBuilder();

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

        public static bool TryGetActionBindingPath(InputAction action, string controlScheme, List<string> results)
        {
            results.Clear();
            if (action == null)
            {
                return false;
            }
            // TODO: Get multiple bindings
            var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(controlScheme));
            if (bindingIndex < 0)
            {
                return false;
            }
            results.Add(action.bindings[bindingIndex].effectivePath);
            return true;
        }
    }
}
#endif
