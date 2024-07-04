#if ENABLE_INPUT_SYSTEM && SUPPORT_TMPRO
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InputGlyphs;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore;

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

    private List<string> _lastDevices = new List<string>();
    private string _lastScheme;
    private List<string> _pathBuffer = new List<string>();
    private List<Tuple<string, Texture2D>> _actionTextures = new List<Tuple<string, Texture2D>>();
    private List<Texture2D> _copiedTextureBuffer = new List<Texture2D>();

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
    }

    private void Update()
    {
        if (PlayerInput == null)
        {
            Debug.LogError("PlayerInput is not set.");
            return;
        }

        var shouldUpdateGlyph = false;
        var devices = PlayerInput.devices;
        if (!IsSameDevices(devices, _lastDevices))
        {
            shouldUpdateGlyph = true;
            _lastDevices.Clear();
            _lastDevices.AddRange(devices.Select(device => device.name));
        }

        var scheme = PlayerInput.currentControlScheme;
        if (scheme != _lastScheme)
        {
            shouldUpdateGlyph = true;
            _lastScheme = scheme;
        }

        if (!shouldUpdateGlyph)
        {
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

    private static bool IsSameDevices(IReadOnlyList<InputDevice> devices, IReadOnlyList<string> deviceNames)
    {
        if (devices.Count != deviceNames.Count)
        {
            return false;
        }

        for (var i = 0; i < devices.Count; i++)
        {
            if (!deviceNames.Contains(devices[i].name))
            {
                return false;
            }
        }

        return true;
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

        var texturePack = new Texture2D(PackedTextureSize, PackedTextureSize);
        var rects = texturePack.PackTextures(targetTextures, 0, 2048, false);

        for (var i = 0; i < _copiedTextureBuffer.Count; i++)
        {
            Destroy(_copiedTextureBuffer[i]);
        }
        _copiedTextureBuffer.Clear();

        var material = new Material(Material);
        material.SetTexture("_MainTex", texturePack);

        var spriteAsset = ScriptableObject.CreateInstance<TMP_SpriteAsset>();
        SetSpriteAssetVersion(spriteAsset, "1.1.0");
        spriteAsset.spriteSheet = texturePack;
        spriteAsset.material = material;
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
#endif
