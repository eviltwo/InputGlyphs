#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using InputGlyphs.Display;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace InputGlyphs.Samples
{
    [AddComponentMenu("")] // Disable the script from the Add Component menu
    public class InputCheckSample : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer = null;

        private IDisposable _callOnce;

        private Texture2D _texture;
        private List<InputControl> _controlBuffer = new List<InputControl>();
        private List<InputDevice> _deviceBuffer = new List<InputDevice>();
        private List<string> _pathBuffer = new List<string>();
        private readonly GlyphsLayoutData _layoutData = new GlyphsLayoutData
        {
            Layout = GlyphsLayout.Horizontal,
            MaxCount = 4,
        };

        private void Start()
        {
            _callOnce = InputSystem.onEvent
                .Where(e => e.HasButtonPress())
                .Call(eventPtr =>
                {
                    _controlBuffer.Clear();
                    _controlBuffer.AddRange(eventPtr.GetAllButtonPresses());
                    DrawGlyphs(_controlBuffer);
                });

            _texture = new Texture2D(2, 2);
        }

        private void DrawGlyphs(IReadOnlyList<InputControl> controls)
        {
            _deviceBuffer.Clear();
            _pathBuffer.Clear();
            for (int i = 0; i < controls.Count; i++)
            {
                var control = controls[i];
                var device = control.device;
                if (!_deviceBuffer.Contains(device))
                {
                    _deviceBuffer.Add(device);
                }

                _pathBuffer.Add(control.path);
            }

            if (DisplayGlyphTextureGenerator.GenerateGlyphTexture(_texture, _deviceBuffer, _pathBuffer, _layoutData))
            {
                Destroy(_spriteRenderer.sprite);
                _spriteRenderer.sprite = Sprite.Create(_texture, new Rect(0, 0, _texture.width, _texture.height), new Vector2(0.5f, 0.5f), Mathf.Min(_texture.width, _texture.height));
            }
        }

        private void OnDestroy()
        {
            _callOnce.Dispose();
            _callOnce = null;
            Destroy(_texture);
            _texture = null;
            if (_spriteRenderer != null)
            {
                Destroy(_spriteRenderer.sprite);
                _spriteRenderer.sprite = null;
            }
        }
    }
}

#endif
