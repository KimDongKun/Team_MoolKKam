using UnityEngine;

public class Player_Animation_Script : MonoBehaviour
{
    public Animator animator;

    private Vector3 lastPosition;
    private float speed;
    private bool isAttacking = false;
    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        // 직접 속도 계산
        Vector3 delta = transform.position - lastPosition;
        speed = new Vector3(delta.x, 0, delta.z).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        animator.SetFloat("Speed", speed);

   

    }
}
