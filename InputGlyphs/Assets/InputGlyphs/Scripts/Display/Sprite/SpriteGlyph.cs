#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Display
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteGlyph : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public SpriteRenderer SpriteRenderer = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference InputActionReference = null;

        private PlayerInput _lastPlayerInput;
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;

        private void Reset()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            _texture = new Texture2D(2, 2);
        }

        private void OnDisable()
        {
            if (_lastPlayerInput != null)
            {
                UnregisterPlayerInputEvents(_lastPlayerInput);
                _lastPlayerInput = null;
            }
        }

        private void OnDestroy()
        {
            Destroy(_texture);
            _texture = null;
            if (SpriteRenderer != null)
            {
                Destroy(SpriteRenderer.sprite);
                SpriteRenderer.sprite = null;
            }
        }

        private void Update()
        {
            if (PlayerInput != _lastPlayerInput)
            {
                if (_lastPlayerInput != null)
                {
                    UnregisterPlayerInputEvents(_lastPlayerInput);
                }
                if (PlayerInput == null)
                {
                    Debug.LogError("PlayerInput is not set.", this);
                }
                else
                {
                    RegisterPlayerInputEvents(PlayerInput);
                    UpdateGlyphs(PlayerInput);
                }
                _lastPlayerInput = PlayerInput;
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
            UpdateGlyphs(playerInput);
        }

        private void UpdateGlyphs(PlayerInput playerInput)
        {
            if (!playerInput.isActiveAndEnabled)
            {
                return;
            }

            var devices = playerInput.devices;
            if (devices.Count == 0)
            {
                Debug.LogWarning("No devices are connected.", this);
                return;
            }

            if (InputLayoutPathUtility.TryGetActionBindingPath(InputActionReference?.action, PlayerInput.currentControlScheme, _pathBuffer))
            {
                InputGlyphManager.LoadGlyph(_texture, devices, _pathBuffer[0]);
                if (_texture == null)
                {
                    Debug.LogError($"Failed to get glyph for input path: {_pathBuffer[0]}", this);
                    var white = Texture2D.whiteTexture;
                    _texture.Reinitialize(white.width, white.height, white.format, white.mipmapCount > 0);
                    _texture.Apply();
                    Graphics.CopyTexture(white, _texture);
                }
                Destroy(SpriteRenderer.sprite);
                SpriteRenderer.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }
}
#endif
