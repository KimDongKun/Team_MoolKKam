using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    public GameObject enemyPrefab;   // EnemyController ������(Enemy ���)
    public int initialSize = 50;

    private readonly Queue<Enemy> inactive = new();
    private readonly LinkedList<Enemy> active = new(); // �� ���� ���� ������

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            var go = Instantiate(enemyPrefab, Vector3.one * 9999f, Quaternion.identity);
            go.SetActive(false);
            var e = go.GetComponent<Enemy>();
            e.SetPool(this);               // Ǯ ���� ����
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
            // ��Ȱ�� ������ "���� ������ Ȱ��" ��Ȱ��(�׻� �ֱ� ������)
            e = active.First.Value;
            active.RemoveFirst();
            e.gameObject.SetActive(false);
        }
        else
        {
            // �ش��� ���
            e = Instantiate(enemyPrefab).GetComponent<Enemy>();
            e.SetPool(this);
        }

        var tr = e.transform;
        tr.SetPositionAndRotation(pos, rot);

        //  Ǯ���� ���� �� ���� �ʱ�ȭ
        e.ResetForSpawn();

        e.gameObject.SetActive(true);
        active.AddLast(e);
        return e;
    }

    public void Despawn(Enemy e)
    {
        // Ȱ�� ��Ͽ��� ����
        var node = active.Find(e);
        if (node != null) active.Remove(node);

        e.gameObject.SetActive(false);
        inactive.Enqueue(e);
    }
}
