#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using InputGlyphs.Loaders.Utils;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA
using UnityEngine.InputSystem.Switch;
#endif
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
            var supportedDevice = activeDevices.OfType<Gamepad>().FirstOrDefault();
            if (supportedDevice == null)
            {
                return false;
            }

            var textureMap = GetTextureMap(supportedDevice);
            var activeTextureMap = textureMap != null ? textureMap : _fallbackTextureMap;
            if (activeTextureMap == null)
            {
                return false;
            }

            var localPath = InputLayoutPathUtility.RemoveRoot(inputLayoutPath);
            if (InputLayoutPathUtility.HasPathComponent(inputLayoutPath))
            {
                var control = supportedDevice.TryGetChildControl(inputLayoutPath);
                if (control != null)
                {
                    inputLayoutPath = control.path;
                    localPath = InputLayoutPathUtility.RemoveRoot(inputLayoutPath);
                }
            }

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

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_WSA
                case SwitchProControllerHID:
                    return _switchProControllerTextureMap;
#endif

                case Gamepad:
                    return _fallbackTextureMap;

                default:
                    return null;
            }
        }
    }
}
#endif
