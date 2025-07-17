using UnityEngine;

public class Player_Attack_Script : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Animator animator;
    public damege_triger_script weaponTrigger;
    public bool isAttacking = false;
    void Start()
    {
    }
    public void EnableDamage()
    {
        weaponTrigger.EnableDamage();
    }

    public void DisableDamage()
    {
        weaponTrigger.DisableDamage();
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            Debug.Log($"마우스 누름");
            animator.SetTrigger("Attack");
            isAttacking = true;
        }

    }

    public void EndAttack()
    {
        isAttacking = false;
    }
}
