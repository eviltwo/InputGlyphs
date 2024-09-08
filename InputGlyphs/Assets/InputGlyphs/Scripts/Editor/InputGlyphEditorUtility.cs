#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace InputGlyphs.Editor
{
    public static class InputGlyphEditorUtility
    {
        public static bool ValidatePlayerInputNotificationBehavior(PlayerInput playerInput)
        {
#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
            return playerInput.notificationBehavior == PlayerNotifications.InvokeUnityEvents
                || playerInput.notificationBehavior == PlayerNotifications.InvokeCSharpEvents;
#else
            return true;
#endif
        }

        public static string GetPlayerInputNotificationBehaviorErrorMessage()
        {
            return "PlayerInput.notificationBehavior must be set to InvokeUnityEvents or InvokeCSharpEvents.";
        }
    }
}
