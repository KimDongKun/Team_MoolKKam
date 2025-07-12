Shader "Hidden/just a pixel filter/Pixelization"
{
    HLSLINCLUDE
        #pragma multi_compile_local _ _PALETTE_ACTIVE
        #pragma multi_compile_local _ _DITHERING_ORDERED_ADDITIVE _DITHERING_WHITE_NOISE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

        static const float3 LUMINANCE_WEIGHTS = float3(0.2126729, 0.7151522, 0.0721750);

        // Uniforms
        float2 _EffectivePixelSize;
        int    _SamplingMode;
        float  _PosterizeLevels;
        int    _LuminanceMode; 
        #if defined(_PALETTE_ACTIVE)
            float _PaletteBlend;
            TEXTURE2D_X(_PaletteTex);
            SAMPLER(sampler_PaletteTex);
        #endif
        #if defined(_DITHERING_ORDERED_ADDITIVE) || defined(_DITHERING_WHITE_NOISE)
            float _DitherIntensity;
        #endif
        #if defined(_DITHERING_ORDERED_ADDITIVE)
            float _DitherPatternTiling;
            TEXTURE2D_X(_DitherPatternTex);
            SAMPLER(sampler_DitherPatternTex);
        #endif

        // Helper Functions
        half3 LinearToGammaApprox(half3 linearCol) { linearCol = max(0.0h, linearCol); return pow(linearCol, 1.0h / 2.2h); }
        half3 GammaToLinearApprox(half3 gammaCol) { gammaCol = max(0.0h, gammaCol); return pow(gammaCol, 2.2h); }
        float GetLuminance(float3 col, int mode) { float3 linearColor = (mode == 1) ? GammaToLinearApprox(col) : col; return dot(linearColor, LUMINANCE_WEIGHTS); }
        half3 Posterize(half3 color, float levels) { if (levels <= 1.0h || levels >= 255.5h) return color; color = max(0.0h, color); return floor(color * levels) / levels; }
        float Hash(float2 p) { float3 p3 = frac(p.xyx * .1031h); p3 += dot(p3, p3.yzx + 33.33h); return frac((p3.x + p3.y) * p3.z); }

    ENDHLSL

    SubShader
    {
        Pass
        {
            Name "PixelFilter Pass"
            ZWrite Off Cull Off Blend Off ZTest Always

            HLSLPROGRAM
                #pragma vertex Vert
                #pragma fragment FragPixelFilter

                half4 FragPixelFilter(Varyings input) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                    // --- Pixelization Setup ---
                    // _BlitTexture_TexelSize.zw contains the source texture size passed to this pass
                    float2 sourceTextureSize = _BlitTexture_TexelSize.zw;
                    // _BlitTexture_TexelSize.xy contains 1/sourceWidth, 1/sourceHeight
                    float2 sourceTexelSize = _BlitTexture_TexelSize.xy;

                    // Ensure effective size is at least one texel
                    float2 effectiveSize = max(float2(1.0f, 1.0f), _EffectivePixelSize);

                    // Size of one final "pixel block" in UV space [0,1]
                    float2 blockUVSize = effectiveSize * sourceTexelSize;
                    if(blockUVSize.x <= 0.0f || blockUVSize.y <= 0.0f) return half4(0,1,1,1); // Error color if invalid size

                    // Calculate the index/coordinate of the current block
                    float2 blockIndexCoord = floor(input.texcoord / blockUVSize);

                    // Calculate the UV coordinate of the origin (top-left) of the current block
                    float2 blockOriginUV = blockIndexCoord * blockUVSize;

                    half4 pixelatedColor;

                    // --- Pixel Sampling ---
                    if (_SamplingMode == 2) { // Average Sampling
                         half4 accumulatedColor = 0.0h; float sampleCount = 0.0f;
                         int texelsPerBlockX = max(1, (int)ceil(effectiveSize.x));
                         int texelsPerBlockY = max(1, (int)ceil(effectiveSize.y));
                         float2 startSourceUV = blockOriginUV;
                         for (int y = 0; y < texelsPerBlockY; ++y) { for (int x = 0; x < texelsPerBlockX; ++x) {
                                 float2 offset = float2(x + 0.5h, y + 0.5h) * sourceTexelSize;
                                 float2 currentSourceUV = startSourceUV + offset;
                                 // Clamp sample UV to avoid reading outside source due to potential precision issues
                                 currentSourceUV = clamp(currentSourceUV, 0.0001h, 0.9999h);
                                 accumulatedColor += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, currentSourceUV);
                                 sampleCount += 1.0f;
                                } }
                         pixelatedColor = (sampleCount > 0.0f) ? (accumulatedColor / sampleCount) : half4(0.0h,0.0h,0.0h,1.0h);
                    } else { // TopLeft and Center Sampling
                         float2 blockCenterOffset = 0.5h * blockUVSize; // Center offset in UV space
                         float2 sampleUV = (_SamplingMode == 1) ? (blockOriginUV + blockCenterOffset) : blockOriginUV;
                         // Clamp sample UV to avoid reading outside source due to potential precision issues
                         sampleUV = clamp(sampleUV, 0.0001h, 0.9999h);
                         pixelatedColor = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, sampleUV);
                    }

                    // --- Posterization ---
                    pixelatedColor.rgb = Posterize(pixelatedColor.rgb, _PosterizeLevels);

                    // --- Palette Mapping ---
                    #if defined(_PALETTE_ACTIVE)
                        float luma = saturate(GetLuminance(pixelatedColor.rgb, _LuminanceMode));
                        half3 paletteColor = SAMPLE_TEXTURE2D_X(_PaletteTex, sampler_PaletteTex, float2(luma, 0.5h)).rgb;
                        pixelatedColor.rgb = lerp(pixelatedColor.rgb, paletteColor, saturate(_PaletteBlend));
                    #endif

                    // --- Dithering ---
                    #if defined(_DITHERING_ORDERED_ADDITIVE)
                        float2 screenUV_Ordered = input.positionCS.xy * _ScreenParams.zw;
                        float2 ditherUV = screenUV_Ordered * _DitherPatternTiling;
                        half ditherThreshold = SAMPLE_TEXTURE2D_X(_DitherPatternTex, sampler_DitherPatternTex, ditherUV).r;
                        half centeredThreshold = ditherThreshold - 0.5h;
                        half ditherOffset = centeredThreshold * _DitherIntensity * (1.0h / 64.0h);
                        pixelatedColor.rgb += ditherOffset;
                    #elif defined(_DITHERING_WHITE_NOISE)
                         float2 noiseCoord = blockIndexCoord;
                         float randomThreshold = Hash(noiseCoord);
                         half centeredThreshold = randomThreshold - 0.5h;
                         half ditherOffset = centeredThreshold * _DitherIntensity * (1.0h / 16.0h);
                         pixelatedColor.rgb += ditherOffset;
                    #endif

                    // Final clamp
                    pixelatedColor.rgb = saturate(pixelatedColor.rgb);

                    return half4(pixelatedColor.rgb, 1.0h);
                }

            ENDHLSL
        }
    }
    Fallback Off
}