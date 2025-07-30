using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public bool stunned = false;
    public GameObject hpSlider;
    public Animator anim;


    private float currentHealth;

    protected virtual void Start() => currentHealth = maxHealth;

    public virtual void TakeDamage(int amount, AttackModel attack)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} �ǰ�! ���� ü��: {currentHealth} �������� {attack.Type}");
        if(attack.Type == AttackType.Parry)
        {
            Debug.Log($"{gameObject.name} �ĸ� ����!");
            anim.SetTrigger("Stun");
            stunned = true;
        }
        if(attack.Type == AttackType.Skill)
        {
            anim.SetTrigger("Stun");
            stunned = true;

        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void FixedUpdate()
    {
       
        if (ShouldAttack())
        {
            Attack();
        }
        else
        {
            Move();
        }
    }
    
    
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ���!");
        Destroy(gameObject); // �Ǵ� �ִϸ��̼� ó�� ��
    }
    public abstract void ResetStun(); // �������� �����ҽ� ������
    protected abstract bool ShouldAttack(); // �������� �����ҽ� ������
    protected abstract void Attack(); // ����(�ִϸ��̼ǰ����� ������ɵ�)
    protected abstract void Move(); // �̵� �Ǵ� ����
}
