using UnityEngine;

using System.Collections;

public class NightWaveManager : MonoBehaviour
{
    [Header("Links")]
    public MeteorPool meteorPool;

    [Header("Cycle")]
    public float dayDuration = 2f;     // 낮(준비 시간)
    public float nightDuration = 180f;  // 밤 3분
    public int currentDay = 1;

    private bool running;

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
            Debug.Log($"[DAY {currentDay}] DAY START");
            yield return new WaitForSeconds(dayDuration);

            // 밤 시작
            Debug.Log($"[DAY {currentDay}] NIGHT START");
            var cfg = GetWave(currentDay);

            // MeteorPool 방향/모드만 건드림(기존 코드 최대활용)
            meteorPool.randomHorizontal = cfg.randomHorizontal;
            meteorPool.fixedDirX = cfg.fixedDirX;

            // 자동 루프는 꺼두고(혹시 켜져있으면 정지) → 우리가 수동으로 스폰
            meteorPool.StopAutoLoop();

            yield return StartCoroutine(RunNight(cfg));

            Debug.Log($"[DAY {currentDay}] NIGHT END");
            currentDay++;
        }
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
                cfg.meteorCount = 6; cfg.intervalMin = 12f; cfg.intervalMax = 15f;
                cfg.randomHorizontal = false; cfg.fixedDirX = 1; // 한쪽만
                break;
            case 2:
                cfg.meteorCount = 8; cfg.intervalMin = 10f; cfg.intervalMax = 13f;
                cfg.randomHorizontal = true; // 좌우 번갈아
                break;
            case 3:
                cfg.meteorCount = 10; cfg.intervalMin = 9f; cfg.intervalMax = 11f;
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