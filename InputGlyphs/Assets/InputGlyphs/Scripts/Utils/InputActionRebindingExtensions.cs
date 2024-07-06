#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace InputGlyphs.Utils
{
    public static class InputActionRebindingExtensions
    {
        public static void GetBindingIndexes(this InputAction action, InputBinding bindingMask, List<int> results)
        {
            results.Clear();
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var bindings = action.bindings;
            for (var i = 0; i < bindings.Count; ++i)
            {
                if (bindingMask.Matches(bindings[i]))
                {
                    results.Add(i);
                }
            }
        }
    }
}
#endif
