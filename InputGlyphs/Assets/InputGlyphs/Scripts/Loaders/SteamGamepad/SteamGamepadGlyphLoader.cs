#if STEAMWORKS_NET && SUPPORT_ADAPTER && SUPPORT_LOADER
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnitySteamInputGlyphLoader;

namespace InputGlyphs.Loaders
{
    public class SteamGamepadGlyphLoader : IInputGlyphLoader
    {
        public ESteamInputGlyphSize GlyphSize = ESteamInputGlyphSize.k_ESteamInputGlyphSize_Small;

        public bool LoadGlyph(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath)
        {
            InputDevice supportedDevice = null;
            for (var i = 0; i < activeDevices.Count; i++)
            {
                var activeDevice = activeDevices[i];
                if (activeDevice is Gamepad)
                {
                    supportedDevice = activeDevice;
                    break;
                }
            }
            if (supportedDevice == null)
            {
                return false;
            }

            return SteamInputGlyphLoader.LoadGlyph(texture, supportedDevice, inputLayoutPath, GlyphSize);
        }
    }
}
#endif