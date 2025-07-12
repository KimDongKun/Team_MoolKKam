using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace justA.PixelFilter
{
    [CreateAssetMenu(menuName = "Rendering/URP/just a pixel filter feature", fileName = "PixelFilterFeature")]
    public class PixelizationRendererFeature : ScriptableRendererFeature
    {
        private const string BUILTIN_PATTERN_RESOURCE_PATH = "DitherPatterns/";

        [System.Serializable]
        public class PixelizationSettings {
            [Tooltip("The shader implementing the pixelization effects.")]
            public Shader pixelizationShader = null;
            [Tooltip("The pipeline stage where the effect should be injected.")]
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
            [Tooltip("Name used for debugging and profiling scopes.")]
            public string profilerTag = "just a pixel filter";
        }

        public PixelizationSettings settings = new PixelizationSettings();

        private Material m_Material;
        private PixelizationPass m_PixelizationPass;
        private Dictionary<DitherPatternType, Texture2D> m_BuiltInPatterns;

        public override void Create()
        {
            Cleanup();
            if (settings.pixelizationShader == null) {
                settings.pixelizationShader = Shader.Find("Hidden/just a pixel filter/Pixelization");
            }
            m_Material = CoreUtils.CreateEngineMaterial(settings.pixelizationShader);
            if (m_Material == null) {
                Debug.LogErrorFormat("{0}: Failed to create material from shader '{1}'.", GetType().Name, settings.pixelizationShader.name);
                return;
            }
            LoadBuiltInPatterns();
            m_PixelizationPass = new PixelizationPass(m_Material, m_BuiltInPatterns, settings.profilerTag) {
                renderPassEvent = settings.renderPassEvent
            };
        }

        private void LoadBuiltInPatterns() {
             m_BuiltInPatterns = new Dictionary<DitherPatternType, Texture2D>();
             LoadPatternFromResources(DitherPatternType.Bayer2x2, "Bayer2x2");
             LoadPatternFromResources(DitherPatternType.Bayer4x4, "Bayer4x4");
             LoadPatternFromResources(DitherPatternType.Bayer8x8, "Bayer8x8");
        }

        private void LoadPatternFromResources(DitherPatternType type, string filename) {
            string path = BUILTIN_PATTERN_RESOURCE_PATH + filename;
            Texture2D tex = Resources.Load<Texture2D>(path);
            if (tex != null) {
                m_BuiltInPatterns[type] = tex;
            }
            else {
                Debug.LogWarningFormat("{0}: Built-in dither pattern '{1}' not found at Resources path '{2}'. Ensure the texture exists and its import settings are correct.",
                    GetType().Name, filename, path);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_PixelizationPass == null || m_Material == null) {
                return;
            }

            if (m_PixelizationPass.Setup(ref renderingData, m_Material))
            {
                renderer.EnqueuePass(m_PixelizationPass);
            }
        }

        protected override void Dispose(bool disposing) {
            Cleanup();
            base.Dispose(disposing);
        }

        private void Cleanup() {
            m_PixelizationPass?.Dispose();
            m_PixelizationPass = null;
            CoreUtils.Destroy(m_Material);
            m_Material = null;
            m_BuiltInPatterns?.Clear();
            m_BuiltInPatterns = null;
        }
    }
}