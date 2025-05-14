#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System.Collections.Generic;
using System.Linq;
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

        private Vector2 _defaultSizeDelta;
        private PlayerInput _lastPlayerInput;
        private List<string> _pathBuffer = new List<string>();
        private Texture2D _texture;

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
            if (PlayerInput == null && InputGlyphDisplaySettings.AutoCollectPlayerInput)
            {
                PlayerInput = PlayerInput.all.FirstOrDefault();
            }
            if (PlayerInput == null)
            {
                Debug.LogWarning("PlayerInput is not set.", this);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (_lastPlayerInput != null)
            {
                UnregisterPlayerInputEvents(_lastPlayerInput);
                _lastPlayerInput = null;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Destroy(_texture);
            _texture = null;
            if (Image != null)
            {
                Destroy(Image.sprite);
                Image.sprite = null;
            }
        }

        protected virtual void Update()
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
                    Destroy(Image.sprite);
                    Image.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
                }
            }
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
