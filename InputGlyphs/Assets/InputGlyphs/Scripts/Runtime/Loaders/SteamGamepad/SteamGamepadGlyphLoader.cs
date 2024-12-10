#if STEAMWORKS_NET && SUPPORT_ADAPTER && !DISABLESTEAMWORKS
using System.Collections.Generic;
using System.IO;
using Steamworks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnitySteamInputAdapter;

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

            // Get path of Glyph image file.
            var steamInputAction = SteamInputAdapter.GetSteamInputAction(supportedDevice, inputLayoutPath);
            var glyphPath = SteamInput.GetGlyphPNGForActionOrigin(steamInputAction, GlyphSize, 0);
            if (string.IsNullOrEmpty(glyphPath))
            {
                return false;
            }

            return LoadImage(texture, glyphPath);
        }

        public static bool LoadImage(Texture2D texture, string path)
        {
            var bytes = File.ReadAllBytes(path);
            return texture.LoadImage(bytes);
        }
    }
}
#endif