#if ENABLE_INPUT_SYSTEM
using InputGlyphs.GlyphLoaderCore;
using UnityEngine;

namespace InputGlyphs.GamepadGlyphs
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

        private void Awake()
        {
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
