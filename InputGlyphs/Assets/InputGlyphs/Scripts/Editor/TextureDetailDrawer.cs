using UnityEditor;
using UnityEngine;
using InputGlyphs.Loaders.Utils;

namespace InputGlyphs.Editor
{
    [CustomPropertyDrawer(typeof(InputGlyphTextureMap.TextureDetail))]
    public class TextureDetailDrawer : PropertyDrawer
    {
        private const float PreviewSize = 48f;
        private const float Spacing = 10f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                // Reset indent level to avoid shifting elements inside the list
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                
                // Path input field
                var pathProp = property.FindPropertyRelative("InputLayoutLocalPath");
                var pathRect = new Rect(
                    position.x,
                    position.y + (position.height - EditorGUIUtility.singleLineHeight) / 2f,
                    position.width - PreviewSize - Spacing,
                    EditorGUIUtility.singleLineHeight
                );
                EditorGUI.PropertyField(pathRect, pathProp, GUIContent.none);

                // Texture preview field (square)
                var textureRect = new Rect(
                    position.xMax - PreviewSize,
                    position.y + (position.height - PreviewSize) / 2f,
                    PreviewSize,
                    PreviewSize
                );
                var textureProp = property.FindPropertyRelative("GlyphTexture");
                textureProp.objectReferenceValue = EditorGUI.ObjectField(
                    textureRect,
                    textureProp.objectReferenceValue,
                    typeof(Texture2D),
                    false
                );

                // Restore indent level
                EditorGUI.indentLevel = indent;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return PreviewSize + 4f;
        }
    }
}
