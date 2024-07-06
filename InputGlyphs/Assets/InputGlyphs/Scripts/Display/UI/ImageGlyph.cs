#if ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InputGlyphs.Display
{
    [RequireComponent(typeof(Image))]
    public class ImageGlyph : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        public Image Image = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference InputActionReference = null;

        private Vector2 _defaultSizeDelta;
        private PlayerInput _lastPlayerInput;
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;
        private List<Texture2D> _textureBuffer = new List<Texture2D>();

        private void Reset()
        {
            Image = GetComponent<Image>();
        }

        private void Awake()
        {
            if (Image == null)
            {
                Image = GetComponent<Image>();
            }
            _defaultSizeDelta = Image.rectTransform.sizeDelta;
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
            if (Image != null)
            {
                Destroy(Image.sprite);
                Image.sprite = null;
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
                if (_pathBuffer.Count == 1)
                {
                    if (InputGlyphManager.LoadGlyph(_texture, devices, _pathBuffer[0]))
                    {
                        Destroy(Image.sprite);
                        Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Max(_texture.width, _texture.height));
                        Image.rectTransform.sizeDelta = _defaultSizeDelta;
                    }
                }
                else
                {
                    _textureBuffer.Clear();
                    for (int i = 0; i < _pathBuffer.Count; i++)
                    {
                        var texture = new Texture2D(2, 2);
                        if (InputGlyphManager.LoadGlyph(texture, devices, _pathBuffer[i]))
                        {
                            _textureBuffer.Add(texture);
                        }
                        else
                        {
                            Destroy(texture);
                        }
                    }
                    if (GlyphTextureUtility.MergeTexturesHorizontal(_texture, _textureBuffer))
                    {
                        Destroy(Image.sprite);
                        Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
                        var ratio = (float)_texture.width / _texture.height;
                        Image.rectTransform.sizeDelta = new Vector2(_defaultSizeDelta.y * ratio, _defaultSizeDelta.y);
                    }
                    for (int i = 0; i < _textureBuffer.Count; i++)
                    {
                        Destroy(_textureBuffer[i]);
                    }
                    _textureBuffer.Clear();
                }
            }
        }
    }
}
#endif
