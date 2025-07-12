using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace justA.PixelFilter
{
    // --- Enums ---
    public enum PixelSamplingMode { TopLeft, Center, Average }
    public enum LuminanceCalculationMode { Linear, Gamma }
    public enum DitheringMode { Off, OrderedAdditive, WhiteNoise }
    public enum DitherPatternType { Custom, Bayer2x2, Bayer4x4, Bayer8x8 }
    public enum SizingMode { FixedBlockSize, TargetWidth, TargetHeight }
    public enum PixelAspectMode { Square, Stretch, Custom }

    [VolumeComponentMenu("Post-processing/just a pixel filter")]
    [Serializable]
    public sealed class Pixelization : VolumeComponent, IPostProcessComponent
    {
        [Tooltip("Enable this override to activate the Pixelization effect.")]
        public BoolParameter overrideEnable = new BoolParameter(false, false);

        [Header("Pixelization Sizing")]
        [Tooltip("Determines how the pixel size is calculated.")]
        public VolumeParameter<SizingMode> sizingMode = new VolumeParameter<SizingMode>() { value = SizingMode.FixedBlockSize };

        [Tooltip("The fixed size (in screen pixels) for each block when Sizing Mode is FixedBlockSize.")]
        public ClampedIntParameter pixelBlockSize = new ClampedIntParameter(value: 8, min: 1, max: 256, overrideState: true);

        [Tooltip("The target horizontal or vertical resolution when Sizing Mode is TargetWidth or TargetHeight.")]
        public ClampedIntParameter targetResolution = new ClampedIntParameter(value: 320, min: 16, max: 4096, overrideState: true);

        [Header("Pixelization Style")]
        [Tooltip("Downsampling factor applied before pixelization (1 = Off). Improves performance.")]
        public ClampedIntParameter downsampleFactor = new ClampedIntParameter(value: 1, min: 1, max: 8, overrideState: true);
        [Tooltip("Method used to determine the color of a pixel block.")]
        public VolumeParameter<PixelSamplingMode> samplingMode = new VolumeParameter<PixelSamplingMode>() { value = PixelSamplingMode.TopLeft };
        [Tooltip("Controls the aspect ratio of the pixel blocks.")]
        public VolumeParameter<PixelAspectMode> pixelAspectMode = new VolumeParameter<PixelAspectMode>() { value = PixelAspectMode.Square };
        [Tooltip("Custom aspect ratio (Width/Height) for pixel blocks when Aspect Mode is Custom.")]
        public FloatParameter customPixelAspectRatio = new FloatParameter(value: 1.0f, overrideState: true);


        [Header("Color Processing")]
        [Tooltip("Reduces the number of distinct color levels per channel (256 = Off).")]
        public ClampedIntParameter posterizeLevels = new ClampedIntParameter(value: 256, min: 2, max: 256, overrideState: true);
        [Tooltip("Enable the Color Palette feature.")]
        public BoolParameter enablePalette = new BoolParameter(false, overrideState: true);
        [Tooltip("How luminance is calculated for palette lookup.")]
        public VolumeParameter<LuminanceCalculationMode> luminanceMode = new VolumeParameter<LuminanceCalculationMode>() { value = LuminanceCalculationMode.Linear };
        [Tooltip("1D Texture LUT used to map colors based on luminance.")]
        public TextureParameter paletteTexture = new TextureParameter(null, overrideState: true);
        [Tooltip("Blending factor between the original color and the palette color.")]
        public ClampedFloatParameter paletteBlend = new ClampedFloatParameter(value: 1f, min: 0f, max: 1f, overrideState: true);

        [Header("Dithering")]
        [Tooltip("Selects the dithering algorithm.")]
        public VolumeParameter<DitheringMode> ditheringMode = new VolumeParameter<DitheringMode>() { value = DitheringMode.Off };
        [Tooltip("Selects the pattern texture source (Used for Ordered Additive mode).")]
        public VolumeParameter<DitherPatternType> ditherPatternType = new VolumeParameter<DitherPatternType>() { value = DitherPatternType.Custom };
        [Tooltip("Assign a custom tileable pattern texture (Used if Pattern Type is Custom).")]
        public TextureParameter ditherPatternTexture = new TextureParameter(null, overrideState: true);
        [Tooltip("Controls pattern tiling scale (Used only for Ordered Additive mode).")]
        public FloatParameter ditherPatternTiling = new FloatParameter(value: 4.0f, overrideState: true);
        [Tooltip("Strength of the dithering effect (0 = Off).")]
        public ClampedFloatParameter ditherIntensity = new ClampedFloatParameter(value: 0.5f, min: 0f, max: 1f, overrideState: true);

        public bool IsActive() => overrideEnable.value &&
                                 (sizingMode.value == SizingMode.FixedBlockSize ? pixelBlockSize.value > 1 : targetResolution.value > 0);

        public bool IsTileCompatible() => true;

        internal bool IsPaletteEffectActive() => IsActive() && enablePalette.value && paletteTexture.value != null && paletteBlend.value > 1e-3f;
        internal bool IsPosterizeActive() => IsActive() && posterizeLevels.value < 256;
        internal bool IsDitheringActive() => IsActive() && ditheringMode.value != DitheringMode.Off && ditherIntensity.value > 1e-3f;
        internal bool HasValidDitherPatternSource() => ditherPatternType.value == DitherPatternType.Custom ? ditherPatternTexture.value != null : true;
    }
}