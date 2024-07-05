# Changelog

## [0.6.0] - 2024-07-05
### Changed
- Change GetGlyphs() function. Texture2D GetGlyphs(devices, path) to bool LoadGlyphs(texture, devices, path)
  - Fixed memory allocations and leaks caused by regenerating textures.

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
