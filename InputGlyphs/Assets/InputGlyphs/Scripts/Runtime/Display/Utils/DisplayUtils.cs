using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace InputGlyphs.Display
{
    public static class DisplayUtils
    {
        public static bool CollectDevicesForControlScheme(InputControlScheme controlScheme, List<InputDevice> results, PlayerInput playerInput = null)
        {
            if (playerInput != null)
            {
                using (var pickResult = controlScheme.PickDevicesFrom(playerInput.devices))
                {
                    if (pickResult.isSuccessfulMatch)
                    {
                        foreach (var device in pickResult.devices)
                        {
                            results.Add(device);
                        }
                        
                        return true;
                    }
                }
            }
            
            using (var pickResult = controlScheme.PickDevicesFrom(InputSystem.devices))
            {
                if (pickResult.isSuccessfulMatch)
                {
                    foreach (var device in pickResult.devices)
                    {
                        results.Add(device);
                    }
                    
                    return true;
                }
            }

            return false;
        }
    }
}
