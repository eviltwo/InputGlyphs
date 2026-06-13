#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Loaders.Utils
{
    /// <summary>
    /// A general initialization class that creates glyph loader for various <see cref="InputDevice"/>, handles the transfer of <see cref="InputGlyphTextureMap"/>, and registers loader with the Manager.
    /// If you want to easily create a glyph loader for custom devices, it is recommended to inherit from this class.
    /// </summary>
    public class DeviceGlyphLoaderInitializer<T> : MonoBehaviour
        where T : InputDevice
    {
        [SerializeField]
        public InputGlyphTextureMap[] TextureMaps = null;

        private void Awake()
        {
            if (InputGlyphManager.HasLoader<DeviceGlyphLoader<T>>())
            {
                return;
            }

            if (TextureMaps == null || TextureMaps.Length == 0)
            {
                return;
            }

            var loader = new DeviceGlyphLoader<T>();
            loader.TextureMaps.AddRange(TextureMaps);
            InputGlyphManager.RegisterLoader(loader);
        }
    }
}
#endif
