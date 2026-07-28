using UnityEngine;

namespace InputGlyphs.Display
{
    public static class InputGlyphDisplaySettings
    {
        /// <summary>
        /// Automatically collect when PlayerInput is null.
        /// </summary>
        public static bool AutoCollectPlayerInput = true;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RuntimeInitializeOnLoad()
        {
            // Required when Domain Reload is disabled.
            AutoCollectPlayerInput = true;
        }
    }
}
