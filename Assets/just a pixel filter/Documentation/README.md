# just a pixel filter for URP

## Overview

just a pixel filter is a highly customizable pixelization post-processing effect for Unity's Universal Render Pipeline (URP). Built using the modern Render Graph system, it offers precise control over pixel size, aspect ratio, color processing (including palette mapping and posterization), and dithering options. Achieve retro aesthetics, stylized looks, or unique visual effects with ease and performance in mind.

## Features

* **Flexible Sizing:** Control pixel size via fixed blocks, target resolution (width or height), or downsampling.
* **Aspect Ratio Control:** Maintain square pixels, stretch to fit, or define a custom aspect ratio.
* **Sampling Modes:** Choose between Top-Left, Center, or Average sampling for pixel block color determination.
* **Color Palette Mapping:** Apply custom 1D LUT textures to remap colors based on luminance.
* **Posterization:** Reduce the number of color levels per channel.
* **Dithering:** Add Ordered (Bayer or Custom Pattern) or White Noise dithering to break up color banding.
* **Performance Options:** Downsampling factor to improve performance on lower-end hardware.
* **Easy Integration:** Simple setup via URP Renderer Feature and Volume Components.
* **Included Dither Patterns:** Built-in Bayer 2x2, 4x4, and 8x8 patterns included.

## Setup Guide

1.  **Add Renderer Feature:**
    * Find your active URP **Renderer** asset. You can locate it via `Project Settings > Graphics > Scriptable Render Pipeline Settings` (select your URP Asset) > `Renderer List`. Click the Renderer asset shown there.
    * In the Inspector for the Renderer asset, click `Add Renderer Feature`.
    * Select `Pixelization Renderer Feature` from the list.
2.  **Verify Shader Assignment:**
    * In the newly added feature's settings, ensure the `Pixelization Shader` field is assigned to the correct shader (`Hidden/just a pixel filter/Pixelization`). It should usually assign automatically.
3.  **Add Volume Component:**
    * In your scene, select a GameObject with a `Volume` component, or add a new `Volume` component to a GameObject (e.g., alongside your main camera or a dedicated global volume object).
    * Ensure the Volume has a `Volume Profile` asset assigned. Create one if needed (`Assets > Create > Volume > Profile`).
    * Select the Volume Profile asset.
    * In the Inspector, click `Add Override > Post-processing > just a pixel filter`.
4.  **Enable & Configure:**
    * In the Volume component override you just added, tick the main checkbox to enable the effect.
    * Tick the individual checkboxes next to each parameter you wish to control (e.g., `Pixel Block Size`, `Enable Palette`).
    * Adjust the parameter values to achieve your desired look.

## Configuration Parameters

These parameters are adjusted on the `Pixelization` Volume Component override:

### Pixelization Sizing

* **Sizing Mode:** (`FixedBlockSize`, `TargetWidth`, `TargetHeight`)
    * `FixedBlockSize`: Pixels are defined by `Pixel Block Size`.
    * `TargetWidth`: Calculates block size to achieve the `Target Resolution` horizontally.
    * `TargetHeight`: Calculates block size to achieve the `Target Resolution` vertically.
* **Pixel Block Size:** (Int, 1-256) Size (in screen pixels) of each square pixel block when `Sizing Mode` is `FixedBlockSize`. Larger values = bigger pixels.
* **Target Resolution:** (Int, 16-4096) The desired horizontal or vertical resolution when `Sizing Mode` is `TargetWidth` or `TargetHeight`.
* **Pixel Aspect Mode:** (`Square`, `Stretch`, `Custom`)
    * `Square`: Pixel blocks are always square.
    * `Stretch`: Pixel blocks stretch to match the screen aspect ratio based on the `Sizing Mode` axis.
    * `Custom`: Pixel block width is determined by `Custom Pixel Aspect Ratio` relative to its height.
* **Custom Pixel Aspect Ratio:** (Float) Width/Height ratio for pixel blocks when `Pixel Aspect Mode` is `Custom`. `1.0` is square.

### Pixelization Style

* **Downsample Factor:** (Int, 1-8) Reduces the internal render texture resolution *before* pixelization (1 = Off). Higher values improve performance significantly but reduce detail.
* **Sampling Mode:** (`TopLeft`, `Center`, `Average`)
    * `TopLeft`: Samples the top-left corner of the area covered by the pixel block. Fastest.
    * `Center`: Samples the center of the area covered by the pixel block.
    * `Average`: Calculates the average color of all screen pixels within the block area. **Can be performance-intensive.**

### Color Processing

* **Posterize Levels:** (Int, 2-256) Reduces the number of distinct color levels per RGB channel (256 = Off). Creates a banded color effect.
* **Enable Palette:** (Bool) Enables the Color Palette Mapping feature below.
* **Luminance Mode:** (`Linear`, `Gamma`) How luminance is calculated for palette lookup when `Enable Palette` is true. `Linear` is generally preferred for accuracy.
* **Palette Texture:** (Texture) A 1D Texture (LUT) used to map colors based on luminance. Requires `Enable Palette`. See 'Creating Custom Palettes'.
* **Palette Blend:** (Float, 0-1) Blends between the original pixel color (0.0) and the palette-mapped color (1.0). Requires `Enable Palette`.

### Dithering

* **Dithering Mode:** (`Off`, `OrderedAdditive`, `WhiteNoise`) Selects the dithering algorithm.
    * `Off`: No dithering.
    * `OrderedAdditive`: Adds a value from a repeating pattern (Bayer or Custom) before color processing. Helps break up banding.
    * `WhiteNoise`: Adds simple pseudo-random noise aligned with pixel blocks.
* **Dither Pattern Type:** (`Custom`, `Bayer2x2`, `Bayer4x4`, `Bayer8x8`) Selects the pattern source when `Dithering Mode` is `OrderedAdditive`. `Bayer` options use included textures.
* **Dither Pattern Texture:** (Texture) Assign a custom tileable grayscale pattern texture when `Dither Pattern Type` is `Custom`. See 'Creating Custom Dither Patterns'.
* **Dither Pattern Tiling:** (Float) Controls how many times the dither pattern tiles across the screen. Used only for `OrderedAdditive` mode.
* **Dither Intensity:** (Float, 0-1) Strength of the dithering effect (0 = Off).

## Creating Custom Palettes

To use the Palette Mapping feature, you need a 1D Look-Up Texture (LUT).

1.  **Create a Gradient:** In an image editor, create a thin image representing your desired color palette (e.g., 256 pixels wide, 1 pixel high). The gradient should typically map from dark (left) to bright (right).
2.  **Import Settings:** Import this texture into Unity with the following settings:
    * **Texture Type:** Default
    * **Wrap Mode:** Clamp
    * **Filter Mode:** Point (No Filter)
    * **Compression:** None (or a lossless format)
    * Ensure **sRGB (Color Texture)** is checked appropriately based on how you created the gradient (usually checked).
3.  **Assign:** Assign to the `Dither Pattern Texture` slot.

## Performance Considerations

* **Downsample Factor:** The single most effective way to improve performance. A value of `2` quarters the number of pixels processed, `4` reduces it to 1/16th. Experiment to find the balance between quality and speed.
* **Sampling Mode:** `Average` is significantly slower than `TopLeft` or `Center`. Avoid `Average` if performance is critical.
* **Other Features:** Palette mapping and dithering generally have a low performance impact.

## Troubleshooting

* **Effect Not Visible:**
    * Verify all steps in the **Setup Guide** were followed correctly.
    * Ensure the main `Pixelization` override checkbox AND the individual parameter checkboxes in the Volume Profile are ticked.
    * Check the Console window (`Ctrl+Shift+C`) for any errors related to the asset or URP.
    * Confirm "Post Processing" is enabled on your Camera component settings.
    * Ensure the Volume layer matches the Camera's Volume Mask, or use a Global Volume.
* **Palette/Dither Not Working:**
    * Ensure `Enable Palette` or the correct `Dithering Mode` is selected.
    * Ensure a valid `Palette Texture` or `Dither Pattern Texture` (if using Custom) is assigned. Check texture import settings (see above).
    * Make sure `Palette Blend` or `Dither Intensity` is greater than 0.
* **Built-in Dither Patterns Missing:** Ensure the asset was imported correctly and the pattern textures (`Bayer2x2`, `Bayer4x4`, `Bayer8x8`) are present in the Resources folder (`/Runtime/Resources/DitherPatterns/`).

## Demo

To explore the capabilities of just a pixel filter:

1. **Open the Demo Scene:** Navigate to `Assets/just a pixel filter/Example/Demo` and open it.
2. **Enter Play Mode:** Click the Play button in the Unity Editor.
3. **Explore Example Profiles:** The scene contains 9 pre-configured volume profiles demonstrating different pixelization effects.
4. **Switch Between Examples:** Click the buttons on the right side of the screen to switch between different profile examples. Each demonstrates different combinations of features and settings.
5. **Examine Settings:** After testing, you can inspect each Volume Profile in the Project window to see how the effects were configured.


## Support

* Email: olszewskidamian1110@gmail.com