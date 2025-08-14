using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using UnityEngine.VFX;

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
    public List<Slash> slashList; // 슬래시 오브젝트 리스트

    public VisualEffect weaponEffact; //

    public VisualEffect chargeEffect;


    public float maxCheckDistance = 17f;  // 최대 검사 거리
    public LayerMask groundLayer;        // Ground 전용 레이어 (선택사항)
    public float bufferTime = 0.3f; // 선입력 유지 시간 (초)
    private float AttackBufferTimer = 0f;

    public bool skill1_canSkill = true;
    public bool skill2_canSkill = true; // 스킬 쿨타임 변수
    public bool skill3_canSkill = true; // 스킬 쿨타임 변수    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (col == null)
            col = GetComponent<CapsuleCollider>();
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

        if((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && playerModel.HasParried )  // 패링후 스킬
        {
            playerModel.IsAttacking = true;
            animator.SetTrigger("ParrySkill");
            playerViewModel.UseSkill(animator,rb,"ParryedAttack");
            StartCoroutine(SlashFX(6));
            StartCoroutine(SlashFX(8));
            StartCoroutine(EnableAttack("ParrySkill", 0.05f));

        }
        if(((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && playerModel.IsGuarding  && !playerModel.HasParried && !playerModel.IsAttacking) ||(Input.GetKey(KeyCode.S) && playerModel.IsGuarding && !playerModel.IsAttacking && !playerModel.HasParried)) // 가드 상태 스킬
        {
            if (Input.GetKey(KeyCode.W) && skill2_canSkill)
            {
                playerModel.IsAttacking = true;
                StartCoroutine(EnableAttack("UpperSkill", 0.05f));
                StartCoroutine(SlashFX(4));
                StartCoroutine(SkillCollDown("UpperSkill", 2));
                ApplyJumpAttack(0f, 13f, Vector3.up);
                animator.SetTrigger("UpperSkill");
                skill2_canSkill = false; // 스킬 쿨타임 시작
                playerModel.IsGrounded = false;
            }
            else if(h!= 0 && skill1_canSkill && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
            {

            playerModel.IsAttacking = true;
            playerViewModel.UseSkill(animator, rb, "GuardAttackSkill");
            StartCoroutine(SlashFX(6));
            StartCoroutine(SlashFX(8));
            StartCoroutine(SkillCollDown("GuardAttackSkill",2));    // 나중에 스킬 객체 만들어서 각 쿨타임적용
            StartCoroutine(EnableAttack("GuardAttackSkill", 0.05f));
            skill1_canSkill = false; // 스킬 쿨타임 시작   



            }else if (Input.GetKey(KeyCode.S) && skill3_canSkill && !playerModel.IsChaged)
            {

                int a = playerModel.FacingDirection; // 플레이어가 바라보는 방향 0: 오른쪽, 1: 왼쪽
                chargeEffect.SetInt("face",a );
                playerModel.IsAttacking = true;
                //skill3_canSkill = false; // 스킬 쿨타임 시작   
                animator.SetTrigger("ChargeAttackSkill");
                playerModel.IsChaged = true;
                skill3_canSkill = false;
                playerModel.chargeTime = 0f; // 차지 시간 초기화
                playerModel.currentLevel = -1; // 차지 레벨 초기화
                chargeEffect.SetBool("Attack", false);

            }


                
            
        }

        if (playerModel.IsChaged)  // 차지공격 상태코드
        {
            playerModel.chargeTime += Time.deltaTime;

            // 단계 계산 (1~3단계)
            int newLevel = 0;
            if (playerModel.chargeTime >= playerModel.maxChargeTime * 1f) newLevel = 3;
            else if (playerModel.chargeTime >= playerModel.maxChargeTime * 0.65f) newLevel = 2;
            else if (playerModel.chargeTime >= playerModel.maxChargeTime * 0.3f) newLevel = 1;
            else if (playerModel.chargeTime >= playerModel.maxChargeTime * 0.2f) newLevel = 0;

            // 단계가 바뀌면 움찔 애니메이션 1회 재생
            if (newLevel != playerModel.currentLevel)
            {
                if (newLevel == 0)
                {
                    chargeEffect.SetBool("Charge0", true);
                }
                else if  (newLevel == 1)
                {
                    chargeEffect.SetBool("Charge1", true);
                }
                else if (newLevel == 2)
                {
                    chargeEffect.SetBool("Charge2", true);
                }
                else if (newLevel == 3)
                {
                    chargeEffect.SetBool("Charge3", true);
                }
                playerModel.currentLevel = newLevel;
                PlayChargeShakeOnce(newLevel);
            }

            if (Input.GetMouseButton(0))
            {
                Debug.Log($"ChargeAttackSkill: Charge Level {playerModel.currentLevel} - Time: {playerModel.chargeTime}");
                if (newLevel == 3)
                {
                   animator.SetTrigger("ChargeAttackSkill_Attack");
                
                }
                else
                {

                    animator.SetTrigger("ChargeAttackSkill_Attack");
                }
                StartCoroutine(EnableAttack("ChargeAttackSkill", 0.05f));
                StartCoroutine(SkillCollDown("ChargeAttackSkill", 2));
                animator.speed = 1f;
                playerModel.IsChaged = false; // 차지 상태 종료
                chargeEffect.SetBool("Charge0", false);
                chargeEffect.SetBool("Charge1", false);
                chargeEffect.SetBool("Charge2", false);
                chargeEffect.SetBool("Charge3", false);
                chargeEffect.SetBool("Attack", true);

            }
        }



        if (Input.GetMouseButton(1) && !playerModel.IsAttacking && !playerModel.IsRolling && playerModel.IsGrounded && !playerModel.IsGuarding)
        {
            animator.SetTrigger("Guard");
            playerViewModel.GuardPlayer(animator,rb);
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && playerModel.IsGrounded)  // 일반공격 트리거
        {
       
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
            if (distance > 2f)
            {
                playerModel.IsAttacking = true;
                playerModel.HasJumpAttacked = true;
                animator.SetTrigger("FlyAttack");
                Vector3 direction = playerModel.FacingDirection == 0 ? Vector3.right : Vector3.left;
            }
        }


    }

    private void PlayChargeShakeOnce(int index)
    {
        if (index == 1)
        {
            animator.speed = 1f;
            animator.Play("Charge2", 0, 0.2f);
        }
        else if (index == 2)
        {
            animator.speed = 1f;
            animator.Play("Charge2", 0, 0.2f);
        }else
        if (index == 3)
        {
            animator.speed = 1f;
            animator.Play("Charge2", 0, 0.2f);
        }
        else if (index == 0)
        {
            animator.speed = 1f;
            animator.Play("Charge2", 0, 0.2f);
        }
        //  StartCoroutine(StopAfterAnimation(0.2f)); // 움찔 애니메이션 길이만큼 재생
    }

    private IEnumerator StopAfterAnimation(float duration)
    {
        yield return new WaitForSeconds(duration);
        animator.speed = 0f; // 다시 첫 프레임에서 멈춤
    }



    private CapsuleCollider col;
    private void OnDrawGizmos()
    {
        if (col == null) return;

        Vector3 footPos = col.bounds.center;
        footPos.y = col.bounds.min.y + 0.05f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(footPos, footPos + Vector3.down * maxCheckDistance);
        Gizmos.DrawWireSphere(footPos + Vector3.down * maxCheckDistance, 0.1f);
    }
    private float lastDistance = 0f;

    float GetDistanceToGround()
    {
        if (col == null) return lastDistance;

        RaycastHit hit;

        // 발바닥 위치 계산 (Collider 기준)
        Vector3 footPos = col.bounds.center;
        footPos.y = col.bounds.min.y + 0.05f;  // 살짝 위에서 시작

        // SphereCast 사용 (두께 있는 레이)
        if (Physics.SphereCast(footPos, 0.1f, Vector3.down, out hit, maxCheckDistance, groundLayer))
        {
            if (hit.collider.CompareTag("ground"))
            {
                lastDistance = hit.distance;
                return hit.distance;
            }
        }

        // 감지 실패 시 이전 값 유지
        return lastDistance;
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
    
    IEnumerator DisAbleHasParryed()
    {
        yield return new WaitForSeconds(0.3f);
        playerModel.HasParried = false;
    }

    IEnumerator SkillCollDown(string type,float time)
    {
        yield return new WaitForSeconds(time);
        if(type == "UpperSkill")
        {
           skill2_canSkill = true;
        }
        else if (type == "GuardAttackSkill")
        {
        skill1_canSkill = true;
            
        }else if( type  == "ChargeAttackSkill")
        {
            skill3_canSkill = true;
        }
    }

    public void EnableDamage(string attack)
    {
        weaponEffact.SetBool("Attack", true);
        playerModel.IsAttacking = true;
       // Debug.Log($"EnableDamage called with attack: {attack}");
        AttackModel attackModel = new AttackModel();
        attackModel.Type = AttackType.Basic; // 기본 공격 타입 설정
        attackModel.Range = weaponController.DefaultRange; // 기본 공격 범위 설정
        if (attack == "Parring")
        {
            attackModel.Type = AttackType.Parry;
            playerModel.StartAttack(weaponController, attackModel);
            playerModel.HasParried = true;
            StartCoroutine(SlashFX(0));
            StartCoroutine(DisAbleHasParryed());
            StartCoroutine(SlashFX(9));
            StartCoroutine(EndAttack(0.3f, 1.5f)); // 공격 종료 후 히트박스 비활성화
            return;
        }
        if (attack == "Combo_1")
        {
            StartCoroutine(SlashFX(0));


        }
        else if (attack == "Combo_2")
        {
            StartCoroutine(SlashFX(1));
        }
        else if (attack == "Combo_3")
        {
            StartCoroutine(SlashFX(2));
        }
        if (attack == "JumpAttack01")
        {
            ApplyJumpAttack(3f, 45f, Vector3.down);
            StartCoroutine(SlashFX(3));
            StartCoroutine(SlashFX(7));
            attackModel.Range.y = 1.5f;
        }
        else if (attack == "JumpAttack02")
        {

            ApplyJumpAttack(3f, 9f, Vector3.up);
            StartCoroutine(SlashFX(4));
            attackModel.Range.y = 2f;
        }
        else if (attack == "JumpAttack03")
        {
            ApplyJumpAttack(3f, 4f, Vector3.up);
            StartCoroutine(SlashFX(5));
            attackModel.Range.y = 1.5f;
        }
        else if (attack == "ParrySkill")
        {

        }
        else if (attack == "GuardAttackSkill")
        {
            attackModel.Type = AttackType.Skill; // 스킬 공격 타입 설정
            playerModel.StartAttack(weaponController, attackModel);
            StartCoroutine(EndAttack(0.3f, 1.5f)); // 공격 종료 후 히트박스 비활성화
            return;
        }
        else if (attack == "UpperSkill")
        {
            attackModel.Type = AttackType.Skill; // 스킬 공격 타입 설정
            attackModel.Range = new Vector3(1f, 1f, 1.4f); // 상향 공격 범위 설정
        }
        else if (attack == "ChargeAttackSkill")
        {
            attackModel.Type = AttackType.Skill; // 차지 공격 타입 설정
            if(playerModel.currentLevel == 0)
            {
                StartCoroutine(SlashFX(10));
                Vector3 range = new Vector3(1f, 1f, 2f);
                attackModel.Range = range; // 차지 공격 범위 설정
            }
            else if (playerModel.currentLevel == 1)
            {
                Debug.Log($"ChargeAttackSkill: Charge Level {playerModel.currentLevel} - Time: {playerModel.chargeTime}");
                StartCoroutine(SlashFX(10));
                Vector3 range = new Vector3(1f, 1f, 2f);
                attackModel.Range = range; // 차지 공격 범위 설정
               // chargeEffect.Reinit();
            }
            else if (playerModel.currentLevel == 2)
            {
                Debug.Log($"ChargeAttackSkill: Charge Level {playerModel.currentLevel} - Time: {playerModel.chargeTime}");
                StartCoroutine(SlashFX(11));

                Vector3 range = new Vector3(1f, 1f, 3f);
                attackModel.Range = range; // 차지 공격 범위 설정
              //  chargeEffect.Reinit();
            }
            else if (playerModel.currentLevel == 3)
            {
                Debug.Log($"ChargeAttackSkill: Charge Level {playerModel.currentLevel} - Time: {playerModel.chargeTime}");

                Vector3 range = new Vector3(1f, 1f, 4f);
                attackModel.Range = range; // 차지 공격 범위 설정
                StartCoroutine(SlashFX(12));
              //  chargeEffect.Reinit();
            }

        }
           
            playerModel.StartAttack(weaponController, attackModel);
            StartCoroutine(EndAttack(0.1f, 1.2f)); // 공격 종료 후 히트박스 비활성화

    }

    IEnumerator EnableAttack(string type,float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableDamage(type);
    }

    IEnumerator EndAttack(float time,float delay)   // 플레이어 공격 히트박스 적용 시간
    {
               yield return new WaitForSeconds(delay);
     //   playerModel.IsAttacking = false;
             yield return new WaitForSeconds(time);
        playerModel.EndAttack(weaponController);
        weaponEffact.SetBool("Attack", false);
    }

    IEnumerator SlashFX(int index)
    {
        slashList[index].slashObject.SetActive(true);


        yield return new WaitForSeconds(0.3f);
        slashList[index].slashObject.SetActive(false);
    }

    void ApplyJumpAttack(float xPower, float yPower,Vector3 dirY)
    {
        Vector3 vel = rb.linearVelocity;
        vel.y = 0f;

        float dir = (playerModel.FacingDirection == 1) ? -1f : 1f;
        vel.x = dir * xPower;

        if (dirY == Vector3.down)
        {
            vel.y = -yPower; // 아래로 순간 속도 설정
            rb.linearVelocity = vel;
        }
        else
        {
            rb.linearVelocity = vel;
        rb.AddForce(dirY * yPower, ForceMode.Impulse);

        }
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

[System.Serializable]
public class Slash
{
    public GameObject slashObject; // 슬래시 오브젝트
    public float delay; // 슬래시 지속 시간
    public Vector3 atPlayerPotion; // 플레이어 위치에서의 슬래시 위치
}
