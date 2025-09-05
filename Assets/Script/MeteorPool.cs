using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeteorPool : MonoBehaviour
{
    [Header("Pooling")]
    public GameObject meteorPrefab;
    public int initialSize = 20;

    [Header("Respawn / Spawn Area")]
    public float topY = 15f;
    public Vector2 xRange = new Vector2(-10f, 10f);
    public float respawnDelay = 0.2f;

    [Header("Move Params")]
    public float meteorSpeed = 8f;
    public bool randomHorizontal = true;
    public int fixedDirX = 1;

    [Header("Spawn Loop Control")]
    public bool autoSpawn = false;                 //  인스펙터로 켜면 기존처럼 자동 스폰
    public float spawnDelayMin = 4f;               //  자동 루프에서만 사용
    public float spawnDelayMax = 7f;

    private readonly Queue<MoonFragmentScript> inactive = new();
    private readonly LinkedList<MoonFragmentScript> active = new();

    //  추가: 루프 핸들
    private Coroutine loopCo;

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(meteorPrefab, Vector3.one * 9999f, Quaternion.identity);
            obj.SetActive(false);
            inactive.Enqueue(obj.GetComponent<MoonFragmentScript>());
        }

        if (autoSpawn) loopCo = StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            float waitTime = Random.Range(spawnDelayMin, spawnDelayMax);
            yield return new WaitForSeconds(waitTime);
        }
    }

    // 추가: 외부에서 자동 루프 끄기/켜기
    public void StartAutoLoop(float min, float max)
    {
        spawnDelayMin = min; spawnDelayMax = max;
        if (loopCo != null) StopCoroutine(loopCo);
        loopCo = StartCoroutine(SpawnLoop());
    }

    public void StopAutoLoop()
    {
        if (loopCo != null)
        {
            StopCoroutine(loopCo);
            loopCo = null;
        }
    }

    public MoonFragmentScript SpawnOne()
    {
        MoonFragmentScript m = null;

        if (inactive.Count > 0)
        {
            m = inactive.Dequeue();
        }
        else if (active.Count > 0)
        {
            m = active.First.Value;
            active.RemoveFirst();
            m.StopAllCoroutines();
            m.gameObject.SetActive(false);
        }
        else
        {
            m = Instantiate(meteorPrefab).GetComponent<MoonFragmentScript>();
        }

        Vector3 pos = new Vector3(Random.Range(xRange.x, xRange.y), topY, 0f);
        m.transform.position = pos;
        m.transform.rotation = Quaternion.identity;

        int dirX = randomHorizontal ? (Random.value < 0.5f ? -1 : 1)
                                    : (fixedDirX != 0 ? (int)Mathf.Sign(fixedDirX) : 1);
        Vector3 dir = new Vector3(dirX, -1f, 0f).normalized;

        m.Init(this, dir, meteorSpeed);
        m.gameObject.SetActive(true);

        active.AddLast(m);
        return m;
    }

    public void Despawn(MoonFragmentScript m)
    {
        var node = active.Find(m);
        if (node != null) active.Remove(node);

        m.gameObject.SetActive(false);
        inactive.Enqueue(m);
    }
}
