using UnityEngine;
using UnityEngine.Rendering;

public class DayNightLightingController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] NightWaveManager manager;
    [SerializeField] Light sun;                     // Directional Light
    [SerializeField] Volume nightVolume;            // (선택) URP/HDRP Global Volume (밤 효과용)

    [Header("Day Settings")]
    [SerializeField] Color dayLightColor = new Color(1f, 0.96f, 0.84f);
    [SerializeField] float dayIntensity = 1.2f;
    [SerializeField] Color dayAmbient = new Color(0.65f, 0.65f, 0.65f);

    [Header("Night Settings")]
    [SerializeField] Color nightLightColor = new Color(0.45f, 0.55f, 0.9f);
    [SerializeField] float nightIntensity = 0.2f;
    [SerializeField] Color nightAmbient = new Color(0.12f, 0.14f, 0.22f);

    [Header("Transition")]
    [SerializeField] float transitionTime = 1.2f;
    [SerializeField] AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Coroutine lerpCo;

    void OnEnable()
    {
        if (!manager) manager = FindAnyObjectByType<NightWaveManager>();
        if (manager)
        {
            manager.OnDayStart += HandleDay;
            manager.OnNightStart += HandleNight;
        }
    }
    void OnDisable()
    {
        if (manager)
        {
            manager.OnDayStart -= HandleDay;
            manager.OnNightStart -= HandleNight;
        }
    }

    void HandleDay(int day, float duration)
    {
        StartLerp(dayLightColor, dayIntensity, dayAmbient, 0f);
    }

    void HandleNight(int day, float duration)
    {
        StartLerp(nightLightColor, nightIntensity, nightAmbient, 1f);
    }

    void StartLerp(Color lc, float intensity, Color ambient, float volumeWeight)
    {
        if (lerpCo != null) StopCoroutine(lerpCo);
        lerpCo = StartCoroutine(LerpLighting(lc, intensity, ambient, volumeWeight));
    }

    System.Collections.IEnumerator LerpLighting(Color targetColor, float targetIntensity, Color targetAmbient, float targetVolumeWeight)
    {
        if (!sun) yield break;

        Color startColor = sun.color;
        float startIntensity = sun.intensity;
        Color startAmbient = RenderSettings.ambientLight;
        float startWeight = nightVolume ? nightVolume.weight : 0f;

        float t = 0f;
        while (t < 1f)
        {
            float k = curve.Evaluate(t);
            sun.color = Color.Lerp(startColor, targetColor, k);
            sun.intensity = Mathf.Lerp(startIntensity, targetIntensity, k);
            RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, k);
            if (nightVolume) nightVolume.weight = Mathf.Lerp(startWeight, targetVolumeWeight, k);

            t += Time.deltaTime / transitionTime;
            yield return null;
        }

        sun.color = targetColor;
        sun.intensity = targetIntensity;
        RenderSettings.ambientLight = targetAmbient;
        if (nightVolume) nightVolume.weight = targetVolumeWeight;
    }
}
