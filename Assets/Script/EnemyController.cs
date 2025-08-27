using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyController : Enemy
{
    public float moveSpeed = 2f;
    public float attackRange = 4.5f;
    public float attackSpeed = 0.05f;
    public float attackCooldown = 3f;
    private float lastAttackTime = -999f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    


    // 🔸 추가: 건물/석상 공격을 위한 설정
    public LayerMask buildingLayer;         // 건물 레이어
    public LayerMask statueLayer;           // 석상 레이어

    public float aggroRadius = 6f;          // 플레이어에 꽂히는 반경
    public float buildingDetectRadius = 3f; // 이동 중 주변 건물 감지 반경

    protected bool isAttacking = false;
    public float stopDistance = 1.5f;
    public Vector3 attackBoxSize;
    public Animator animator;
    public Transform attackAreaObject;
    private Coroutine attackCo;
    public string buildingTag = "Building"; // 건물 태그이름
    public string statueTag = "Statue";  // 석상 태그이름


    // 🔸 추가: 현재 타겟
    private Transform currentTarget;
    private Transform statue;


    // === Targeting Params ===
    public float deaggroRadius = 12f;  // 플레이어를 놓는 거리(히스테리시스)
    public float retargetCooldown = 0.3f; // 과도한 전환 방지

    // === Targeting State ===
    private float lastRetargetAt = -999f;

    // (선택) 빌딩 캐시(Find 호출 줄이기)
    private GameObject[] buildingCache;




    protected override void Start()
    {
        base.Start();
        if (!player) player = GameObject.FindWithTag("Player")?.transform;
        if (!animator) animator = GetComponent<Animator>();
        if (!anim) anim = GetComponent<Animator>();
        if (attackAreaObject) attackBoxSize = attackAreaObject.lossyScale;

        // 🔸 석상 찾기(태그/레이어 중 편한 방식)
        if (!statue)
        {
            var go = GameObject.FindWithTag(statueTag);
            if (go) statue = go.transform;
        }
        StartCoroutine(TargetScanLoop());
        StartCoroutine(SpawnedRoutine());
    }

    void OnEnable()
    {
        if (!player) player = GameObject.FindWithTag("Player")?.transform;
        if (!animator) animator = GetComponent<Animator>();
        if (!anim) anim = GetComponent<Animator>();
        if (attackAreaObject) attackBoxSize = attackAreaObject.lossyScale;

        if (!statue)
        {
            var go = GameObject.FindWithTag(statueTag);
            if (go) statue = go.transform;
        }
        StartCoroutine(TargetScanLoop());
        StartCoroutine(SpawnedRoutine());
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    private IEnumerator TargetScanLoop()
    {
        var wait = new WaitForSeconds(1f);
        while (true)
        {
            buildingCache = GameObject.FindGameObjectsWithTag(buildingTag);
            if (!statue)
            {
                var go = GameObject.FindWithTag(statueTag);
                if (go) statue = go.transform;
            }
            yield return wait;
        }
    }


    private void UpdateTarget()
    {
        if (Time.time < lastRetargetAt + retargetCooldown) return;

        // 현재 타겟 유효성 체크
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            currentTarget = null;

        // 1) 플레이어 어그로/디어그로
        if (player != null)
        {
            float pdist = Mathf.Abs(player.position.x - transform.position.x); // 2D 횡스크롤이면 X만으로도 충분
            bool canAggro = pdist <= aggroRadius;
            bool shouldDrop = pdist > deaggroRadius;

            // 플레이어가 범위에 들어오면 즉시 스위치
            if (canAggro && currentTarget != player)
            {
                currentTarget = player;
                lastRetargetAt = Time.time;
                return;
            }

            // 플레이어를 쫓는 중인데 멀어졌으면 해제
            if (currentTarget == player && shouldDrop)
            {
                currentTarget = null;
                lastRetargetAt = Time.time;
            }
        }

        // 2) 플레이어가 타겟 아니면 주변 건물 중 가장 가까운 것 선택
        if (currentTarget == null)
        {
            Transform nearestBuilding = null;
            float best = float.MaxValue;

            var buildings = buildingCache ?? GameObject.FindGameObjectsWithTag(buildingTag);
            for (int i = 0; i < buildings.Length; i++)
            {
                var t = buildings[i].transform;
                if (!t.gameObject.activeInHierarchy) continue;

                float d = Mathf.Abs(t.position.x - transform.position.x);
                if (d < best)
                {
                    best = d;
                    nearestBuilding = t;
                }
            }

            if (nearestBuilding != null)
            {
                currentTarget = nearestBuilding;
                lastRetargetAt = Time.time;
            }
        }

        // 3) 그래도 없으면 석상 폴백
        if (currentTarget == null && statue != null && statue.gameObject.activeInHierarchy)
        {
            currentTarget = statue;
            lastRetargetAt = Time.time;
        }
    }


    public override void ResetForSpawn()
    {
        base.ResetForSpawn();
        isAttacking = false;
        attackCo = null;
        lastAttackTime = -999f;
        if (attackAreaObject) attackBoxSize = attackAreaObject.lossyScale;
        StopAllCoroutines();
    }

    private IEnumerator SpawnedRoutine()
    {
        anim?.SetTrigger("spawn");
        yield return new WaitForSeconds(1f);
        spawned = false; // 이제 전투 개시
        FlipToTarget();  // 방향 정렬
    }

    public override void ResetStun() => stunned = false;

    // 🔸 타겟 획득 로직 (우선순위: 플레이어(가까우면) > 주변 건물 > 석상)
    private void AcquireTarget()
    {
        Transform t = null;

        // 1) 플레이어 우선
        if (player != null)
        {
            float pdist = Mathf.Abs(player.position.x - transform.position.x);
            if (pdist <= aggroRadius) t = player;
        }

        // 2) 주변 건물 찾기
        if (t == null)
        {
            var buildings = GameObject.FindGameObjectsWithTag(buildingTag);
            float bestDist = float.MaxValue;
            foreach (var b in buildings)
            {
                float d = Mathf.Abs(b.transform.position.x - transform.position.x);
                if (d < bestDist)
                {
                    bestDist = d;
                    t = b.transform;
                }
            }
        }

        // 3) 석상
        if (t == null)
        {
            if (statue == null)
            {
                var go = GameObject.FindWithTag(statueTag);
                if (go) statue = go.transform;
            }
            t = statue;
        }

        currentTarget = t;
    }

    protected override bool ShouldAttack()
    {
        if (spawned || stunned || isAttacking) return false;

        UpdateTarget();
        if (currentTarget == null) return false;

        float distance = Mathf.Abs(currentTarget.position.x - transform.position.x);
        bool inRange = distance <= attackRange;
        bool cooldownReady = Time.time >= lastAttackTime + attackCooldown;
        return inRange && cooldownReady;
    }

    protected override void Attack()
    {
        if (isAttacking || attackCo != null) return;
        attackCo = StartCoroutine(AttackSequence());
        isAttacking = true;
    }

    private IEnumerator AttackSequence()
    {
        animator.SetBool("isMoving", false);
        lastAttackTime = Time.time;

        FlipToTarget();

        animator.ResetTrigger("Attack");
        animator.SetTrigger("AttackCharge");

        yield return new WaitForSeconds(0.5f);
        if (stunned) { isAttacking = false; attackCo = null; yield break; }

        animator.ResetTrigger("AttackCharge");
        animator.SetTrigger("Attack");

        attackCo = null;
    }

    protected override void Move()
    {
        if (isAttacking || stunned || spawned) return;

        UpdateTarget();
        if (currentTarget == null) return;

        float distance = Mathf.Abs(currentTarget.position.x - transform.position.x);
        if (distance <= attackRange)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        float direction = Mathf.Sign(currentTarget.position.x - transform.position.x);
        transform.position += new Vector3(direction * moveSpeed * Time.fixedDeltaTime, 0, 0);

        FlipToTarget();
        animator.SetBool("isMoving", true);
        //animator.Play("Walk_ver_B_Front_L45");

        Vector3 pos = transform.position;
        pos.z = -1.05f;
        transform.position = pos;
    }

    // 🔸 현재 타겟을 기준으로 Flip
    private void FlipToTarget()
    {
        if (currentTarget == null) { AcquireTarget(); }
        var target = currentTarget != null ? currentTarget : (player != null ? player : statue);
        if (target == null) return;

        float dir = Mathf.Sign(target.position.x - transform.position.x);
        // Enemy.Flip()은 player 기준이라 여기서는 직접 회전
        float yRotation = (dir >= 0f) ? 90f : 270f;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    // 🔸 우선순위: Player > Building > Statue
    public void EnableDamage()
    {
        Collider[] hits = Physics.OverlapBox(attackPoint.position, attackBoxSize, attackPoint.rotation);

        Collider playerHit = null;
        Collider buildingHit = null;
        Collider statueHit = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerHit = hit;
                break; // 최우선
            }

            if (hit.CompareTag(buildingTag) && buildingHit == null)
            {
                buildingHit = hit;
            }

            if (hit.CompareTag(statueTag) && statueHit == null)
            {
                statueHit = hit;
            }
        }

        if (playerHit != null)
        {
            var pc = playerHit.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage((int)damage);
            return;
        }

        Collider targetOther = buildingHit != null ? buildingHit : statueHit;
        if (targetOther != null)
        {
            var dmg = targetOther.GetComponent<IDamageable>();
            if (dmg != null) dmg.TakeDamage((int)damage);
        }
    }

    private bool IsInLayerMask(int layer, LayerMask mask)
    {
        return (mask.value & (1 << layer)) != 0;
    }

    public void EndAttack() => isAttacking = false;

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }
}
