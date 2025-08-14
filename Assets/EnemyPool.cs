using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;   // EnemyController 프리팹(Enemy 상속)
    public int initialSize = 50;

    private readonly Queue<Enemy> inactive = new();
    private readonly LinkedList<Enemy> active = new(); // 맨 앞이 가장 오래된

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(enemyPrefab, Vector3.one * 9999f, Quaternion.identity);
            go.SetActive(false);
            var e = go.GetComponent<Enemy>();
            e.SetPool(this);               // 풀 참조 주입
            inactive.Enqueue(e);
        }
    }

    public Enemy SpawnAt(Vector3 pos, Quaternion rot)
    {
        Enemy e = null;

        if (inactive.Count > 0)
        {
            e = inactive.Dequeue();
        }
        else if (active.Count > 0)
        {
            // 비활성 없으면 "가장 오래된 활성" 재활용(항상 주기 유지용)
            e = active.First.Value;
            active.RemoveFirst();
            e.gameObject.SetActive(false);
        }
        else
        {
            // 극단적 방어
            e = Instantiate(enemyPrefab).GetComponent<Enemy>();
            e.SetPool(this);
        }

        var tr = e.transform;
        tr.SetPositionAndRotation(pos, rot);

        //  풀에서 꺼낼 때 상태 초기화
        e.ResetForSpawn();

        e.gameObject.SetActive(true);
        active.AddLast(e);
        return e;
    }

    public void Despawn(Enemy e)
    {
        // 활성 목록에서 제거
        var node = active.Find(e);
        if (node != null) active.Remove(node);

        e.gameObject.SetActive(false);
        inactive.Enqueue(e);
    }
}
