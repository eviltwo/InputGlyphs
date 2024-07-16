using UnityEngine;

namespace InputGlyphs.Loaders
{
    public class SteamGamepadGlyphInitializer : MonoBehaviour
    {
#if STEAMWORKS_NET && !DISABLESTEAMWORKS && SUPPORT_ADAPTER
        private static bool _initialized;

        private void Awake()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            var loader = new SteamGamepadGlyphLoader();
            InputGlyphManager.RegisterLoader(loader);
        }
#endif
    }
}
