# Changelog

## [1.2.5] - 2025-05-14
### Added
- Added sample scene for rebinding.
### Fixed
- Fixed an issue where the UI would not update when rebinding buttons on a cloned PlayerInput.actions.
- Made the UpdateGlyphs() function of the UI component public.

## [1.2.4] - 2025-02-08
### Fixed
- Fixed the error related to FindAnyObjectByType in older Unity versions.

## [1.2.3] - 2024-12-27
### Fixed
- Fixed errors during Linux build. (Switch Pro Controller, DualSense Controller)
- Supported actions in the "Usage" category. (including input layout paths with wildcards)

## [1.2.2] - 2024-12-10
### Modified
- Supported `DISABLESTEAMWORKS` Assembly Definition.
- Changed to use the parent path if the image cannot be found.
  - Example: `leftStick/x` -> `leftStick`

## [1.2.1] - 2024-10-31
### Modified
- Modified to automatically collect PlayerInput when it is not set.
- Modified the InputGlyphsText component to work with Unity 6.

## [1.2.0] - 2024-09-08
### Added
- Added the Index value for Single Layout.
### Changed
- Changed the Image to be transformed using LayoutElement.
### Modified
- Modified to allow multiple components to be edited.

## [1.1.2] - 2024-09-02
### Added
- Added shift key to texture map.

## [1.1.1] - 2024-07-24
### Changed
- Changed the glyph image generation to use GlyphLayoutData struct instead of GlyphLayout.

## [1.1.0] - 2024-07-16
### Changed
- [GlyphLoader](https://github.com/eviltwo/UnitySteamInputGlyphLoader) package has been integrated into this package.
  - Users no longer need to import the GlyphLoader package.
### Fixed
- Fixed the issue where an error was output when adding a component from the script.

## [1.0.2] - 2024-07-16
### Updated
- Updated SteamInputAdapter package version to 1.0.1
  - Supported control components. Example: "XInputController/{Submit}"
### Changed
- Renamed `GetLocalPath()` to `RemoveRoot()`. Internal processing was also changed.

## [1.0.1] - 2024-07-13
### Updated
- Updated SteamInputAdapter package version to 1.0.0

## [1.0.0] - 2024-07-10
- Stable Release Version

## [0.7.4] - 2024-07-09
### Fixed
- Supports Text Mesh Pro Text(3D).

## [0.7.3] - 2024-07-07
### Changed
- Added version to URLs importing packages such as steamworks.

## [0.7.2] - 2024-07-07
### Fixed
- Fix glyphs not display in built game

## [0.7.1] - 2024-07-06
### Added
- Added text tag hint.
### Changed
- Remove prefix input_ from text tag. input_Move -> Move

## [0.7.0] - 2024-07-06
### Added
- Added duo player sample scene.
- Added input check sample scene.
### Changed
- Renamed display classes.
- Changed directory of solo player sample scene.

## [0.6.2] - 2024-07-06
### Added
- Added glyph texture generator.
- Added GlyphLayout
### Changed
- Changed glyph display classes to use texture generator. 

## [0.6.1] - 2024-07-06
### Added
- Added glyph display for UI Image.
- Added glyph display for Sprite Renderer.

## [0.6.0] - 2024-07-05
### Changed
- Change GetGlyph() to LoadGlyph().
  - Texture2D GetGlyph(devices, path) -> bool LoadGlyph(texture, devices, path)
  - Fixed memory allocations and leaks caused by regenerating textures.
- Update and support latest SteamInputGlyphLoader package.

## [0.5.2] - 2024-07-05
### Fixed
- Fix detection player input changes in TextGlyph
- Fix texture memory allocation in TextGlyph

## [0.5.1] - 2024-07-05
### Added
- Support steam input glyph

## [0.5.0] - 2024-07-05
### Added
- Glyph manager
- Glyph loaders
