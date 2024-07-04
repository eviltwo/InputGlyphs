#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs
{
    public interface IInputGlyphLoader
    {
        Texture2D GetGlyph(IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath);
    }
}
#endif
