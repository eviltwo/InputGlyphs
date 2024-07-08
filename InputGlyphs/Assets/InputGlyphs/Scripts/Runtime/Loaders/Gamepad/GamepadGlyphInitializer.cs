#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using InputGlyphs.Loaders.Utils;
using UnityEngine;

namespace InputGlyphs.Loaders
{
    public class GamepadGlyphInitializer : MonoBehaviour
    {
        [SerializeField]
        private InputGlyphTextureMap _fallbackTextureMap = null;

        [SerializeField]
        private InputGlyphTextureMap _xboxTextureMap = null;

        [SerializeField]
        private InputGlyphTextureMap _playstationTextureMap = null;

        [SerializeField]
        private InputGlyphTextureMap _switchProControllerTextureMap = null;

        private static bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            var gamepadGlyphLoader = new GamepadGlyphLoader(
                _fallbackTextureMap,
                _xboxTextureMap,
                _playstationTextureMap,
                _switchProControllerTextureMap);
            InputGlyphManager.RegisterLoader(gamepadGlyphLoader);
        }
    }
}
#endif
