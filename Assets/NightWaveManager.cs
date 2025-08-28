using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NightWaveManager : MonoBehaviour
{
    [Header("Links")]
    public MeteorPool meteorPool;

    [Header("Cycle")]
    public float dayDuration = 2f;     // 낮(준비 시간)
    public float nightDuration = 180f;  // 밤 3분
    public int currentDay = 1;

    public GameObject bossUi;

    public GameObject boss;
    private bool running;
    public GameObject endPageUi;
    public Transform endPageSpawnPos;
    public GameObject player;
    public bool IsNight { get; private set; }
    public float NightRemaining { get; private set; }

    public event Action<int, float> OnDayStart;
    public event Action<int, float> OnNightStart;          // (day, duration)
    public event Action<float, float> OnNightProgress;     // (elapsed, duration)
    public event Action<int> OnNightEnd;

    public GameObject idleNpc; // 관리자 평소 대화 npc 스크립트
    public GameObject endPageNpc; // 관리자 평소 대화 npc 스크립트
    public GameObject endWall;
    public GameObject endPageRertyUi;
    public bool isEndPage = false;

    public void Start()
    {
        if (!running) StartCoroutine(DayNightLoop());
    }

    IEnumerator DayNightLoop()
    {
        while (TutorialManager.isTutorial)
        {
            yield return null;
        }

        running = true;

        while (true)
        {
            // 낮
            IsNight = false;
            OnDayStart?.Invoke(currentDay, dayDuration);
            Debug.Log($"[DAY {currentDay}] DAY START");
            StartCoroutine(TrackNightProgress(dayDuration));
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
                cfg.meteorCount = 40; cfg.intervalMin = 6f; cfg.intervalMax = 12f;
                cfg.randomHorizontal = false; cfg.fixedDirX = 1; // 한쪽만
                break;
            case 2:
                cfg.meteorCount = 60; cfg.intervalMin = 5f; cfg.intervalMax = 13f;
                cfg.randomHorizontal = true; // 좌우 번갈아
                break;
            case 3:
                cfg.meteorCount = 80; cfg.intervalMin = 4f; cfg.intervalMax = 11f;
                cfg.randomHorizontal = true;
                break;
            case 4:
                if (!boss.activeSelf)
                {
                    isEndPage = true;
                    endPageUi.SetActive(true);
                    idleNpc.SetActive(false);
                    endPageNpc.SetActive(true);
                    endWall.SetActive(true);
                    StartCoroutine(EndPageEvent());
                    // boss.SetActive(true);
                }
                nightDuration = 600f;
                cfg.meteorCount = 0;
                break;
            default: // 4일차+
                if(boss.activeSelf) Debug.Log("GameOver.");

                //cfg.intervalMin = 3f; cfg.intervalMax = 4f;
                //cfg.randomHorizontal = true;
                break;
        }

        return cfg;
    }

    public IEnumerator EndPageRetry()
    {
        boss.SetActive(false);
        nightDuration = 600f;
        endPageRertyUi.SetActive(true);
        yield return new WaitForSeconds(2f);
        player.transform.position = endPageSpawnPos.position;
        yield return new WaitForSeconds(2f);
        endPageRertyUi.SetActive(false);
    }

    IEnumerator EndPageEvent()
    {
        yield return new WaitForSeconds(1.5f);
        endPageSpawnPos = GameObject.Find("endPageSpawn").transform;
        player.transform.position = endPageSpawnPos.position;
        yield return new WaitForSeconds(2.5f);
        endPageUi.SetActive(false);
        

    }

    public void SpawnBoss()
    {
        boss.SetActive(true);
    }


    private struct WaveConfig
    {
        public int meteorCount;
        public float intervalMin, intervalMax;
        public bool randomHorizontal;
        public int fixedDirX;
    }
}