#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEditor;

namespace InputGlyphs.Display.Editor
{
    [CustomEditor(typeof(InputGlyphImage))]
    public class InputGlyphImageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

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
