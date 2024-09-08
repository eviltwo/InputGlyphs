#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEditor;

namespace InputGlyphs.Display.Editor
{
    [CustomEditor(typeof(InputGlyphImage))]
    public class InputGlyphImageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var imageProperty = serializedObject.FindProperty(nameof(InputGlyphImage.Image));
            EditorGUILayout.PropertyField(imageProperty);

            var playerInputProperty = serializedObject.FindProperty(nameof(InputGlyphImage.PlayerInput));
            EditorGUILayout.PropertyField(playerInputProperty);

            var inputActionReferenceProperty = serializedObject.FindProperty(nameof(InputGlyphImage.InputActionReference));
            EditorGUILayout.PropertyField(inputActionReferenceProperty);

            var glyphsLayoutDataProperty = serializedObject.FindProperty(nameof(InputGlyphImage.GlyphsLayoutData));
            EditorGUILayout.PropertyField(glyphsLayoutDataProperty);

            var enableLayoutElementProperty = serializedObject.FindProperty(nameof(InputGlyphImage.EnableLayoutElement));
            EditorGUILayout.PropertyField(enableLayoutElementProperty);
            if (enableLayoutElementProperty.boolValue)
            {
                var layoutElementPriorityProperty = serializedObject.FindProperty(nameof(InputGlyphImage.LayoutElementPriority));
                EditorGUILayout.PropertyField(layoutElementPriorityProperty);

                var layoutElementSizeProperty = serializedObject.FindProperty(nameof(InputGlyphImage.LayoutElementSize));
                EditorGUILayout.PropertyField(layoutElementSizeProperty);
            }

            serializedObject.ApplyModifiedProperties();

            var glyphImage = (InputGlyphImage)target;

            if (glyphImage.PlayerInput != null
                && glyphImage.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeUnityEvents
                && glyphImage.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeCSharpEvents)
            {
                EditorGUILayout.HelpBox("PlayerInput.notificationBehavior must be set to InvokeUnityEvents or InvokeCSharpEvents.", MessageType.Error);
            }
        }
    }
}
#endif
