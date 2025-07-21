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
    public float rotationSpeed = 10f;
    public float h;
    private bool isGrounded = false;
    public float jumpForce = 7f;
    private Rigidbody rb;
    public float fallMultiplier = 1.5f;
    public float lowJumpMultiplier = 1f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        // rotate y 설정 오른쪽 90 왼쪽 270   

        animator.SetFloat("Speed", Mathf.Abs(h));

        player.transform.position = new Vector3(player.transform.position.x + (h * 6f * Time.deltaTime), player.transform.position.y, player.transform.position.z);
        if (h > 0)
        {
            targetRotation = Quaternion.Euler(0, 90, 0); // 오른쪽
        }
        else if (h < 0)
        {
            targetRotation = Quaternion.Euler(0, 270, 0); // 왼쪽
        }
        else if (h == 0)
        {
            targetRotation = Quaternion.Euler(0, 180, 0); // 중앙
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        Vector3 fixedEuler = transform.rotation.eulerAngles;
        fixedEuler.x = 0f;
        fixedEuler.z = 0f;
        transform.rotation = Quaternion.Euler(fixedEuler);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !playerModel.isAttacking)
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0) && !playerModel.isAttacking)
        {
            Debug.Log($"마우스 누름");
            animator.SetTrigger("Attack");
            playerModel.isAttacking = true;
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
        playerModel.isAttacking = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
            isGrounded = true;
    }
}
