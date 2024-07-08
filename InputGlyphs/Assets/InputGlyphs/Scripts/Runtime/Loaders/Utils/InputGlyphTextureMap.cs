using UnityEngine;

namespace InputGlyphs.Loaders.Utils
{
    /// <summary>
    /// A map that associates control paths of input devices with glyph textures.
    /// </summary>
    [CreateAssetMenu(fileName = "InputGlyphTextureMap", menuName = "InputGlyphs/InputGlyphTextureMap")]
    public class InputGlyphTextureMap : ScriptableObject
    {
        [System.Serializable]
        public class TextureDetail
        {
            [SerializeField]
            public string InputLayoutLocalPath;

            [SerializeField]
            public Texture2D GlyphTexture;
        }

        [SerializeField]
        public TextureDetail[] TextureDetails = null;

        public bool TryGetTexture(string inputLayoutLocalPath, out Texture2D texture)
        {
            for (var i = 0; i < TextureDetails.Length; i++)
            {
                var textureDetails = TextureDetails[i];
                if (textureDetails.InputLayoutLocalPath == inputLayoutLocalPath)
                {
                    texture = textureDetails.GlyphTexture;
                    return true;
                }
            }
            texture = null;
            return false;
        }
    }
}
