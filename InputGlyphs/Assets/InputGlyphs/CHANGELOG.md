# Changelog

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
