#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs
{
    public interface IInputGlyphLoader
    {
        /// <summary>
        /// Load glyphs for the given device and layout path and writes it to texture.
        /// </summary>
        /// <param name="texture">Texture onto which glyphs are written.</param>
        /// <param name="activeDevices">Active devices</param>
        /// <param name="inputLayoutPath">example: &lt;gamepad&gt;/dpad/left</param>
        /// <returns>Return true if the load was success.</returns>
        bool LoadGlyph(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath);
    }
}
#endif
