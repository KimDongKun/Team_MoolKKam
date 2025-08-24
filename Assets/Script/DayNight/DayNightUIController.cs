using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DayNightUIController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] NightWaveManager manager;

    [Header("Widgets")]
    [SerializeField] public TMP_Text phaseText;       // "DAY" / "NIGHT"
    [SerializeField] public TMP_Text dayText;         // "Day 3"
    [SerializeField] public Slider nightProgress;     // 0~1 (³²Àº ½Ã°£ Ç¥½Ã¸é 1 - t/dur)
    [SerializeField] public TMP_Text nightTimerText;  // "02:15"
    [SerializeField] CanvasGroup nightTint;    // ¹ã¿¡¸¸ »ìÂ¦ ¾îµÓ°Ô (alpha 0~0.35)
    [SerializeField] GameObject nightImage;
    [SerializeField] GameObject DayImage;
    [Header("Style")]
    [SerializeField] float tintAlphaNight = 0.35f;
    [SerializeField] float tintFade = 0.3f;

    Coroutine tintCo;

    void OnEnable()
    {
        if (!manager) manager = FindAnyObjectByType<NightWaveManager>();
        if (manager)
        {
            nightImage.SetActive(false);
            DayImage.SetActive(false);
            manager.OnDayStart += HandleDay;
            manager.OnNightStart += HandleNight;
            manager.OnNightProgress += HandleNightProgress;
            manager.OnNightEnd += HandleNightEnd;
        }
        InitUI();
    }
    void OnDisable()
    {
        if (manager)
        {
            manager.OnDayStart -= HandleDay;
            manager.OnNightStart -= HandleNight;
            manager.OnNightProgress -= HandleNightProgress;
            manager.OnNightEnd -= HandleNightEnd;
        }
    }

    void InitUI()
    {
        if (nightProgress) nightProgress.gameObject.SetActive(false);
        if (nightTimerText) nightTimerText.gameObject.SetActive(false);
        if (phaseText) phaseText.text = "DAY";
        if (nightTint) nightTint.alpha = 0f;
    }

    void HandleDay(int day, float duration)
    {
        if (phaseText)
        {
            phaseText.text = "DAY";
            DayImage.SetActive(true);
            nightImage.SetActive(false);
        }
        if (dayText) dayText.text = $"Day {day}";
        if (nightProgress) nightProgress.gameObject.SetActive(true);
        if (nightTimerText) nightTimerText.gameObject.SetActive(true);

        if (nightTimerText)
        {
            nightTimerText.text = Format(duration);
            nightTimerText.gameObject.SetActive(true);
        }
        FadeTint(0f);
    }

    void HandleNight(int day, float duration)
    {
        if (phaseText)
        {
            phaseText.text = "NIGHT";
            nightImage.SetActive(true);
            DayImage.SetActive(false);
        }
        if (dayText) dayText.text = $"Day {day}";
        if (nightProgress)
        {
            nightProgress.value = 0f;
            nightProgress.gameObject.SetActive(true);
        }
        if (nightTimerText)
        {
            nightTimerText.text = Format(duration);
            nightTimerText.gameObject.SetActive(true);
        }
        FadeTint(tintAlphaNight);
    }

    void HandleNightProgress(float elapsed, float duration)
    {
        if (nightProgress) nightProgress.value = elapsed / duration;
        if (nightTimerText) nightTimerText.text = Format(duration - elapsed);
    }

    void HandleNightEnd(int day)
    {
        if (nightProgress) nightProgress.gameObject.SetActive(false);
        if (nightTimerText) nightTimerText.gameObject.SetActive(false);
        FadeTint(0f);
    }

    void FadeTint(float target)
    {
        if (!nightTint) return;
        if (tintCo != null) StopCoroutine(tintCo);
        tintCo = StartCoroutine(FadeTintRoutine(target));
    }

    System.Collections.IEnumerator FadeTintRoutine(float target)
    {
        float start = nightTint.alpha;
        float t = 0f;
        while (t < 1f)
        {
            nightTint.alpha = Mathf.Lerp(start, target, t);
            t += Time.deltaTime / tintFade;
            yield return null;
        }
        nightTint.alpha = target;
    }

    string Format(float sec)
    {
        if (sec < 0f) sec = 0f;
        int m = Mathf.FloorToInt(sec / 60f);
        int s = Mathf.FloorToInt(sec % 60f);
        return $"{m:00}:{s:00}";
    }
}
