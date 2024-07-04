#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs
{
    public static class InputGlyphManager
    {
        private static List<IInputGlyphLoader> _loaders = new List<IInputGlyphLoader>();

        public static void RegisterLoader(IInputGlyphLoader loader)
        {
            if (loader != null)
            {
                _loaders.Add(loader);
            }
        }

        public static void UnregisterLoader(IInputGlyphLoader loader)
        {
            if (loader != null)
            {
                _loaders.Remove(loader);
            }
        }

        public static Texture2D GetGlyph(IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath)
        {
            for (var i = 0; i < _loaders.Count; i++)
            {
                var texture = _loaders[i].GetGlyph(activeDevices, inputLayoutPath);
                if (texture != null)
                {
                    return texture;
                }
            }
            return null;
        }
    }
}
#endif
