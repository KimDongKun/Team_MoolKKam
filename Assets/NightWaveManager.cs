using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class NightWaveManager : MonoBehaviour
{
    [Header("Links")]
    public MeteorPool meteorPool;

    [Header("Cycle")]
    public float dayDuration = 2f;     // 낮(준비 시간)
    public float nightDuration = 180f;  // 밤 3분
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
            // 낮
            IsNight = false;
            OnDayStart?.Invoke(currentDay);
            Debug.Log($"[DAY {currentDay}] DAY START");
            yield return new WaitForSeconds(dayDuration);

            // 밤 시작
            IsNight = true;
            Debug.Log($"[DAY {currentDay}] NIGHT START");
            var cfg = GetWave(currentDay);

            meteorPool.randomHorizontal = cfg.randomHorizontal;
            meteorPool.fixedDirX = cfg.fixedDirX;
            meteorPool.StopAutoLoop();

            OnNightStart?.Invoke(currentDay, nightDuration);

            // 진행률 트래킹 코루틴 병렬 실행
            var progress = StartCoroutine(TrackNightProgress(nightDuration));

            yield return StartCoroutine(RunNight(cfg));

            // 진행률 트래킹 종료 보장
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

        // 180초 동안 cfg.meteorCount개 분배 스폰
        while (elapsed < nightDuration && spawned < cfg.meteorCount)
        {
            meteorPool.SpawnOne();
            spawned++;

            // 간격
            float wait = Random.Range(cfg.intervalMin, cfg.intervalMax);
            yield return new WaitForSeconds(wait);
            elapsed += wait;
        }

        // 남은 시간 대기
        if (elapsed < nightDuration)
            yield return new WaitForSeconds(nightDuration - elapsed);
    }

    //  날마다 “메테오 개수/간격/방향 패턴”만 바꿔줌
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
                cfg.randomHorizontal = false; cfg.fixedDirX = 1; // 한쪽만
                break;
            case 2:
                cfg.meteorCount = 30; cfg.intervalMin = 5f; cfg.intervalMax = 13f;
                cfg.randomHorizontal = true; // 좌우 번갈아
                break;
            case 3:
                cfg.meteorCount = 40; cfg.intervalMin = 4f; cfg.intervalMax = 11f;
                cfg.randomHorizontal = true;
                break;
            default: // 4일차+
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