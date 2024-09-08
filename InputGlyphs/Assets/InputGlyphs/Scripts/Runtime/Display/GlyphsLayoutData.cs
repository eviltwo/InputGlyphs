using System;

namespace InputGlyphs.Display
{
    [Serializable]
    public struct GlyphsLayoutData
    {
        public GlyphsLayout Layout;
        public int Index;
        public int MaxCount;

        public static GlyphsLayoutData Default => new GlyphsLayoutData
        {
            Layout = GlyphsLayout.Horizontal,
            Index = 0,
            MaxCount = 4,
        };
    }

    public enum GlyphsLayout
    {
        Single = 1,
        Horizontal = 2,
    }
}
