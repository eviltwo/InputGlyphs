using UnityEditor;
using UnityEngine;

namespace InputGlyphs.Display.Editor
{
    [CustomPropertyDrawer(typeof(GlyphsLayoutData))]
    public class GlyphsLayoutDataDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var rect = position;
                rect.height = EditorGUIUtility.singleLineHeight;

                var layoutProperty = property.FindPropertyRelative(nameof(GlyphsLayoutData.Layout));
                EditorGUI.PropertyField(rect, layoutProperty, new GUIContent($"Glyphs {layoutProperty.displayName}"));
                rect.y += EditorGUIUtility.singleLineHeight;

                var layout = (GlyphsLayout)layoutProperty.intValue;
                if (layout == GlyphsLayout.Single)
                {
                    var indexProperty = property.FindPropertyRelative(nameof(GlyphsLayoutData.Index));
                    EditorGUI.PropertyField(rect, indexProperty, new GUIContent($"Glyphs {indexProperty.displayName}"));
                }
                else if (layout == GlyphsLayout.Horizontal)
                {
                    var maxCountProperty = property.FindPropertyRelative(nameof(GlyphsLayoutData.MaxCount));
                    EditorGUI.PropertyField(rect, maxCountProperty, new GUIContent($"Glyphs {maxCountProperty.displayName}"));
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var layoutProperty = property.FindPropertyRelative(nameof(GlyphsLayoutData.Layout));
            var layout = (GlyphsLayout)layoutProperty.intValue;
            var lineCount = 2;
            return EditorGUIUtility.singleLineHeight * lineCount;
        }
    }
}
