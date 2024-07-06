# Changelog

## [0.7.0] - 2024-07-06
### Added
- Added duo player sample scene.
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
