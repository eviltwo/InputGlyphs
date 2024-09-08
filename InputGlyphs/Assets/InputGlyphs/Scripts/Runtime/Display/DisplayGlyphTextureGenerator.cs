#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
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

        /// <summary>
        /// Generates glyph texture for the specified inputLayoutPaths and writes it to the texture. The glyph textures are arranged according to the layout.
        /// </summary>
        public static bool GenerateGlyphTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths, GlyphsLayoutData layoutData)
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

            if (layoutData.Layout == GlyphsLayout.Single)
            {
                return GenerateSingleGlyphTexture(texture, activeDevices, inputLayoutPaths, layoutData.Index);
            }
            else
            {
                return GenerateMultipleGlyphsTexture(texture, activeDevices, inputLayoutPaths, layoutData.MaxCount);
            }
        }

        private static bool GenerateSingleGlyphTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths, int index)
        {
            if (index < 0 || index >= inputLayoutPaths.Count)
            {
                return false;
            }

            if (InputGlyphManager.LoadGlyph(texture, activeDevices, inputLayoutPaths[index]))
            {
                return true;
            }

            return false;
        }

        private static bool GenerateMultipleGlyphsTexture(Texture2D texture, IReadOnlyList<InputDevice> activeDevices, IReadOnlyList<string> inputLayoutPaths, int maxCount)
        {
            if (inputLayoutPaths.Count == 1)
            {
                return GenerateSingleGlyphTexture(texture, activeDevices, inputLayoutPaths, 0);
            }

            _textureBuffer.Clear();
            var loadedCount = 0;
            for (int i = 0; i < inputLayoutPaths.Count; i++)
            {
                var texTemp = new Texture2D(2, 2);
                if (InputGlyphManager.LoadGlyph(texTemp, activeDevices, inputLayoutPaths[i]))
                {
                    _textureBuffer.Add(texTemp);
                    loadedCount++;
                    if (loadedCount >= maxCount)
                    {
                        break;
                    }
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
