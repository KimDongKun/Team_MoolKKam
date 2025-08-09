using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeteorPool : MonoBehaviour
{
    [Header("Pooling")]
    public GameObject meteorPrefab;
    public int initialSize = 20;

    [Header("Respawn / Spawn Area")]
    public float topY = 15f;        // 화면 위 스폰 높이
    public Vector2 xRange = new Vector2(-10f, 10f);
    public float respawnDelay = 0.2f; // 반납 후 새로 스폰하기까지 간격

    [Header("Move Params")]
    public float meteorSpeed = 8f;
    public bool randomHorizontal = true; // 좌/우 랜덤 낙하
    public int fixedDirX = 1;            // randomHorizontal=false일 때 1:오른쪽, -1:왼쪽

    private float spawnDelayMin = 4f; // 스폰 간격
    private float spawnDelayMax = 7f; // 스폰 간격

    private readonly Queue<MoonFragmentScript> inactive = new();      // 대기열
    private readonly LinkedList<MoonFragmentScript> active = new();   // 사용 중(맨 앞이 가장 오래됨)

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(meteorPrefab, Vector3.one * 9999f, Quaternion.identity);
            obj.SetActive(false);
            inactive.Enqueue(obj.GetComponent<MoonFragmentScript>());
        }

        StartCoroutine(SpawnLoop()); // 일정/랜덤 텀으로 계속 스폰
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            Debug.Log("Spawned a meteor!");
            // 랜덤 텀 (예: 0.5 ~ 2초)
            float waitTime = Random.Range(spawnDelayMin, spawnDelayMax);
            yield return new WaitForSeconds(waitTime);
        }
    }


    public void SpawnOne()
    {
        MoonFragmentScript m = null;

        if (inactive.Count > 0)
        {
            // 반드시 비활성부터
            m = inactive.Dequeue();
            // Debug.Log("[SpawnOne] from INACTIVE");
        }
        else if (active.Count > 0)
        {
            //  비활성 없으면 활성 중 가장 오래된 것 재활용
            m = active.First.Value;
            active.RemoveFirst();

            // 안전 리셋
            m.StopAllCoroutines();
            m.gameObject.SetActive(false);
            // Debug.Log("[SpawnOne] REUSE oldest ACTIVE");
        }
        else
        {
            // 극단적 상황 대비(거의 안탐)
            m = Instantiate(meteorPrefab).GetComponent<MoonFragmentScript>();
            // Debug.LogWarning("[SpawnOne] POOL EMPTY; INSTANTIATED");
        }

        // 스폰 위치/방향/회전 세팅
        Vector3 pos = new Vector3(Random.Range(xRange.x, xRange.y), topY, 0f);
        m.transform.position = pos;
        m.transform.rotation = new Quaternion(0f, 0f, -0.16873014f, 0.985662341f);

        int dirX = randomHorizontal ? (Random.value < 0.5f ? -1 : 1)
                                    : (fixedDirX != 0 ? (int)Mathf.Sign(fixedDirX) : 1);
        Vector3 dir = new Vector3(dirX, -1f, 0f);

        // 초기화 & 활성화
        m.Init(this, dir, meteorSpeed);
        m.gameObject.SetActive(true);

        // 활성 목록의 "가장 최신"으로 등록
        active.AddLast(m);
    }

    public void Despawn(MoonFragmentScript m)
    {
        // 활성 목록에서 제거 (어디 있든 안전하게)
        var node = active.Find(m);
        if (node != null) active.Remove(node);

        m.gameObject.SetActive(false);
        inactive.Enqueue(m); // 반드시 비활성 큐로 복귀
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnOne();
    }
}