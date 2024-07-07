#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Loaders.Utils;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
using UnityEngine.InputSystem.XInput;

namespace InputGlyphs.Loaders
{
    public class GamepadGlyphLoader : IInputGlyphLoader
    {
        private readonly InputGlyphTextureMap _fallbackTextureMap;
        private readonly InputGlyphTextureMap _xboxControllerTextureMap;
        private readonly InputGlyphTextureMap _playstationControllerTextureMap;
        private readonly InputGlyphTextureMap _switchProControllerTextureMap;

        public GamepadGlyphLoader(
            InputGlyphTextureMap fallbackTextureMap,
            InputGlyphTextureMap xboxControllerTextureMap,
            InputGlyphTextureMap playstationControllerTextureMap,
            InputGlyphTextureMap switchProControllerTextureMap)
        {
            _fallbackTextureMap = fallbackTextureMap;
            _xboxControllerTextureMap = xboxControllerTextureMap;
            _playstationControllerTextureMap = playstationControllerTextureMap;
            _switchProControllerTextureMap = switchProControllerTextureMap;
        }

        public bool LoadGlyph(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, string inputLayoutPath)
        {
            InputGlyphTextureMap activeTextureMap = null;
            for (var i = 0; i < activeDevices.Count; i++)
            {
                var activeDevice = activeDevices[i];
                activeTextureMap = GetTextureMap(activeDevice);
                if (activeTextureMap == null && activeDevice is Gamepad)
                {
                    activeTextureMap = _fallbackTextureMap;
                }
                if (activeTextureMap != null)
                {
                    break;
                }
            }
            if (activeTextureMap == null)
            {
                return false;
            }

            var localPath = InputLayoutPathUtility.GetLocalPath(inputLayoutPath);
            if (activeTextureMap.TryGetTexture(localPath, out var result))
            {
                texture.Reinitialize(result.width, result.height, TextureFormat.ARGB32, false);
                texture.SetPixels(result.GetPixels());  // Glyph texture must be readable
                texture.Apply();
                return true;
            }

            return false;
        }

        private InputGlyphTextureMap GetTextureMap(InputDevice device)
        {
            switch (device)
            {
                case XInputController:
                    return _xboxControllerTextureMap;

                case DualShockGamepad:
                    return _playstationControllerTextureMap;

                case SwitchProControllerHID:
                    return _switchProControllerTextureMap;

                case Gamepad:
                    return _fallbackTextureMap;

                default:
                    return null;
            }
        }
    }
}
#endif
