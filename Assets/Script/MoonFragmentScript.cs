using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MoonFragmentScript : MonoBehaviour
{
    [Header("Move")]
    public float speed = 8f;                // ���� �ӵ�(��ü ũ��)
    public Vector3 dir = new Vector3(1, -1, 0); // '/' ���� �⺻��(������ �Ʒ�)

    private Rigidbody rb;
    private bool stopped;
    private MeteorPool pool;
    public VisualEffect fallEffect;
    public float lifetime = 15f; // Ǯ�� �ݳ��Ǳ� �������� ���� �ð�

    [Header("Enemy Pool")]
    public EnemyPool enemyPool;


    public void Init(MeteorPool poolRef, Vector3 dropDir, float spd)
    {
        pool = poolRef;
        dir = dropDir;
        speed = spd;
        ResetState();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!enemyPool) enemyPool = FindAnyObjectByType<EnemyPool>();
    }

    void OnEnable()
    {
        // Ǯ���� ���� ������ ���� �ʱ�ȭ
        ResetState();
    }

    void ResetState()
    {
        stopped = false;
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        fallEffect.SetBool("fallin",true);
        rb.useGravity = true;
    }

    void FixedUpdate()
    {
        if (stopped) return;

        // ������ �밢�� ����(�߷±��� ��ġ�� �� �ڿ�������)
        Vector3 v = dir.normalized * speed;
        rb.linearVelocity = v + Physics.gravity; // �ʹ� ������ +Physics.gravity�� ���� ��
    }

    void OnTriggerEnter(Collider col)
    {
        if (stopped) return;

        if (col.CompareTag("ground"))
        {
            StartCoroutine(StayAndDespawn());
            Vector3 spawnPos = col.ClosestPoint(transform.position) + Vector3.up * 0.05f;
            enemyPool.SpawnAt(spawnPos, Quaternion.identity); // �� �� ��ȯ
        }
    }

    IEnumerator StayAndDespawn()
    {
        stopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
      //  rb.isKinematic = true; // ������ ����
        fallEffect.SetBool("fallin", false);
        yield return new WaitForSeconds(lifetime); // ������ 1�� �ӹ���

        // Ǯ�� �ݳ� -> Ǯ���� ���� ������ �� �ٽ� ���� ����
        pool.Despawn(this);
    }
}
