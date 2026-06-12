#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
using InputGlyphs.Attributes;
using InputGlyphs.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace InputGlyphs.Display
{
    public class InputGlyphImage : UIBehaviour, ILayoutElement
    {
        [SerializeField]
        public Image Image = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference InputActionReference = null;

        [SerializeField]
        public GlyphsLayoutData GlyphsLayoutData = GlyphsLayoutData.Default;
        
        [SerializeField, ControlSchemeName]
        public string ControlScheme;

        private readonly PlayerInputChangeDetector _playerInputChangeDetector = new ();
        private Vector2 _defaultSizeDelta;
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;
        private Sprite _createdSprite;

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
            Image = GetComponent<Image>();
#if UNITY_2022_3_OR_NEWER
            PlayerInput = FindAnyObjectByType<PlayerInput>();
#else
            PlayerInput = FindObjectOfType<PlayerInput>();
#endif
        }
#endif // UNITY_EDITOR

        protected override void Awake()
        {
            base.Awake();
            if (Image == null)
            {
                Image = GetComponent<Image>();
            }
            _defaultSizeDelta = Image.rectTransform.sizeDelta;
            _texture = new Texture2D(2, 2);
        }

        protected override void Start()
        {
            base.Start();
            UpdateGlyphs(PlayerInput);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            _playerInputChangeDetector.ControlsChanged += UpdateGlyphs;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _playerInputChangeDetector.OnDisable();
            _playerInputChangeDetector.ControlsChanged -= UpdateGlyphs;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(_texture);
            _texture = null;
            Destroy(_createdSprite);
            _createdSprite = null;
            if (Image != null)
            {
                Image.sprite = null;
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

        private readonly List<InputDevice> _deviceBuffer = new();

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
                Image.sprite = _createdSprite;
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
            Image.sprite = _createdSprite;
        }

        //
        // ILayoutElement
        //

        [SerializeField]
        public bool EnableLayoutElement = true;

        [SerializeField]
        public int LayoutElementPriority = 1;

        [SerializeField]
        public float LayoutElementSize = 100f;

        public virtual int layoutPriority => EnableLayoutElement ? LayoutElementPriority : -1;

        public virtual void CalculateLayoutInputHorizontal() { }

        public virtual void CalculateLayoutInputVertical() { }

        public virtual float minWidth => -1;

        public virtual float minHeight => -1;

        public virtual float preferredWidth
        {
            get
            {
                if (Image == null || Image.sprite == null)
                {
                    return LayoutElementSize;
                }

                var ratio = (float)Image.sprite.rect.width / Image.sprite.rect.height;
                return LayoutElementSize * ratio;
            }
        }

        public virtual float preferredHeight => LayoutElementSize;

        public virtual float flexibleWidth => -1;

        public virtual float flexibleHeight => -1;

        protected void SetDirty()
        {
            if (!IsActive())
                return;
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
    }
}
#endif
