#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputGlyphs.Display
{
    public class InputGlyphSprite : MonoBehaviour
    {
        [SerializeField]
        public SpriteRenderer SpriteRenderer = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference InputActionReference = null;

        [SerializeField]
        public GlyphsLayoutData GlyphsLayoutData = GlyphsLayoutData.Default;

        private PlayerInput _lastPlayerInput;
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;

        private void Reset()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
#if UNITY_2022_3_OR_NEWER
            PlayerInput = FindAnyObjectByType<PlayerInput>();
#else
            PlayerInput = FindObjectOfType<PlayerInput>();
#endif
        }

        private void Awake()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            _texture = new Texture2D(2, 2);
        }

        private void Start()
        {
            if (PlayerInput == null && InputGlyphDisplaySettings.AutoCollectPlayerInput)
            {
                PlayerInput = PlayerInput.all.FirstOrDefault();
            }
            if (PlayerInput == null)
            {
                Debug.LogWarning("PlayerInput is not set.", this);
            }
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
            if (PlayerInput == null && InputGlyphDisplaySettings.AutoCollectPlayerInput)
            {
                PlayerInput = PlayerInput.all.FirstOrDefault();
            }

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
            if (playerInput == PlayerInput)
            {
                UpdateGlyphs(playerInput);
            }
        }

        public void UpdateGlyphs()
        {
            UpdateGlyphs(PlayerInput);
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

            if (InputActionReference == null || InputActionReference.action == null)
            {
                Debug.LogWarning("InputActionReference is not set.", this);
                return;
            }

            var playerInputAction = playerInput.actions.FindAction(InputActionReference.action.id);
            if (InputLayoutPathUtility.TryGetActionBindingPath(playerInputAction, PlayerInput.currentControlScheme, _pathBuffer))
            {
                if (DisplayGlyphTextureGenerator.GenerateGlyphTexture(_texture, devices, _pathBuffer, GlyphsLayoutData))
                {
                    Destroy(SpriteRenderer.sprite);
                    SpriteRenderer.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
                }
            }
        }
    }
}
#endif
