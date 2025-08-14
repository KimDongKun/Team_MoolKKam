using UnityEngine;

using System.Collections;

public class NightWaveManager : MonoBehaviour
{
    [Header("Links")]
    public MeteorPool meteorPool;

    [Header("Cycle")]
    public float dayDuration = 2f;     // ��(�غ� �ð�)
    public float nightDuration = 180f;  // �� 3��
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
            // ��
            Debug.Log($"[DAY {currentDay}] DAY START");
            yield return new WaitForSeconds(dayDuration);

            // �� ����
            Debug.Log($"[DAY {currentDay}] NIGHT START");
            var cfg = GetWave(currentDay);

            // MeteorPool ����/��常 �ǵ帲(���� �ڵ� �ִ�Ȱ��)
            meteorPool.randomHorizontal = cfg.randomHorizontal;
            meteorPool.fixedDirX = cfg.fixedDirX;

            // �ڵ� ������ ���ΰ�(Ȥ�� ���������� ����) �� �츮�� �������� ����
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
                cfg.meteorCount = 6; cfg.intervalMin = 12f; cfg.intervalMax = 15f;
                cfg.randomHorizontal = false; cfg.fixedDirX = 1; // ���ʸ�
                break;
            case 2:
                cfg.meteorCount = 8; cfg.intervalMin = 10f; cfg.intervalMax = 13f;
                cfg.randomHorizontal = true; // �¿� ������
                break;
            case 3:
                cfg.meteorCount = 10; cfg.intervalMin = 9f; cfg.intervalMax = 11f;
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