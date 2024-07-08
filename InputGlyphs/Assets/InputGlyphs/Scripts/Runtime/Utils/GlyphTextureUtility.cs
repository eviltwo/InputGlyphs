using System.Collections.Generic;
using UnityEngine;

namespace InputGlyphs.Utils
{
    public static class GlyphTextureUtility
    {
        /// <summary>
        /// Combines multiple textures into a single texture arranged in a horizontal row.
        /// </summary>
        /// <param name="texture">texture to write the results to.</param>
        /// <param name="sourceTextures">Original textures. They can be of different sizes.</param>
        /// <returns>Return true if the load was success.</returns>
        public static bool MergeTexturesHorizontal(Texture2D texture, IReadOnlyList<Texture2D> sourceTextures)
        {
            var width = 0;
            var height = 0;
            for (int i = 0; i < sourceTextures.Count; i++)
            {
                var sourceTexture = sourceTextures[i];
                if (sourceTexture != null)
                {
                    width += sourceTexture.width;
                    height = Mathf.Max(height, sourceTexture.height);
                }
            }

            if (width <= 0f || height <= 0f)
            {
                return false;
            }

            texture.Reinitialize(width, height, TextureFormat.ARGB32, true);

            var posX = 0;
            for (int i = 0; i < sourceTextures.Count; i++)
            {
                var sourceTexture = sourceTextures[i];
                if (sourceTexture != null)
                {
                    var writeHeight = Mathf.FloorToInt((height - sourceTexture.height) * 0.5f);
                    texture.SetPixels(posX, writeHeight, sourceTexture.width, sourceTexture.height, sourceTexture.GetPixels());
                    posX += sourceTexture.width;
                }
            }
            texture.Apply();
            return true;
        }
    }
}
