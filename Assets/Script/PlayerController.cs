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

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerModel.IsAttacking)
        {
            playerViewModel.MovePlayer(animator, player);   //�÷��̾� �̵��Լ�
        }
        else
        {
            playerModel.IsPlayerMoving = false; // �÷��̾ �������� ����
        }
            playerViewModel.JumpPlayer(Input.GetKeyDown(KeyCode.Space), animator, rb); //�÷��̾� �����Լ�
            float h = Input.GetAxisRaw("Horizontal");
        if(h != 0) {
            animator.SetBool("PressMove", true);
        }
        else {
        animator.SetBool("PressMove",false);
          
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !playerModel.IsRolling && playerModel.IsPlayerMoving  )
        {
            playerViewModel.RollPlayer(animator,rb); //�÷��̾� ������
        }
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("OnLeftClick", true);
        }
        else
        {
            animator.SetBool("OnLeftClick", false);
        }
        if (Input.GetMouseButtonDown(0) && !playerModel.IsAttacking)
        {
            Debug.Log($"���콺 ����");
            animator.SetTrigger("Attack");
            playerModel.IsAttacking = true;
            Vector3 direction = playerModel.FacingDirection == 0 ? Vector3.right : Vector3.left;
            rb.linearVelocity += direction * 7f;
        }

    }
    public void EnableDamage()
    {
        playerModel.StartAttack(weaponController);
    }

    public void DisableDamage()
    {
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
