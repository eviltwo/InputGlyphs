#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using InputGlyphs.Loaders.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Loaders
{
    [AddComponentMenu("InputGlyphs/Initializer/MouseGlyphInitializer")]
    public class MouseGlyphInitializer : DeviceGlyphLoaderInitializer<Mouse>
    {
    }
}
#endif