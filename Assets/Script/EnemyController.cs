using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : Enemy
{
    private bool facingRight = true;
    private Transform player;
    public float moveSpeed = 2f;
    public float attackRange = 4.5f;
    public float attackCooldown = 5f;
    private float lastAttackTime = -999f;
    public Transform attackPoint;
    public LayerMask playerLayer;
    protected bool isAttacking = false;
    public float stopDistance = 1.5f;
    public Vector3 attackBoxSize;  // 박스 크기 (가로, 세로)
    public Animator animator;
    public Transform attackAreaObject;
    protected override void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player")?.transform;
        animator = GetComponent<Animator>();
        playerLayer = LayerMask.GetMask("Default");
        attackBoxSize = attackAreaObject.lossyScale;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        
    }

    public override void ResetStun()
    {
         stunned = false;
    }
    protected override bool ShouldAttack()
    {
        if (player == null) return false;
        if (stunned || isAttacking) return false;
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        bool inRange = distance <= attackRange;
        //Debug.Log($"Distance to player: {distance}, In range: {inRange}");
        bool cooldownReady = Time.time >= lastAttackTime + attackCooldown;

        return inRange && cooldownReady;
    }
    protected override void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        if (direction > 0 && !facingRight)
            Flip();
        else if (direction < 0 && facingRight)
            Flip();
    }
    protected override void Move()
    {
        if (player == null || isAttacking || stunned) return;
        float distance = Mathf.Abs(player.position.x - transform.position.x);
        if (distance <= stopDistance)
        {
            animator.SetBool("isMoving", false);
            return;
        }
        // 플레이어와 적의 X축 거리
        float direction = Mathf.Sign(player.position.x - transform.position.x);

        // 방향으로 이동
        transform.position += new Vector3(direction * moveSpeed * Time.fixedDeltaTime, 0, 0);

        // 바라보는 방향 반전 (좌우 뒤집기)
        if (direction > 0 && !facingRight )
            Flip();
        else if (direction < 0 && facingRight)
            Flip();

        // 애니메이션 연동
        animator.SetBool("isMoving", true);
        Vector3 pos = transform.position;
        pos.z = -1.05f; // 원하는 Z값
        transform.position = pos;
    }
    private void Flip()
    {
        
            facingRight = !facingRight;
            float yRotation = facingRight ? 90f : 270f;
            transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        
    }
    public void EnableDamage()
    {
        Collider[] hits = Physics.OverlapBox(attackPoint.position, attackBoxSize, attackPoint.rotation);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))  // 태그로 필터링
            {
                PlayerController player = hit.GetComponent<PlayerController>();
                if (player != null)
                {
                  string type = player.TakeDamage((int)damage);
                  Debug.Log($"Enemy dealt {damage} damage to player. Type: {type}");
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(attackPoint.position, attackPoint.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, attackBoxSize);
    }
    public void EndAttack()
    {
        isAttacking = false;
    }
}
