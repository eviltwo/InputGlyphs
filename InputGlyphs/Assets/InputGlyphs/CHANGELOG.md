# Changelog

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
