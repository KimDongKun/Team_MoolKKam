#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using justA.PixelFilter;

namespace justA.PixelFilter.Editor
{
    [CustomEditor(typeof(Pixelization))]
    sealed class PixelizationEditor : VolumeComponentEditor
    {
        // Parameters
        SerializedDataParameter m_OverrideEnable;
        SerializedDataParameter m_SizingMode;
        SerializedDataParameter m_PixelBlockSize;
        SerializedDataParameter m_TargetResolution;
        SerializedDataParameter m_DownsampleFactor;
        SerializedDataParameter m_SamplingMode;
        SerializedDataParameter m_PixelAspectMode;
        SerializedDataParameter m_CustomPixelAspectRatio;
        SerializedDataParameter m_PosterizeLevels;
        SerializedDataParameter m_EnablePalette;
        SerializedDataParameter m_LuminanceMode;
        SerializedDataParameter m_PaletteTexture;
        SerializedDataParameter m_PaletteBlend;
        SerializedDataParameter m_DitheringMode;
        SerializedDataParameter m_DitherPatternType;
        SerializedDataParameter m_DitherPatternTexture;
        SerializedDataParameter m_DitherPatternTiling;
        SerializedDataParameter m_DitherIntensity;

        public override void OnEnable()
        {
            var o = new PropertyFetcher<Pixelization>(serializedObject);
            m_OverrideEnable = Unpack(o.Find(x => x.overrideEnable));
            m_SizingMode = Unpack(o.Find(x => x.sizingMode));
            m_PixelBlockSize = Unpack(o.Find(x => x.pixelBlockSize));
            m_TargetResolution = Unpack(o.Find(x => x.targetResolution));
            m_DownsampleFactor = Unpack(o.Find(x => x.downsampleFactor));
            m_SamplingMode = Unpack(o.Find(x => x.samplingMode));
            m_PixelAspectMode = Unpack(o.Find(x => x.pixelAspectMode));
            m_CustomPixelAspectRatio = Unpack(o.Find(x => x.customPixelAspectRatio));
            m_PosterizeLevels = Unpack(o.Find(x => x.posterizeLevels));
            m_EnablePalette = Unpack(o.Find(x => x.enablePalette));
            m_LuminanceMode = Unpack(o.Find(x => x.luminanceMode));
            m_PaletteTexture = Unpack(o.Find(x => x.paletteTexture));
            m_PaletteBlend = Unpack(o.Find(x => x.paletteBlend));
            m_DitheringMode = Unpack(o.Find(x => x.ditheringMode));
            m_DitherPatternType = Unpack(o.Find(x => x.ditherPatternType));
            m_DitherPatternTexture = Unpack(o.Find(x => x.ditherPatternTexture));
            m_DitherPatternTiling = Unpack(o.Find(x => x.ditherPatternTiling));
            m_DitherIntensity = Unpack(o.Find(x => x.ditherIntensity));
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Master Control", EditorStyles.boldLabel);
            PropertyField(m_OverrideEnable, EditorGUIUtility.TrTextContent("Enable Pixelization Effect"));
            EditorGUILayout.Space();

            if (m_OverrideEnable.value.boolValue)
            {
                EditorGUILayout.LabelField("Pixelization Sizing", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertyField(m_SizingMode);
                SizingMode currentSizingMode = (SizingMode)m_SizingMode.value.enumValueIndex;
                if (currentSizingMode == SizingMode.FixedBlockSize) {
                    PropertyField(m_PixelBlockSize);
                } else { 
                    PropertyField(m_TargetResolution);
                }
                PropertyField(m_PixelAspectMode);
                 PixelAspectMode currentAspectMode = (PixelAspectMode)m_PixelAspectMode.value.enumValueIndex;
                 if (currentAspectMode == PixelAspectMode.Custom) {
                     PropertyField(m_CustomPixelAspectRatio);
                 }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);


                EditorGUILayout.LabelField("Pixelization Style", EditorStyles.boldLabel);
                 EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                 PropertyField(m_SamplingMode);
                 if (m_SamplingMode.value.enumValueIndex == (int)PixelSamplingMode.Average) {
                    EditorGUILayout.HelpBox("Average sampling mode can be performance intensive.", MessageType.Warning);
                 }
                 PropertyField(m_DownsampleFactor);
                 EditorGUILayout.EndVertical();
                 EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);


                EditorGUILayout.LabelField("Color Processing & Palette", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertyField(m_PosterizeLevels);
                EditorGUILayout.Space();
                PropertyField(m_EnablePalette, EditorGUIUtility.TrTextContent("Enable Palette Mapping"));
                if (m_EnablePalette.value.boolValue) {
                    EditorGUI.indentLevel++;
                    PropertyField(m_LuminanceMode);
                    PropertyField(m_PaletteTexture);
                    if (m_PaletteTexture.value.objectReferenceValue != null) { PropertyField(m_PaletteBlend); }
                    else { EditorGUILayout.HelpBox("Assign a 1D Look-Up Texture (LUT).", MessageType.Info); }
                     EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

                EditorGUILayout.LabelField("Dithering", EditorStyles.boldLabel);
                 EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                PropertyField(m_DitheringMode);
                DitheringMode currentDitherMode = (DitheringMode)m_DitheringMode.value.enumValueIndex;
                if (currentDitherMode != DitheringMode.Off) {
                    EditorGUI.indentLevel++;
                    PropertyField(m_DitherIntensity);
                    if (currentDitherMode == DitheringMode.OrderedAdditive) {
                        PropertyField(m_DitherPatternTiling, EditorGUIUtility.TrTextContent("Pattern Tiling"));
                        PropertyField(m_DitherPatternType);
                        DitherPatternType currentPatternType = (DitherPatternType)m_DitherPatternType.value.enumValueIndex;
                        if (currentPatternType == DitherPatternType.Custom) {
                            PropertyField(m_DitherPatternTexture);
                            if (m_DitherPatternTexture.value.objectReferenceValue == null) { EditorGUILayout.HelpBox("Assign custom Pattern Texture.", MessageType.Info); }
                        } else { EditorGUILayout.HelpBox($"Using built-in {currentPatternType} (loaded from Resources).", MessageType.None); }
                    } else if (currentDitherMode == DitheringMode.WhiteNoise) { EditorGUILayout.HelpBox("White Noise uses procedural hash aligned with pixel blocks.", MessageType.None); }
                     EditorGUI.indentLevel--;
                }
                 EditorGUILayout.EndVertical();
            } else {
                 EditorGUILayout.HelpBox("Enable the 'Enable Pixelization Effect' override above to see settings.", MessageType.Info);
            }
        }
    }
}
#endif