Input Glyphs Documentation

Here are the online documents with images that support other languages.
https://eviltwo.github.io/InputGlyphs_Docs/




# InputGlyphs

InputGlyphs is a package for displaying glyph images (icons) of the buttons on input devices detected by Unity's InputSystem.
It is easy to integrate and designed to allow for extension of devices and glyph images.

The glyph images in the game will automatically switch according to the device in use.

The package includes pre-configured glyph images for keyboard & mouse and gamepads, but you can add or change your own glyph images or use the glyphs provided by Steamworks.




# Getting started

## Set up the Input System
- Please import and enable the Input System according to UnityÅfs official documentation.
  https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/Installation.html
- Use Player Input events for character movement in the game.
  https://docs.unity3d.com/Packages/com.unity.inputsystem@1.4/manual/Components.html
  - For the Behavior of Player Input, you need to select either Invoke C Sharp Events or Invoke Unity Events.

## Place the Initialization Object
Place the InputGlyphs/Prefabs/InputGlyphsSetup prefab in the first scene.

## Draw Glyph Images

### Sprite Renderer
- Attach the Sprite Renderer component to any object.
- Attach the Input Glyph Sprite component to the same object.
  - Assign the player you want to display in the Player Input field.
  - Assign the action you want to display in the Input Action Reference field.

### UI Image
- Attach the UI Image component to any object within the Canvas.
- Attach the Input Glyph Image component to the same object.
  - Assign the player you want to display in the Player Input field.
  - Assign the action you want to display in the Input Action Reference field.

### Text Mesh Pro
- Attach the Text Mesh Pro - UI component to any object within the Canvas.
- Attach the Input Glyph Text component to the same object.
  - Assign the player you want to display in the Player Input field.
  - Assign the action you want to display in the Input Action References field.
- Write tags like <sprite name=ActionName> in the Text Mesh Pro component. The tag will be replaced with the Glyph image.

## Play the Game
When you play the game, the glyph images will be displayed. If they are not displayed, check the error log.
