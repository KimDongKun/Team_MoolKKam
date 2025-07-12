using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using System.Collections.Generic;

namespace justA.PixelFilter
{
    internal class PixelizationPass : ScriptableRenderPass
    {
        private readonly Material m_PixelizationMaterial;
        private readonly ProfilingSampler m_ProfilingSampler;
        private readonly Dictionary<DitherPatternType, Texture2D> m_BuiltInDitherPatterns;

        // Shader Uniforms
        private static readonly int s_EffectivePixelSizeID = Shader.PropertyToID("_EffectivePixelSize");
        private static readonly int s_SamplingModeID = Shader.PropertyToID("_SamplingMode");
        private static readonly int s_PaletteTexID = Shader.PropertyToID("_PaletteTex");
        private static readonly int s_PaletteBlendID = Shader.PropertyToID("_PaletteBlend");
        private static readonly int s_PosterizeLevelsID = Shader.PropertyToID("_PosterizeLevels");
        private static readonly int s_LuminanceModeID = Shader.PropertyToID("_LuminanceMode");
        private static readonly int s_DitherPatternTexID = Shader.PropertyToID("_DitherPatternTex");
        private static readonly int s_DitherPatternTilingID = Shader.PropertyToID("_DitherPatternTiling");
        private static readonly int s_DitherIntensityID = Shader.PropertyToID("_DitherIntensity");
        private static Vector4 s_identityScaleBias = new Vector4(1, 1, 0, 0);

        // Shader Keyword Names
        private static readonly string s_kwPaletteActiveName = "_PALETTE_ACTIVE";
        private static readonly string s_kwDitheringOrderedName = "_DITHERING_ORDERED_ADDITIVE";
        private static readonly string s_kwDitheringWhiteNoiseName = "_DITHERING_WHITE_NOISE";

        private Pixelization m_VolumeComponent;
        private TextureHandle m_SourceTextureHandle;

        // Render Graph Pass Data Definitions
        private class PixelizePassData { internal TextureHandle source; internal Material material; internal Pixelization volumeSettings; }
        private class BlitBackPassData { internal TextureHandle sourceToBlit; internal Material blitMaterial; }

        public PixelizationPass(Material pixelizationMaterial, Dictionary<DitherPatternType, Texture2D> builtInPatterns, string profilingName)
        {
            m_PixelizationMaterial = pixelizationMaterial;
            m_BuiltInDitherPatterns = builtInPatterns ?? new Dictionary<DitherPatternType, Texture2D>();
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            m_ProfilingSampler = new ProfilingSampler(profilingName);
        }

        private void SetMaterialKeyword(Material mat, string name, bool enabled)
        {
            if (mat == null) return;
            if (enabled) mat.EnableKeyword(name); else mat.DisableKeyword(name);
        }

        public bool Setup(ref RenderingData renderingData, Material material)
        {
            var stack = VolumeManager.instance.stack;
            m_VolumeComponent = stack.GetComponent<Pixelization>();

            if (m_VolumeComponent == null || !m_VolumeComponent.IsActive() || material == null)
            {
                 SetMaterialKeyword(material, s_kwPaletteActiveName, false);
                 SetMaterialKeyword(material, s_kwDitheringOrderedName, false);
                 SetMaterialKeyword(material, s_kwDitheringWhiteNoiseName, false);
                 return false;
            }
            return true;
        }


        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (m_PixelizationMaterial == null || m_VolumeComponent == null || !m_VolumeComponent.IsActive()) return;

            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            if (!cameraData.postProcessEnabled || (cameraData.cameraType != CameraType.Game && cameraData.cameraType != CameraType.SceneView)) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            m_SourceTextureHandle = resourceData.activeColorTexture;

            // Intermediate Texture Setup uses source texture size
            TextureDesc intermediateDesc = renderGraph.GetTextureDesc(m_SourceTextureHandle);
            intermediateDesc.name = "PixelFilterIntermediate"; intermediateDesc.depthBufferBits = 0;
            int downsample = Mathf.Max(1, m_VolumeComponent.downsampleFactor.value);
            intermediateDesc.width = Mathf.Max(1, intermediateDesc.width / downsample);
            intermediateDesc.height = Mathf.Max(1, intermediateDesc.height / downsample);
            TextureHandle intermediateHandle = renderGraph.CreateTexture(intermediateDesc);

            // Pass source texture description for size calculation in ConfigureMaterial
            ConfigureMaterial(m_PixelizationMaterial, m_VolumeComponent, intermediateDesc);

            // Pass 1: Render Effect
            using (var builder = renderGraph.AddRasterRenderPass<PixelizePassData>("PixelFilter Pass", out var passData, m_ProfilingSampler)) {
                passData.source = m_SourceTextureHandle;
                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(intermediateHandle, 0, AccessFlags.Write);
                passData.volumeSettings = m_VolumeComponent; passData.material = m_PixelizationMaterial;
                builder.SetRenderFunc((PixelizePassData data, RasterGraphContext context) => ExecutePixelizePass(data, context));
            }
            // Pass 2: Blit Back
            using (var builder = renderGraph.AddRasterRenderPass<BlitBackPassData>("Blit Back Pass", out var passData, m_ProfilingSampler)) {
                passData.sourceToBlit = intermediateHandle; builder.UseTexture(passData.sourceToBlit, AccessFlags.Read);
                passData.blitMaterial = Blitter.GetBlitMaterial(TextureDimension.Tex2D);
                builder.SetRenderAttachment(m_SourceTextureHandle, 0, AccessFlags.Write);
                builder.SetRenderFunc((BlitBackPassData data, RasterGraphContext context) => ExecuteBlitBackPass(data, context));
            }
        }

        private void ConfigureMaterial(Material material, Pixelization settings, TextureDesc sourceDesc)
        {
            if (material == null || settings == null) return;

            // Start with keywords OFF
            SetMaterialKeyword(material, s_kwPaletteActiveName, false);
            SetMaterialKeyword(material, s_kwDitheringOrderedName, false);
            SetMaterialKeyword(material, s_kwDitheringWhiteNoiseName, false);

            // --- Calculate Effective Pixel Size ---
            float sourceWidth = sourceDesc.width;
            float sourceHeight = sourceDesc.height;
            Vector2 effectivePixelSize = Vector2.one;

            if (sourceWidth > 0 && sourceHeight > 0)
            {
                if (settings.sizingMode.value == SizingMode.FixedBlockSize)
                {
                    effectivePixelSize.x = Mathf.Max(1f, settings.pixelBlockSize.value);
                    effectivePixelSize.y = Mathf.Max(1f, settings.pixelBlockSize.value);
                }
                else if (settings.sizingMode.value == SizingMode.TargetWidth)
                {
                    float size = sourceWidth / Mathf.Max(1f, settings.targetResolution.value);
                    effectivePixelSize.x = Mathf.Max(1f, size);
                    effectivePixelSize.y = Mathf.Max(1f, size);
                }
                else // TargetHeight
                {
                    float size = sourceHeight / Mathf.Max(1f, settings.targetResolution.value);
                    effectivePixelSize.x = Mathf.Max(1f, size);
                    effectivePixelSize.y = Mathf.Max(1f, size);
                }

                // --- Apply Aspect Ratio Correction ---
                if (settings.pixelAspectMode.value == PixelAspectMode.Stretch)
                {
                    if (settings.sizingMode.value == SizingMode.TargetWidth) {
                         effectivePixelSize.x = sourceWidth / Mathf.Max(1f, settings.targetResolution.value);
                         effectivePixelSize.y = effectivePixelSize.x * (sourceHeight / sourceWidth);
                    } else if (settings.sizingMode.value == SizingMode.TargetHeight) {
                         effectivePixelSize.y = sourceHeight / Mathf.Max(1f, settings.targetResolution.value);
                         effectivePixelSize.x = effectivePixelSize.y * (sourceWidth / sourceHeight);
                    }
                     effectivePixelSize.x = Mathf.Max(1f, effectivePixelSize.x);
                     effectivePixelSize.y = Mathf.Max(1f, effectivePixelSize.y);

                }
                else if (settings.pixelAspectMode.value == PixelAspectMode.Custom)
                {
                    float customAspect = Mathf.Max(0.01f, settings.customPixelAspectRatio.value);
                    // Adjust X based on Y size and custom aspect
                    effectivePixelSize.x = Mathf.Max(1f, effectivePixelSize.y * customAspect);
                }
            }

            material.SetVector(s_EffectivePixelSizeID, effectivePixelSize);

            // Set Other Uniforms
            material.SetInt(s_SamplingModeID, (int)settings.samplingMode.value);
            material.SetFloat(s_PosterizeLevelsID, settings.posterizeLevels.value);
            material.SetInt(s_LuminanceModeID, (int)settings.luminanceMode.value);

            bool paletteActive = settings.IsPaletteEffectActive();
            if (paletteActive) {
                SetMaterialKeyword(material, s_kwPaletteActiveName, true);
                material.SetTexture(s_PaletteTexID, settings.paletteTexture.value);
                material.SetFloat(s_PaletteBlendID, settings.paletteBlend.value);
            }

            bool ditherActive = settings.IsDitheringActive();
            if (ditherActive) {
                material.SetFloat(s_DitherIntensityID, settings.ditherIntensity.value);
                if (settings.ditheringMode.value == DitheringMode.OrderedAdditive && settings.HasValidDitherPatternSource()) {
                    SetMaterialKeyword(material, s_kwDitheringOrderedName, true);
                    material.SetFloat(s_DitherPatternTilingID, settings.ditherPatternTiling.value);
                    Texture patternTexture = settings.ditherPatternTexture.value;
                    if (settings.ditherPatternType.value != DitherPatternType.Custom) {
                        if(m_BuiltInDitherPatterns.TryGetValue(settings.ditherPatternType.value, out Texture2D builtInTex)) { patternTexture = builtInTex; }
                        else { patternTexture = null; }
                    }
                    material.SetTexture(s_DitherPatternTexID, patternTexture);
                } else if (settings.ditheringMode.value == DitheringMode.WhiteNoise) {
                    SetMaterialKeyword(material, s_kwDitheringWhiteNoiseName, true);
                }
            }
        }

        private static void ExecutePixelizePass(PixelizePassData data, RasterGraphContext context) {
            if (data.material == null) return;
            Blitter.BlitTexture(context.cmd, data.source, s_identityScaleBias, data.material, 0);
        }
        private static void ExecuteBlitBackPass(BlitBackPassData data, RasterGraphContext context) {
            if (data.blitMaterial == null) return;
            Blitter.BlitTexture(context.cmd, data.sourceToBlit, s_identityScaleBias, data.blitMaterial, 0);
        }
        public void Dispose() { }
    }
}