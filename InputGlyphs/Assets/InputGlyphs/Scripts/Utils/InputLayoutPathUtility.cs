#if ENABLE_INPUT_SYSTEM
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
    }
}
#endif
