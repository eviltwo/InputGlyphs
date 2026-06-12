#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using InputGlyphs.Attributes;
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
        
        [SerializeField, ControlSchemeName]
        public string ControlScheme;

        private readonly PlayerInputChangeDetector _playerInputChangeDetector = new ();
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;
        private Sprite _createdSprite;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
#if UNITY_2022_3_OR_NEWER
            PlayerInput = FindAnyObjectByType<PlayerInput>();
#else
            PlayerInput = FindObjectOfType<PlayerInput>();
#endif
        }
#endif // UNITY_EDITOR

        protected virtual void Awake()
        {
            if (SpriteRenderer == null)
            {
                SpriteRenderer = GetComponent<SpriteRenderer>();
            }
            _texture = new Texture2D(2, 2);
        }

        protected virtual void Start()
        {
            UpdateGlyphs(PlayerInput);
        }
        
        protected virtual void OnEnable()
        {
            _playerInputChangeDetector.ControlsChanged += UpdateGlyphs;
        }

        protected virtual void OnDisable()
        {
            _playerInputChangeDetector.OnDisable();
            _playerInputChangeDetector.ControlsChanged -= UpdateGlyphs;
        }

        protected virtual void OnDestroy()
        {
            Destroy(_texture);
            _texture = null;
            Destroy(_createdSprite);
            _createdSprite = null;
            if (SpriteRenderer != null)
            {
                SpriteRenderer.sprite = null;
            }
        }
        
        protected virtual void Update()
        {
            if (PlayerInput == null && InputGlyphDisplaySettings.AutoCollectPlayerInput)
            {
                PlayerInput = PlayerInput.all.FirstOrDefault();
            }
            
            _playerInputChangeDetector.Update(PlayerInput);
        }

        public void UpdateGlyphs()
        {
            UpdateGlyphs(PlayerInput);
        }
        
        private readonly List<InputDevice>  _deviceBuffer = new();
        
        private void UpdateGlyphs(PlayerInput playerInput)
        {
            if (InputActionReference == null || InputActionReference.action == null)
            {
                Debug.LogWarning("InputActionReference is not set.", this);
                return;
            }
            
            _deviceBuffer.Clear();
            string controlScheme;
            if (string.IsNullOrEmpty(ControlScheme))
            {
                if (playerInput == null)
                {
                    _deviceBuffer.Clear();
                    controlScheme = string.Empty;
                }
                else
                {
                    _deviceBuffer.AddRange(playerInput.devices);
                    controlScheme = playerInput.currentControlScheme;
                }
            }
            else
            {
                DisplayUtils.CollectDevicesForControlScheme(InputActionReference.action.actionMap.controlSchemes.FirstOrDefault(v => v.name == ControlScheme), _deviceBuffer, playerInput);
                controlScheme = ControlScheme;
            }

            var playerInputAction = playerInput?.actions.FindAction(InputActionReference.action.id) ?? InputActionReference.action;
            if (InputLayoutPathUtility.TryGetActionBindingPath(playerInputAction, controlScheme, _pathBuffer)
                && DisplayGlyphTextureGenerator.GenerateGlyphTexture(_texture, _deviceBuffer, _pathBuffer, GlyphsLayoutData))
            {
                Destroy(_createdSprite);
                _createdSprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
                SpriteRenderer.sprite = _createdSprite;
            }
            else
            {
                HandleGlyphTextureGenerationFailed();
            }
        }
        
        protected virtual void HandleGlyphTextureGenerationFailed()
        {
            _texture.Reinitialize(8, 8);
            Destroy(_createdSprite);
            _createdSprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
            SpriteRenderer.sprite = _createdSprite;
        }
    }
}
#endif
