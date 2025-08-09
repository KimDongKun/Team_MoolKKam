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
    protected bool isAttacking = false;
    public float stopDistance = 1.5f;
    public Vector3 attackBoxSize;
    public Animator animator;
    public Transform attackAreaObject;
    private Coroutine attackCo;
    

    protected override void Start()
    {
        base.Start();
        if (!player) player = GameObject.FindWithTag("Player")?.transform;
        if (!animator) animator = GetComponent<Animator>();
        if (!anim) anim = GetComponent<Animator>();
        if (attackAreaObject) attackBoxSize = attackAreaObject.lossyScale;
        
        //  풀에서 첫 스폰도 고려해 Start에서 한 번 호출돼도 OK
        StartCoroutine(SpawnedRoutine());
    }

    void OnEnable()
    {
        // 풀에서 재활성화될 때 참조 보정
        if (!player) player = GameObject.FindWithTag("Player")?.transform;
        if (!animator) animator = GetComponent<Animator>();
        if (!anim) anim = GetComponent<Animator>();
        if (attackAreaObject) attackBoxSize = attackAreaObject.lossyScale;
        StartCoroutine(SpawnedRoutine()); //  스폰 연출 후 spawned=false
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
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
        Flip();
    }

    public override void ResetStun() => stunned = false;

    protected override bool ShouldAttack()
    {
        if (player == null || spawned) return false;
        if (stunned || isAttacking) return false;
        float distance = Mathf.Abs(player.position.x - transform.position.x);
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

        Flip();

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
        if (player == null || isAttacking || stunned || spawned) return;

        float distance = Mathf.Abs(player.position.x - transform.position.x);
        if (distance <= attackRange)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        transform.position += new Vector3(direction * moveSpeed * Time.fixedDeltaTime, 0, 0);

        Flip();

        animator.SetBool("isMoving", true);

        Vector3 pos = transform.position;
        pos.z = -1.05f;
        transform.position = pos;
    }


    public void EnableDamage()
    {
        Collider[] hits = Physics.OverlapBox(attackPoint.position, attackBoxSize, attackPoint.rotation);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerCtrl = hit.GetComponent<PlayerController>();
                if (playerCtrl != null)
                {
                    string type = playerCtrl.TakeDamage((int)damage);
                    Debug.Log($"Enemy dealt {damage} damage to player. Type: {type}");
                }
            }
        }
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
