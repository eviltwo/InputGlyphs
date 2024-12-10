#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs
{
    /// <summary>
    /// Manages GlyphLoaders and load Glyph images from registered GlyphLoaders.
    /// Register GlyphLoaders when you start the game.
    /// </summary>
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

        /// <summary>
        /// Load glyphs for the given device and layout path and writes it to texture.
        /// </summary>
        /// <param name="texture">Texture onto which glyphs are written.</param>
        /// <param name="activeDevices">Active devices</param>
        /// <param name="inputLayoutPath">example: &lt;gamepad&gt;/dpad/left</param>
        /// <returns>Return true if the load was success.</returns>
        public static bool LoadGlyph(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath)
        {
            for (var i = 0; i < _loaders.Count; i++)
            {
                if (_loaders[i].LoadGlyph(texture, activeDevices, inputLayoutPath))
                {
                    return true;
                }
            }

            var parentPath = InputLayoutPathUtility.GetParent(inputLayoutPath);
            if (string.IsNullOrEmpty(parentPath))
            {
                return false;
            }
            else
            {
                return LoadGlyph(texture, activeDevices, parentPath);
            }
        }
    }
}
#endif
