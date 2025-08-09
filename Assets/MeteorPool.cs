using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class MeteorPool : MonoBehaviour
{
    [Header("Pooling")]
    public GameObject meteorPrefab;
    public int initialSize = 20;

    [Header("Respawn / Spawn Area")]
    public float topY = 15f;        // ȭ�� �� ���� ����
    public Vector2 xRange = new Vector2(-10f, 10f);
    public float respawnDelay = 0.2f; // �ݳ� �� ���� �����ϱ���� ����

    [Header("Move Params")]
    public float meteorSpeed = 8f;
    public bool randomHorizontal = true; // ��/�� ���� ����
    public int fixedDirX = 1;            // randomHorizontal=false�� �� 1:������, -1:����

    private float spawnDelayMin = 4f; // ���� ����
    private float spawnDelayMax = 7f; // ���� ����

    private readonly Queue<MoonFragmentScript> inactive = new();      // ��⿭
    private readonly LinkedList<MoonFragmentScript> active = new();   // ��� ��(�� ���� ���� ������)

    void Start()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var obj = Instantiate(meteorPrefab, Vector3.one * 9999f, Quaternion.identity);
            obj.SetActive(false);
            inactive.Enqueue(obj.GetComponent<MoonFragmentScript>());
        }

        StartCoroutine(SpawnLoop()); // ����/���� ������ ��� ����
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            Debug.Log("Spawned a meteor!");
            // ���� �� (��: 0.5 ~ 2��)
            float waitTime = Random.Range(spawnDelayMin, spawnDelayMax);
            yield return new WaitForSeconds(waitTime);
        }
    }


    public void SpawnOne()
    {
        MoonFragmentScript m = null;

        if (inactive.Count > 0)
        {
            // �ݵ�� ��Ȱ������
            m = inactive.Dequeue();
            // Debug.Log("[SpawnOne] from INACTIVE");
        }
        else if (active.Count > 0)
        {
            //  ��Ȱ�� ������ Ȱ�� �� ���� ������ �� ��Ȱ��
            m = active.First.Value;
            active.RemoveFirst();

            // ���� ����
            m.StopAllCoroutines();
            m.gameObject.SetActive(false);
            // Debug.Log("[SpawnOne] REUSE oldest ACTIVE");
        }
        else
        {
            // �ش��� ��Ȳ ���(���� ��Ž)
            m = Instantiate(meteorPrefab).GetComponent<MoonFragmentScript>();
            // Debug.LogWarning("[SpawnOne] POOL EMPTY; INSTANTIATED");
        }

        // ���� ��ġ/����/ȸ�� ����
        Vector3 pos = new Vector3(Random.Range(xRange.x, xRange.y), topY, 0f);
        m.transform.position = pos;
        m.transform.rotation = new Quaternion(0f, 0f, -0.16873014f, 0.985662341f);

        int dirX = randomHorizontal ? (Random.value < 0.5f ? -1 : 1)
                                    : (fixedDirX != 0 ? (int)Mathf.Sign(fixedDirX) : 1);
        Vector3 dir = new Vector3(dirX, -1f, 0f);

        // �ʱ�ȭ & Ȱ��ȭ
        m.Init(this, dir, meteorSpeed);
        m.gameObject.SetActive(true);

        // Ȱ�� ����� "���� �ֽ�"���� ���
        active.AddLast(m);
    }

    public void Despawn(MoonFragmentScript m)
    {
        // Ȱ�� ��Ͽ��� ���� (��� �ֵ� �����ϰ�)
        var node = active.Find(m);
        if (node != null) active.Remove(node);

        m.gameObject.SetActive(false);
        inactive.Enqueue(m); // �ݵ�� ��Ȱ�� ť�� ����
    }

    IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);
        SpawnOne();
    }
}