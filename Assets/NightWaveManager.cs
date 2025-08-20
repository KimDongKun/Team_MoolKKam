using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NightWaveManager : MonoBehaviour
{
    [Header("Links")]
    public MeteorPool meteorPool;

    [Header("Cycle")]
    public float dayDuration = 2f;     // ��(�غ� �ð�)
    public float nightDuration = 180f;  // �� 3��
    public int currentDay = 1;

    private bool running;

    public bool IsNight { get; private set; }
    public float NightRemaining { get; private set; }

    public event Action<int> OnDayStart;
    public event Action<int, float> OnNightStart;          // (day, duration)
    public event Action<float, float> OnNightProgress;     // (elapsed, duration)
    public event Action<int> OnNightEnd;


    void Start()
    {
        if (!running) StartCoroutine(DayNightLoop());
    }

    IEnumerator DayNightLoop()
    {
        running = true;

        while (true)
        {
            // ��
            IsNight = false;
            OnDayStart?.Invoke(currentDay);
            Debug.Log($"[DAY {currentDay}] DAY START");
            yield return new WaitForSeconds(dayDuration);

            // �� ����
            IsNight = true;
            Debug.Log($"[DAY {currentDay}] NIGHT START");
            var cfg = GetWave(currentDay);

            meteorPool.randomHorizontal = cfg.randomHorizontal;
            meteorPool.fixedDirX = cfg.fixedDirX;
            meteorPool.StopAutoLoop();

            OnNightStart?.Invoke(currentDay, nightDuration);

            // ����� Ʈ��ŷ �ڷ�ƾ ���� ����
            var progress = StartCoroutine(TrackNightProgress(nightDuration));

            yield return StartCoroutine(RunNight(cfg));

            // ����� Ʈ��ŷ ���� ����
            if (progress != null) StopCoroutine(progress);

            Debug.Log($"[DAY {currentDay}] NIGHT END");
            OnNightEnd?.Invoke(currentDay);

            currentDay++;
        }
    }

    IEnumerator TrackNightProgress(float duration)
    {
        float t = 0f;
        NightRemaining = duration;
        while (t < duration)
        {
            OnNightProgress?.Invoke(t, duration);
            NightRemaining = duration - t;
            t += Time.deltaTime;
            yield return null;
        }
        NightRemaining = 0f;
    }
    IEnumerator RunNight(WaveConfig cfg)
    {
        float elapsed = 0f;
        int spawned = 0;

        // 180�� ���� cfg.meteorCount�� �й� ����
        while (elapsed < nightDuration && spawned < cfg.meteorCount)
        {
            meteorPool.SpawnOne();
            spawned++;

            // ����
            float wait = Random.Range(cfg.intervalMin, cfg.intervalMax);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }

        // ���� �ð� ���
        if (elapsed < nightDuration)
            yield return new WaitForSeconds(nightDuration - elapsed);
    }

    //  ������ �����׿� ����/����/���� ���ϡ��� �ٲ���
    private WaveConfig GetWave(int day)
    {
        var cfg = new WaveConfig
        {
            meteorCount = 6,
            intervalMin = 12f,
            intervalMax = 15f,
            randomHorizontal = false,
            fixedDirX = 1
        };

        switch (day)
        {
            case 1:
                cfg.meteorCount = 20; cfg.intervalMin = 6f; cfg.intervalMax = 12f;
                cfg.randomHorizontal = false; cfg.fixedDirX = 1; // ���ʸ�
                break;
            case 2:
                cfg.meteorCount = 30; cfg.intervalMin = 5f; cfg.intervalMax = 13f;
                cfg.randomHorizontal = true; // �¿� ������
                break;
            case 3:
                cfg.meteorCount = 40; cfg.intervalMin = 4f; cfg.intervalMax = 11f;
                cfg.randomHorizontal = true;
                break;
            default: // 4����+
                cfg.meteorCount = Mathf.Min(25, 20 + (day - 9) * 2);
                cfg.intervalMin = 3f; cfg.intervalMax = 4f;
                cfg.randomHorizontal = true;
                break;
        }

        return cfg;
    }

    private struct WaveConfig
    {
        public int meteorCount;
        public float intervalMin, intervalMax;
        public bool randomHorizontal;
        public int fixedDirX;
    }
}