#if ENABLE_INPUT_SYSTEM && SUPPORT_TMPRO
using System;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

namespace InputGlyphs.Display
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextInputGlyph : MonoBehaviour
    {
        public static int PackedTextureSize = 2048;

        [SerializeField, HideInInspector]
        public TextMeshProUGUI Text = null;

        [SerializeField, HideInInspector]
        public Material Material = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference[] InputActionReferences = null;

        private PlayerInput _lastPlayerInput;
        private List<string> _pathBuffer = new List<string>();
        private List<Tuple<string, Texture2D>> _actionTextures = new List<Tuple<string, Texture2D>>();
        private List<Texture2D> _copiedTextureBuffer = new List<Texture2D>();
        private Material _sharedMaterial;

        private void Reset()
        {
            Text = GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            if (Text == null)
            {
                Text = GetComponent<TextMeshProUGUI>();
            }
            _sharedMaterial = new Material(Material);
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
            Destroy(_sharedMaterial);
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

            _actionTextures.Clear();
            for (var i = 0; i < InputActionReferences.Length; i++)
            {
                var actionReference = InputActionReferences[i];
                if (TryGetInputPaths(InputActionReferences[i], PlayerInput, _pathBuffer))
                {
                    var texture = InputGlyphManager.GetGlyph(devices, _pathBuffer[0]);
                    if (texture == null)
                    {
                        Debug.LogError($"Failed to get glyph for input path: {_pathBuffer[0]}");
                        texture = Texture2D.whiteTexture;
                    }
                    _actionTextures.Add(Tuple.Create(actionReference.action.name, texture));
                }
            }
            SetGlyphsToSpriteAsset(_actionTextures);
        }

        private static bool TryGetInputPaths(InputActionReference actionReferences, PlayerInput playerInput, List<string> results)
        {
            results.Clear();
            var action = actionReferences?.action;
            if (action == null)
            {
                return false;
            }
            // TODO: Get multiple bindings
            var bindingIndex = action.GetBindingIndex(InputBinding.MaskByGroup(playerInput.currentControlScheme));
            if (bindingIndex < 0)
            {
                return false;
            }
            results.Add(action.bindings[bindingIndex].effectivePath);
            return true;
        }

        private void SetGlyphsToSpriteAsset(IReadOnlyList<Tuple<string, Texture2D>> actionTextures)
        {
            // Copy to readable textures
            var targetTextures = new Texture2D[actionTextures.Count];
            _copiedTextureBuffer.Clear();
            for (var i = 0; i < actionTextures.Count; i++)
            {
                var sourceTexture = actionTextures[i].Item2;
                if (sourceTexture.isReadable)
                {
                    targetTextures[i] = sourceTexture;
                }
                else
                {
                    var copiedTexture = new Texture2D(sourceTexture.width, sourceTexture.height, sourceTexture.format, true);
                    Graphics.CopyTexture(sourceTexture, copiedTexture);
                    targetTextures[i] = copiedTexture;
                    _copiedTextureBuffer.Add(copiedTexture);
                }
            }

            // Pack textures
            var texturePack = new Texture2D(PackedTextureSize, PackedTextureSize);
            var rects = texturePack.PackTextures(targetTextures, 0, 2048, false);

            // Destroy copied readable textures
            for (var i = 0; i < _copiedTextureBuffer.Count; i++)
            {
                Destroy(_copiedTextureBuffer[i]);
            }
            _copiedTextureBuffer.Clear();

            // Setup material
            _sharedMaterial.SetTexture("_MainTex", texturePack);

            // Create sprite asset for TextMeshPro
            var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            SetSpriteAssetVersion(spriteAsset, "1.1.0"); // Preventing processing for older versions from occurring
            spriteAsset.spriteSheet = texturePack;
            spriteAsset.material = _sharedMaterial;
            spriteAsset.spriteInfoList = new List<TMP_Sprite>();
            for (var i = 0; i < rects.Length; i++)
            {
                var rect = rects[i];

                // Create glyph
                var glyphMetrics = new GlyphMetrics(
                    texturePack.width * rect.width,
                    texturePack.height * rect.height,
                    0,
                    texturePack.height * rect.height * 0.8f,
                    texturePack.width * rect.width);
                var glyphRect = new GlyphRect(
                    Mathf.FloorToInt(texturePack.width * rect.xMin),
                    Mathf.FloorToInt(texturePack.height * rect.yMin),
                    Mathf.FloorToInt(texturePack.width * rect.width),
                    Mathf.FloorToInt(texturePack.height * rect.height));
                var spriteGlyph = new TMP_SpriteGlyph((uint)i, glyphMetrics, glyphRect, 1, i);
                spriteAsset.spriteGlyphTable.Add(spriteGlyph);

                // Create character
                var glyphCharacter = new TMP_SpriteCharacter(0, spriteGlyph);
                glyphCharacter.name = $"input_{actionTextures[i].Item1}";
                spriteAsset.spriteCharacterTable.Add(glyphCharacter);
            }
            spriteAsset.UpdateLookupTables();
            Text.spriteAsset = spriteAsset;
        }

        private static void SetSpriteAssetVersion(TMP_SpriteAsset spriteAsset, string version)
        {
            var fieldInfo = typeof(TMP_SpriteAsset).GetField("m_Version", BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(spriteAsset, version);
            }
        }
    }
}
#endif
