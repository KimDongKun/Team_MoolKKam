using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    public PlayerModel playerModel;
    public PlayerViewModel playerViewModel;

    public GameObject player;
    private Quaternion targetRotation;
    public float rotationSpeed = 10f;
    public float h;
    private bool isGrounded = false;
    public float jumpForce = 7f;
    private Rigidbody rb;
    public float fallMultiplier = 1.5f;
    public float lowJumpMultiplier = 1f;
    private Animator animator;
    private Player_Attack_Script pas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerModel = new PlayerModel();
        playerViewModel = new PlayerViewModel(playerModel);
         animator = GetComponent<Animator>();
        pas = GetComponent<Player_Attack_Script>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        // rotate y ¼³Á¤ ¿À¸¥ÂÊ 90 ¿ÞÂÊ 270   

            player.transform.position = new Vector3(player.transform.position.x + (h*6f * Time.deltaTime), player.transform.position.y, player.transform.position.z);
        
            if (h > 0)
        {
            targetRotation = Quaternion.Euler(0, 90, 0); // ¿À¸¥ÂÊ
        }
        else if (h < 0)
        {
            targetRotation = Quaternion.Euler(0, 270, 0); // ¿ÞÂÊ
        }
        else if (h == 0)
        {
            targetRotation = Quaternion.Euler(0, 180, 0); // Áß¾Ó
        }
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        Vector3 fixedEuler = transform.rotation.eulerAngles;
        fixedEuler.x = 0f;
        fixedEuler.z = 0f;
        transform.rotation = Quaternion.Euler(fixedEuler);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !pas.isAttacking)
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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
            isGrounded = true;
    }
}
