#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.GlyphLoaderCore
{
    public class InputGlyphLoaderInitializer<T> : MonoBehaviour
        where T : InputDevice
    {
        [SerializeField]
        public InputGlyphTextureMap[] TextureMaps = null;

        private static bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }

            if (TextureMaps == null || TextureMaps.Length == 0)
            {
                return;
            }

            var loader = new InputGlyphLoader<T>();
            loader.TextureMaps.AddRange(TextureMaps);
            InputGlyphManager.RegisterLoader(loader);
        }
    }
}
#endif
