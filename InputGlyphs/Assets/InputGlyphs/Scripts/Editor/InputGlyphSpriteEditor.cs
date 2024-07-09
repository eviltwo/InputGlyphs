#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEditor;

namespace InputGlyphs.Display.Editor
{
    [CustomEditor(typeof(InputGlyphSprite))]
    public class InputGlyphSpriteEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var glyphSprite = (InputGlyphSprite)target;

            if (glyphSprite.PlayerInput != null
                && glyphSprite.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeUnityEvents
                && glyphSprite.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeCSharpEvents)
            {
                EditorGUILayout.HelpBox("PlayerInput.notificationBehavior must be set to InvokeUnityEvents or InvokeCSharpEvents.", MessageType.Error);
            }
        }
    }
}
#endif
