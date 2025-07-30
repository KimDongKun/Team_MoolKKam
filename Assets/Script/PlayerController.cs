using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
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
    public List<Slash> slashList; // ������ ������Ʈ ����Ʈ

    



    public float maxCheckDistance = 17f;  // �ִ� �˻� �Ÿ�
    public LayerMask groundLayer;        // Ground ���� ���̾� (���û���)
    public float bufferTime = 0.3f; // ���Է� ���� �ð� (��)
    private float AttackBufferTimer = 0f;

    public bool skill1_canSkill = true;
    public bool skill2_canSkill = true; // ��ų ��Ÿ�� ����
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
            playerViewModel.MovePlayer(animator, player);   //�÷��̾� �̵��Լ�
        }
        else
        {
            playerModel.IsPlayerMoving = false; // �÷��̾ �������� ����
        }
            playerViewModel.JumpPlayer(Input.GetKeyDown(KeyCode.Space), animator, rb); //�÷��̾� �����Լ�
 
        if (Input.GetKeyDown(KeyCode.LeftShift) && !playerModel.IsRolling && playerModel.IsPlayerMoving  )
        {
            playerViewModel.RollPlayer(animator,rb); //�÷��̾� ������
        }
        if (playerModel.IsRolling)
        {
            playerViewModel.Rolling(player);
        }

        if((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && playerModel.HasParried )  // �и��� ��ų
        {
            playerModel.IsAttacking = true;
            animator.SetTrigger("ParrySkill");
            playerViewModel.UseSkill(animator,rb,"ParryedAttack");
            StartCoroutine(SlashFX(6));
            StartCoroutine(SlashFX(8));
            StartCoroutine(EnableAttack("ParrySkill", 0.05f));

        }
        if((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && playerModel.IsGuarding  && !playerModel.HasParried && !playerModel.IsAttacking) // ���� ���� ��ų
        {
            if (Input.GetKey(KeyCode.W) && skill2_canSkill)
            {
                playerModel.IsAttacking = true;
                StartCoroutine(EnableAttack("UpperSkill", 0.05f));
                StartCoroutine(SlashFX(4));
                StartCoroutine(SkillCollDown("UpperSkill", 2));
                ApplyJumpAttack(0f, 13f, Vector3.up);
                animator.SetTrigger("UpperSkill");
                skill2_canSkill = false; // ��ų ��Ÿ�� ����
                playerModel.IsGrounded = false;
            }
            else if(h!= 0 && skill1_canSkill)
            {

            playerModel.IsAttacking = true;
            playerViewModel.UseSkill(animator, rb, "GuardAttackSkill");
            StartCoroutine(SlashFX(6));
            StartCoroutine(SlashFX(8));
            StartCoroutine(EnableAttack("UpperSkill", 0.05f));
            StartCoroutine(SkillCollDown("GuardAttackSkill",2));    // ���߿� ��ų ��ü ���� �� ��Ÿ������
            StartCoroutine(EnableAttack("GuardAttackSkill", 0.05f));
            skill1_canSkill = false; // ��ų ��Ÿ�� ����   

            }


                
            
        }

        if(Input.GetMouseButton(1) && !playerModel.IsAttacking && !playerModel.IsRolling && playerModel.IsGrounded && !playerModel.IsGuarding)
        {
            animator.SetTrigger("Guard");
            playerViewModel.GuardPlayer(animator,rb);
        }


        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && playerModel.IsGrounded)  // �Ϲݰ��� Ʈ����
        {
       
            playerModel.IsAttacking = true;
            animator.SetTrigger("Attack");
            Vector3 direction = playerModel.FacingDirection == 0 ? Vector3.right : Vector3.left;
            rb.linearVelocity = direction * 7f;
           
        }
        float distance = GetDistanceToGround();
        animator.SetFloat("DistanceToGround", distance);
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && !playerModel.IsGrounded && !playerModel.HasJumpAttacked) // ���߰��� Ʈ����
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

        // �߹ٴ� ��ġ ��� (Collider ����)
        Vector3 footPos = col.bounds.center;
        footPos.y = col.bounds.min.y + 0.05f;  // ��¦ ������ ����

        // SphereCast ��� (�β� �ִ� ����)
        if (Physics.SphereCast(footPos, 0.1f, Vector3.down, out hit, maxCheckDistance, groundLayer))
        {
            if (hit.collider.CompareTag("ground"))
            {
                lastDistance = hit.distance;
                return hit.distance;
            }
        }

        // ���� ���� �� ���� �� ����
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
        yield return new WaitForSeconds(1f);
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
            
        }
    }

    public void EnableDamage(string attack)
    {
        playerModel.IsAttacking = true;
        Debug.Log($"EnableDamage called with attack: {attack}");
        AttackModel attackModel = new AttackModel();
        attackModel.Type = AttackType.Basic; // �⺻ ���� Ÿ�� ����
        if (attack == "Parring")
        {
            attackModel.Type = AttackType.Parry;
            playerModel.StartAttack(weaponController, attackModel);
            playerModel.HasParried = true;        
            StartCoroutine(SlashFX(0));
            StartCoroutine(DisAbleHasParryed());
            StartCoroutine(SlashFX(9));
            return;
        }
        if(attack == "Combo_1")
        {
            StartCoroutine(SlashFX(0));
       

        }else if(attack == "Combo_2")
        {
            StartCoroutine(SlashFX(1));
        }else if(attack == "Combo_3")
        {
            StartCoroutine(SlashFX(2));
        }
        if (attack == "JumpAttack01")
        {
            ApplyJumpAttack(3f, 45f, Vector3.down);
            StartCoroutine(SlashFX(3));
            StartCoroutine(SlashFX(7));
        }
        else if (attack == "JumpAttack02")
        {

            ApplyJumpAttack(3f, 9f, Vector3.up);
            StartCoroutine(SlashFX(4));
        }
        else if (attack == "JumpAttack03")
        {
            ApplyJumpAttack(3f, 4f, Vector3.up);
            StartCoroutine(SlashFX(5));
        }
        else if (attack == "ParrySkill")
        {

        }else if (attack == "GuardAttackSkill")
        {
            attackModel.Type = AttackType.Skill; // ��ų ���� Ÿ�� ����
            playerModel.StartAttack(weaponController, attackModel);
            StartCoroutine(EndAttack(0.3f,1.5f)); // ���� ���� �� ��Ʈ�ڽ� ��Ȱ��ȭ
            return;
        }else if (attack == "UpperSkill")
        {
            
        }


        playerModel.StartAttack(weaponController, attackModel);
        StartCoroutine(EndAttack(0.1f,1.2f)); // ���� ���� �� ��Ʈ�ڽ� ��Ȱ��ȭ

    }

    IEnumerator EnableAttack(string type,float delay)
    {
        yield return new WaitForSeconds(delay);
        EnableDamage(type);
    }

    IEnumerator EndAttack(float time,float delay)   // �÷��̾� ���� ��Ʈ�ڽ� ���� �ð�
    {
               yield return new WaitForSeconds(delay);
     //   playerModel.IsAttacking = false;
             yield return new WaitForSeconds(time);
        playerModel.EndAttack(weaponController);
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
            vel.y = -yPower; // �Ʒ��� ���� �ӵ� ����
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
    public GameObject slashObject; // ������ ������Ʈ
    public float delay; // ������ ���� �ð�
    public Vector3 atPlayerPotion; // �÷��̾� ��ġ������ ������ ��ġ
}
