#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.GlyphLoaderCore
{
    public class InputGlyphLoader<T> : IInputGlyphLoader
        where T : InputDevice
    {
        public readonly List<InputGlyphTextureMap> TextureMaps = new List<InputGlyphTextureMap>();

        public Texture2D GetGlyph(IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath)
        {
            var isActiveDevice = false;
            for (var i = 0; i < activeDevices.Count; i++)
            {
                if (activeDevices[i] is T)
                {
                    isActiveDevice = true;
                    break;
                }
            }
            if (!isActiveDevice)
            {
                return null;
            }

            var localPath = InputLayoutPathUtility.GetLocalPath(inputLayoutPath);
            for (var i = 0; i < TextureMaps.Count; i++)
            {
                if (TextureMaps[i].TryGetTexture(localPath, out var texture))
                {
                    return texture;
                }
            }

            return null;
        }
    }
}
#endif
