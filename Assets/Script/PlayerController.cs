using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //[Header("MVVM_DGP")]
    public PlayerModel playerModel;
    public PlayerViewModel playerViewModel;

    [Header("Component")]
    public WeaponController weaponController;
    public Animator animator;

    [Header("Player Data")]
    public GameObject player;
    private Quaternion targetRotation;
    private Rigidbody rb;


    public float maxCheckDistance = 5f;  // 최대 검사 거리
    public LayerMask groundLayer;        // Ground 전용 레이어 (선택사항)
    public float bufferTime = 0.3f; // 선입력 유지 시간 (초)
    private float AttackBufferTimer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        
    }

    // Update is called once per frame
    void Update()
    {
            float h = Input.GetAxisRaw("Horizontal");
        if (h != 0)
        {
            animator.SetBool("PressMove", true);
        }
        else
        {
            animator.SetBool("PressMove", false);

        }
        if (Input.GetMouseButton(0) || (AttackBufferTimer != 0 && AttackBufferTimer < bufferTime))
        {
            animator.SetBool("OnLeftClick", true);
            AttackBufferTimer += Time.deltaTime;
        }
        else
        {
            AttackBufferTimer = 0f; // Reset the buffer timer if the left click is not pressed
            animator.SetBool("OnLeftClick", false);
        }
        if (Input.GetMouseButton(1))
        {
            animator.SetBool("OnRightClick", true);
        }
        else
        {
            animator.SetBool("OnRightClick", false);
        }
        animator.SetBool("IsAttacking", playerModel.IsAttacking);


        if (!playerModel.IsAttacking && !playerModel.IsGuarding && !playerModel.IsRolling ||
    (playerModel.IsAttacking && !playerModel.IsGrounded))
        {
            playerViewModel.MovePlayer(animator, player);   //플레이어 이동함수
        }
        else
        {
            playerModel.IsPlayerMoving = false; // 플레이어가 움직이지 않음
        }
            playerViewModel.JumpPlayer(Input.GetKeyDown(KeyCode.Space), animator, rb); //플레이어 점프함수
 
        if (Input.GetKeyDown(KeyCode.LeftShift) && !playerModel.IsRolling && playerModel.IsPlayerMoving  )
        {
            playerViewModel.RollPlayer(animator,rb); //플레이어 구르기
        }
        if (playerModel.IsRolling)
        {
            playerViewModel.Rolling(player);
        }

        if(Input.GetMouseButton(1) && !playerModel.IsAttacking && !playerModel.IsRolling && playerModel.IsGrounded && !playerModel.IsGuarding)
        {
            animator.SetTrigger("Guard");
            playerViewModel.GuardPlayer(animator,rb);
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && playerModel.IsGrounded)  // 일반공격 트리거
        {
            // Debug.Log($"마우스 왼쪽 누름");
            playerModel.IsAttacking = true;
            animator.SetTrigger("Attack");
            Vector3 direction = playerModel.FacingDirection == 0 ? Vector3.right : Vector3.left;
            rb.linearVelocity = direction * 7f;
           
        }
        float distance = GetDistanceToGround();
        animator.SetFloat("DistanceToGround", distance);
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && !playerModel.IsGrounded && !playerModel.HasJumpAttacked) // 공중공격 트리거
        {
            Debug.Log($"Distance to ground: {distance}");
            if (distance > 3f)
            {
                playerModel.IsAttacking = true;
                playerModel.HasJumpAttacked = true;
                animator.SetTrigger("FlyAttack");
                Vector3 direction = playerModel.FacingDirection == 0 ? Vector3.right : Vector3.left;
            }
        }


    }
    


    float GetDistanceToGround()
    {
        RaycastHit hit;

        // 아래 방향으로 Ray 발사
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxCheckDistance))
        {
            if (hit.collider.CompareTag("ground"))
            {
                return hit.distance; // Ground까지의 거리 반환
            }
        }

        return -1f; // Ground가 감지되지 않았을 경우
    }



public void GarudExit()
    {
        playerModel.IsGuarding = false;
        animator.SetBool("IsGuard", false);
    }
    public string TakeDamage(int damage)
    {
            return    playerViewModel.TakeDamage(damage);
    }

    public void EnableDamage(string attack)
    {
        AttackBufferTimer = 0f;  // 공격 버퍼 타이머 초기화
        playerModel.IsAttacking = true;
        Debug.Log($"EnableDamage called with attack: {attack}");
        AttackModel attackModel = new AttackModel();
        attackModel.Type = AttackType.Basic; // 기본 공격 타입 설정
        if (attack == "Parring")
        {
            attackModel.Type = AttackType.Parry;
            playerModel.StartAttack(weaponController, attackModel);
            return;
        }
        if (attack == "JumpAttack01")
            ApplyJumpAttack(3f, 4f);
        else if (attack == "JumpAttack02")
            ApplyJumpAttack(3f, 9f);
        else if (attack == "JumpAttack03")
            ApplyJumpAttack(3f, 4f);

        playerModel.StartAttack(weaponController, attackModel);
    }

    void ApplyJumpAttack(float xPower, float yPower)
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;

        float dir = (playerModel.FacingDirection == 1) ? -1f : 1f;
        vel.x = dir * xPower;

        rb.linearVelocity = vel;
        rb.AddForce(Vector3.up * yPower, ForceMode.Impulse);
    }

    public void DisableDamage()
    {
        playerModel.IsAttacking = false;
        playerModel.EndAttack(weaponController);
    }
    public void EndAttack()
    {
        playerModel.IsAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
            playerModel.IsGrounded = true;
    }
}
