#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Display
{
    /// <summary>
    /// Generates layout-aligned glyphs for display.
    /// </summary>
    public static class DisplayGlyphTextureGenerator
    {
        private static List<Texture2D> _textureBuffer = new List<Texture2D>();

        public static bool GenerateGlyphTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths, GlyphsLayout layout)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null.");
                return false;
            }

            if (activeDevices == null || activeDevices.Count == 0 || inputLayoutPaths == null || inputLayoutPaths.Count == 0)
            {
                return false;
            }

            if (inputLayoutPaths.Count == 1 || layout == GlyphsLayout.Single)
            {
                return GenerateSingleGlyphTexture(texture, activeDevices, inputLayoutPaths);
            }
            else
            {
                return GenerateMultipleGlyphsTexture(texture, activeDevices, inputLayoutPaths);
            }
        }

        private static bool GenerateSingleGlyphTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths)
        {
            for (int i = 0; i < inputLayoutPaths.Count; i++)
            {
                if (InputGlyphManager.LoadGlyph(texture, activeDevices, inputLayoutPaths[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool GenerateMultipleGlyphsTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths)
        {
            _textureBuffer.Clear();
            for (int i = 0; i < inputLayoutPaths.Count; i++)
            {
                var texTemp = new Texture2D(2, 2);
                if (InputGlyphManager.LoadGlyph(texTemp, activeDevices, inputLayoutPaths[i]))
                {
                    _textureBuffer.Add(texTemp);
                }
                else
                {
                    Object.Destroy(texTemp);
                }
            }
            if (_textureBuffer.Count == 0)
            {
                return false;
            }
            var successed = GlyphTextureUtility.MergeTexturesHorizontal(texture, _textureBuffer);
            for (int i = 0; i < _textureBuffer.Count; i++)
            {
                Object.Destroy(_textureBuffer[i]);
            }
            _textureBuffer.Clear();
            return successed;
        }
    }
}
#endif
