using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using InputGlyphs.Loaders.Utils;

namespace InputGlyphs.Editor
{
    [CustomEditor(typeof(InputGlyphTextureMap), editorForChildClasses: true)]
    public class InputGlyphTextureMapEditor : UnityEditor.Editor
    {
        private ReorderableList _list;
        private Vector2 _scrollPosition;

        private void OnEnable()
        {
            var textureDetailsProp = serializedObject.FindProperty("TextureDetails");
            if (textureDetailsProp == null) return;

            _list = new ReorderableList(serializedObject, textureDetailsProp, true, true, true, true);

            _list.drawHeaderCallback = (rect) =>
            {
                EditorGUI.LabelField(rect, "Texture Details Map");
            };

            _list.drawElementCallback = (rect, index, _, _) =>
            {
                var element = textureDetailsProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, element, GUIContent.none);
            };

            _list.elementHeightCallback = (index) =>
            {
                var element = textureDetailsProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element, GUIContent.none);
            };
        }

        public override void OnInspectorGUI()
        {
            if (_list == null)
            {
                base.OnInspectorGUI();
                return;
            }

            serializedObject.Update();

            _list.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
