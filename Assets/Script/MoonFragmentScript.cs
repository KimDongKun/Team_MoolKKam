using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class MoonFragmentScript : MonoBehaviour
{
    [Header("Move")]
    public float speed = 8f;                // 낙하 속도(전체 크기)
    public Vector3 dir = new Vector3(1, -1, 0); // '/' 방향 기본값(오른쪽 아래)

    private Rigidbody rb;
    private bool stopped;
    private MeteorPool pool;
    public VisualEffect fallEffect;
    public float lifetime = 15f; // 풀에 반납되기 전까지의 생존 시간

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
        // 풀에서 꺼낼 때마다 상태 초기화
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

        // 일정한 대각선 낙하(중력까지 합치면 더 자연스러움)
        Vector3 v = dir.normalized * speed;
        rb.linearVelocity = v + Physics.gravity; // 너무 빠르면 +Physics.gravity를 빼도 됨
    }

    void OnTriggerEnter(Collider col)
    {
        if (stopped) return;

        if (col.CompareTag("ground"))
        {
            StartCoroutine(StayAndDespawn());
            Vector3 spawnPos = col.ClosestPoint(transform.position) + Vector3.up * 0.05f;
            enemyPool.SpawnAt(spawnPos, Quaternion.identity); // ← 적 소환
        }
    }

    IEnumerator StayAndDespawn()
    {
        stopped = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
      //  rb.isKinematic = true; // 완전히 정지
        fallEffect.SetBool("fallin", false);
        yield return new WaitForSeconds(lifetime); // 땅에서 1초 머무름

        // 풀에 반납 -> 풀에서 일정 딜레이 후 다시 공중 스폰
        pool.Despawn(this);
    }
}
