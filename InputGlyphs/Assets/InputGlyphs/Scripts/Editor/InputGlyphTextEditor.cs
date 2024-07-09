#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM && SUPPORT_TMPRO
using System.Text;
using UnityEditor;
using UnityEngine;

namespace InputGlyphs.Display.Editor
{
    [CustomEditor(typeof(InputGlyphText))]
    public class InputGlyphTextEditor : UnityEditor.Editor
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var glyphText = (InputGlyphText)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Enter the following tag into the text:");
            using (new EditorGUI.IndentLevelScope())
            {
                for (int i = 0; i < glyphText.InputActionReferences.Length; i++)
                {
                    if (glyphText.InputActionReferences[i] == null)
                    {
                        continue;
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        _stringBuilder.Clear();
                        _stringBuilder.Append("<sprite name=");
                        _stringBuilder.Append(glyphText.InputActionReferences[i].action.name);
                        _stringBuilder.Append(">");
                        EditorGUILayout.LabelField(_stringBuilder.ToString());
                        if (GUILayout.Button("Copy"))
                        {
                            EditorGUIUtility.systemCopyBuffer = _stringBuilder.ToString();
                        }
                    }
                }
            }

            if (glyphText.PlayerInput != null
                && glyphText.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeUnityEvents
                && glyphText.PlayerInput.notificationBehavior != UnityEngine.InputSystem.PlayerNotifications.InvokeCSharpEvents)
            {
                EditorGUILayout.HelpBox("PlayerInput.notificationBehavior must be set to InvokeUnityEvents or InvokeCSharpEvents.", MessageType.Error);
            }
        }
    }
}

#endif
