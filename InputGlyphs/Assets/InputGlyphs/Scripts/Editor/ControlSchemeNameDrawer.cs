using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputGlyphs.Attributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Editor
{
    [CustomPropertyDrawer(typeof(ControlSchemeNameAttribute))]
    public class ControlSchemeNameDrawer : PropertyDrawer
    {
        private readonly List<string> _options = new ();
        private const string BlankMessage = "<Follow PlayerInput>";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }
            
            _options.Clear();
            _options.Add(BlankMessage);
            GetSchemeNames(property, _options);

            var propertyValue = property.stringValue;
            var isValidValue = false;
            var index = 0;
            if (string.IsNullOrWhiteSpace(propertyValue))
            {
                isValidValue = true;
                index = 0;
            }
            else
            {
                var foundIndex =  _options.FindIndex(x => x == propertyValue);
                if (foundIndex >= 0)
                {
                    isValidValue = true;
                    index = foundIndex;
                }
            }

            if (isValidValue)
            {
                index = EditorGUI.Popup(
                    position,
                    label.text,
                    index,
                    _options.ToArray());
                property.stringValue = index == 0 ? "" : _options[index];
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private static void GetSchemeNames(SerializedProperty property, List<string> results)
        {
            var target = property.serializedObject.targetObject;
            var type = target.GetType();
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var playerInputField = type.GetFields(bindingFlags).FirstOrDefault(f => f.FieldType == typeof(PlayerInput));
            if (playerInputField != null)
            {
                var playerInput = (PlayerInput)playerInputField.GetValue(target);
                if (playerInput != null && playerInput.actions != null && playerInput.actions.controlSchemes.Count > 0)
                {
                    results.AddRange(playerInput.actions.controlSchemes.Select(v => v.name));
                    return;
                }
            }
            
            var inputActionField = type.GetFields(bindingFlags).FirstOrDefault(f => f.FieldType == typeof(InputActionReference));
            if (inputActionField != null)
            {
                var inputActionReference = (InputActionReference)inputActionField.GetValue(target);
                if (inputActionReference != null && inputActionReference.action != null && inputActionReference.action.actionMap.controlSchemes.Count > 0)
                {
                    results.AddRange(inputActionReference.action.actionMap.controlSchemes.Select(v => v.name));
                    return;
                }
            }

            var inputActionArrayField = type.GetFields(bindingFlags).FirstOrDefault(f => f.FieldType == typeof(InputActionReference[]));
            if (inputActionArrayField != null)
            {
                var inputActionReferenceArray = (InputActionReference[])inputActionArrayField.GetValue(target);
                if (inputActionReferenceArray != null && inputActionReferenceArray.Length > 0 && inputActionReferenceArray[0] != null)
                {
                    results.AddRange(inputActionReferenceArray[0].action.actionMap.controlSchemes.Select(v => v.name));
                    return;
                }
            }
        }
    }
}
