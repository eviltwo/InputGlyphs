using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Display
{
    public class PlayerInputChangeDetector
    {
        private PlayerInput _lastPlayerInput;
        
        public event System.Action<PlayerInput> ControlsChanged;
        
        public void OnDisable()
        {
            if (_lastPlayerInput != null)
            {
                UnregisterPlayerInputEvents(_lastPlayerInput);
                _lastPlayerInput = null;
            }
        }
        
        public void Update(PlayerInput playerInput)
        {
            if (playerInput != null && !playerInput.isActiveAndEnabled)
            {
                playerInput = null;
            }
            
            if (playerInput != _lastPlayerInput)
            {
                if (_lastPlayerInput != null)
                {
                    UnregisterPlayerInputEvents(_lastPlayerInput);
                }
                
                _lastPlayerInput = playerInput;
                
                if (playerInput != null)
                {
                    RegisterPlayerInputEvents(playerInput);
                    ControlsChanged?.Invoke(playerInput);
                }
            }
        }
        
        private void RegisterPlayerInputEvents(PlayerInput playerInput)
        {
            switch (playerInput.notificationBehavior)
            {
                case PlayerNotifications.InvokeUnityEvents:
                    playerInput.controlsChangedEvent.AddListener(OnControlsChanged);
                    break;
                case PlayerNotifications.InvokeCSharpEvents:
                    playerInput.onControlsChanged += OnControlsChanged;
                    break;
            }
        }

        private void UnregisterPlayerInputEvents(PlayerInput playerInput)
        {
            switch (playerInput.notificationBehavior)
            {
                case PlayerNotifications.InvokeUnityEvents:
                    playerInput.controlsChangedEvent.RemoveListener(OnControlsChanged);
                    break;
                case PlayerNotifications.InvokeCSharpEvents:
                    playerInput.onControlsChanged -= OnControlsChanged;
                    break;
            }
        }
        
        private void OnControlsChanged(PlayerInput playerInput)
        {
            if (playerInput == _lastPlayerInput)
            {
                ControlsChanged?.Invoke(playerInput);
            }
        }
    }
}
