using UnityEngine;

namespace InputGlyphs.GlyphLoaderCore
{
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
