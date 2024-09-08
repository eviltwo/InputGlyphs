#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using InputGlyphs.Editor;
using UnityEditor;

namespace InputGlyphs.Display.Editor
{
    [CustomEditor(typeof(InputGlyphSprite)), CanEditMultipleObjects]
    public class InputGlyphSpriteEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var playerInputError = false;
            foreach (var t in targets)
            {
                var glyphImage = (InputGlyphSprite)t;
                if (glyphImage.PlayerInput != null && !InputGlyphEditorUtility.ValidatePlayerInputNotificationBehavior(glyphImage.PlayerInput))
                {
                    playerInputError = true;
                    break;
                }
            }
            if (playerInputError)
            {
                EditorGUILayout.HelpBox(InputGlyphEditorUtility.GetPlayerInputNotificationBehaviorErrorMessage(), MessageType.Error);
            }
        }
    }
}
#endif
