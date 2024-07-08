#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace InputGlyphs.Utils
{
    public static class InputActionRebindingExtensions
    {
        /// <summary>
        /// Gets the indexes of all bindings in the action's bindings that match the specified binding mask.
        /// This function just changes the number of results of <see cref="InputActionRebindingExtensions.GetBindingIndex()"/> .
        /// </summary>
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
