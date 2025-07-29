using System.Collections;
using System.Collections.Generic;
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
    public List<Slash> slashList; // ������ ������Ʈ ����Ʈ



    public float maxCheckDistance = 10f;  // �ִ� �˻� �Ÿ�
    public LayerMask groundLayer;        // Ground ���� ���̾� (���û���)
    public float bufferTime = 0.3f; // ���Է� ���� �ð� (��)
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

        if(Input.GetMouseButton(1) && !playerModel.IsAttacking && !playerModel.IsRolling && playerModel.IsGrounded && !playerModel.IsGuarding)
        {
            animator.SetTrigger("Guard");
            playerViewModel.GuardPlayer(animator,rb);
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !playerModel.IsAttacking && !playerModel.IsGuarding && playerModel.IsGrounded)  // �Ϲݰ��� Ʈ����
        {
            // Debug.Log($"���콺 ���� ����");
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

        // �Ʒ� �������� Ray �߻�
        if (Physics.Raycast(transform.position, Vector3.down, out hit, maxCheckDistance,groundLayer))
        {
            if (hit.collider.CompareTag("ground"))
            {
                return hit.distance; // Ground������ �Ÿ� ��ȯ
            }
        }

        return -1f; // Ground�� �������� �ʾ��� ���
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
        playerModel.IsAttacking = true;
        Debug.Log($"EnableDamage called with attack: {attack}");
        AttackModel attackModel = new AttackModel();
        attackModel.Type = AttackType.Basic; // �⺻ ���� Ÿ�� ����
        if (attack == "Parring")
        {
            attackModel.Type = AttackType.Parry;
            playerModel.StartAttack(weaponController, attackModel);
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
         //   StartCoroutine(SlashFX(6));
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

        playerModel.StartAttack(weaponController, attackModel);
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
}
