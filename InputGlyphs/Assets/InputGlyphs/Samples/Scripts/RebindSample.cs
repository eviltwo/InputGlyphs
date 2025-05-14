using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;

namespace InputGlyphs.Samples
{
    public class RebindSample : MonoBehaviour
    {
        public PlayerInput PlayerInput;

        public InputActionReference ActionReference;

        public UnityEvent OnComplete;

        private RebindingOperation _rebindOp;

        private bool _enableActionAfterRebind;

        private int _rebindingIndex;

        private void OnDisable()
        {
            _rebindOp?.Dispose();
            _rebindOp = null;
        }

        private static readonly List<int> _bindingIndexBuffer = new List<int>();

        public void Rebind()
        {
            var action = PlayerInput.actions.FindAction(ActionReference.action.id);
            action.GetBindingIndexes(InputBinding.MaskByGroup(PlayerInput.currentControlScheme), _bindingIndexBuffer);
            if (_bindingIndexBuffer.Count == 0)
            {
                Debug.LogError("No binding found for the current control scheme.");
                return;
            }

            _enableActionAfterRebind = action.enabled;
            action.Disable();
            _rebindingIndex = _bindingIndexBuffer[0];
            _rebindOp?.Dispose();
            _rebindOp = action.PerformInteractiveRebinding(_rebindingIndex)
                .OnComplete(OnCompleteBinding)
                .Start();
        }

        private void OnCompleteBinding(RebindingOperation op)
        {
            if (_enableActionAfterRebind)
            {
                op.action.Enable();
            }

            var binding = op.action.bindings[_rebindingIndex];
            Debug.Log($"[{op.action.GetBindingDisplayString()}] Rebinding complete. New binding: {binding.effectivePath}");

            OnComplete.Invoke();
        }
    }
}
