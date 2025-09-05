using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DamagePostProcess : MonoBehaviour
{
    public Volume volume;
    private ColorAdjustments colorAdjust;

    private void Start()
    {
        volume.profile.TryGet(out colorAdjust);
    }

    public void ShowDamageEffect()
    {
        StartCoroutine(FadeEffect());
    }

    private IEnumerator FadeEffect()
    {
        float elapsed = 0f;
        float duration = 0.5f;
        colorAdjust.colorFilter.value = Color.red;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = 1f - (elapsed / duration);
            colorAdjust.colorFilter.value = Color.Lerp(Color.white, Color.red, t * 0.5f);
            yield return null;
        }

        colorAdjust.colorFilter.value = Color.white;
    }
}