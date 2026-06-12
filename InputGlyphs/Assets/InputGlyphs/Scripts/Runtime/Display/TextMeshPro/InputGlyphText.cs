#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM && SUPPORT_TMPRO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputGlyphs.Attributes;
using InputGlyphs.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.TextCore;

namespace InputGlyphs.Display
{
    public class InputGlyphText : MonoBehaviour
    {
        [SerializeField]
        public TMP_Text Text = null;

        [SerializeField, HideInInspector]
        public Material Material = null;

        [SerializeField]
        public PlayerInput PlayerInput = null;

        [SerializeField]
        public InputActionReference[] InputActionReferences = null;

        [SerializeField]
        public GlyphsLayoutData GlyphsLayoutData = GlyphsLayoutData.Default;

        [SerializeField, ControlSchemeName]
        public string ControlScheme;

        private readonly PlayerInputChangeDetector _playerInputChangeDetector = new ();
        private List<string> _pathBuffer = new List<string>();
        private List<Texture2D> _actionTextureBuffer = new List<Texture2D>();
        private List<Tuple<string, int>> _actionTextureIndexes = new List<Tuple<string, int>>();
        private List<Texture2D> _copiedTextureBuffer = new List<Texture2D>();
        private Texture2D _packedTexture;
        private Material _sharedMaterial;
        private TMP_SpriteAsset _sharedSpriteAsset;

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Text = GetComponent<TMP_Text>();
#if UNITY_2022_3_OR_NEWER
            PlayerInput = FindAnyObjectByType<PlayerInput>();
#else
            PlayerInput = FindObjectOfType<PlayerInput>();
#endif
        }
#endif // UNITY_EDITOR

        protected virtual void Awake()
        {
            if (Text == null)
            {
                Text = GetComponent<TMP_Text>();
            }
            _packedTexture = new Texture2D(2, 2);
            _sharedMaterial = new Material(Material);
            _sharedMaterial.SetTexture("_MainTex", _packedTexture);
            _sharedSpriteAsset = CreateEmptySpriteAsset();
            _sharedSpriteAsset.material = _sharedMaterial;
            _sharedSpriteAsset.spriteSheet = _packedTexture;
            Text.spriteAsset = _sharedSpriteAsset;
        }

        protected void Start()
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
            for (var i = 0; i < _actionTextureBuffer.Count; i++)
            {
                Destroy(_actionTextureBuffer[i]);
            }
            _actionTextureBuffer.Clear();
            Destroy(_packedTexture);
            _packedTexture = null;
            Destroy(_sharedMaterial);
            _sharedMaterial = null;
            Destroy(_sharedSpriteAsset);
            _sharedSpriteAsset = null;
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
            Profiler.BeginSample("UpdateGlyphs");

            _actionTextureIndexes.Clear();
            var assignedTextureCount = 0;
            for (var i = 0; i < InputActionReferences.Length; i++)
            {
                var actionReference = InputActionReferences[i];
                if (actionReference == null || actionReference.action == null)
                {
                    Debug.LogWarning("InputActionReference is not set.", this);
                    return;
                }
                
                Texture2D texture;
                var textureIndex = assignedTextureCount;
                if (textureIndex < _actionTextureBuffer.Count)
                {
                    texture = _actionTextureBuffer[textureIndex];
                }
                else
                {
                    texture = new Texture2D(2, 2);
                    _actionTextureBuffer.Add(texture);
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
                    DisplayUtils.CollectDevicesForControlScheme(actionReference.action.actionMap.controlSchemes.FirstOrDefault(v => v.name == ControlScheme), _deviceBuffer, playerInput);
                    controlScheme = ControlScheme;
                }

                var playerInputAction = playerInput?.actions.FindAction(actionReference.action.id) ?? actionReference.action;
                if (InputLayoutPathUtility.TryGetActionBindingPath(playerInputAction, controlScheme, _pathBuffer)
                    && DisplayGlyphTextureGenerator.GenerateGlyphTexture(texture, _deviceBuffer, _pathBuffer, GlyphsLayoutData))
                {
                    // Glyph texture generation succeeded; the texture is updated in place.
                }
                else
                {
                    HandleGlyphTextureGenerationFailed(texture);
                }
                
                _actionTextureIndexes.Add(Tuple.Create(playerInputAction.name, textureIndex));
                assignedTextureCount++;
            }
            
            SetGlyphsToSpriteAsset(_actionTextureBuffer, _actionTextureIndexes);

            Profiler.EndSample();
        }
        
        protected virtual void HandleGlyphTextureGenerationFailed(Texture2D texture)
        {
            texture.Reinitialize(8, 8);
        }

        private void SetGlyphsToSpriteAsset(IReadOnlyList<Texture2D> actionTextures, IReadOnlyList<Tuple<string, int>> actionTextureIndexes)
        {
            Profiler.BeginSample("SetGlyphsToSpriteAsset");

            // Copy to readable textures
            var targetTextures = new Texture2D[actionTextures.Count];
            _copiedTextureBuffer.Clear();
            for (var i = 0; i < actionTextures.Count; i++)
            {
                var sourceTexture = actionTextures[i];
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
            var rects = _packedTexture.PackTextures(targetTextures, 0, 2048, false);

            // Destroy copied readable textures
            for (var i = 0; i < _copiedTextureBuffer.Count; i++)
            {
                Destroy(_copiedTextureBuffer[i]);
            }
            _copiedTextureBuffer.Clear();

            // Create sprite asset for TextMeshPro
            _sharedSpriteAsset.spriteGlyphTable.Clear();
            _sharedSpriteAsset.spriteCharacterTable.Clear();
            for (var i = 0; i < actionTextureIndexes.Count; i++)
            {
                var actionTextureIndex = actionTextureIndexes[i];
                var rect = rects[actionTextureIndex.Item2];

                // Create glyph
                var glyphMetrics = new GlyphMetrics(
                    _packedTexture.width * rect.width,
                    _packedTexture.height * rect.height,
                    0,
                    _packedTexture.height * rect.height * 0.8f,
                    _packedTexture.width * rect.width);
                var glyphRect = new GlyphRect(
                    Mathf.FloorToInt(_packedTexture.width * rect.xMin),
                    Mathf.FloorToInt(_packedTexture.height * rect.yMin),
                    Mathf.FloorToInt(_packedTexture.width * rect.width),
                    Mathf.FloorToInt(_packedTexture.height * rect.height));
                var spriteGlyph = new TMP_SpriteGlyph((uint)i, glyphMetrics, glyphRect, 1, i);
                _sharedSpriteAsset.spriteGlyphTable.Add(spriteGlyph);

                // Create character
                var glyphCharacter = new TMP_SpriteCharacter(0, spriteGlyph);
                glyphCharacter.name = actionTextureIndex.Item1;
                _sharedSpriteAsset.spriteCharacterTable.Add(glyphCharacter);
            }
            _sharedSpriteAsset.UpdateLookupTables();
            Text.SetAllDirty();

            Profiler.EndSample();
        }

        private static TMP_SpriteAsset CreateEmptySpriteAsset()
        {
            var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
            SetSpriteAssetVersion(spriteAsset, "1.1.0"); // Preventing processing for older versions from occurring
            return spriteAsset;
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
