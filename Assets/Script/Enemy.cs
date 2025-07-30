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
        Debug.Log($"{gameObject.name} 피격! 현재 체력: {currentHealth} 공격유형 {attack.Type}");
        if(attack.Type == AttackType.Parry)
        {
            Debug.Log($"{gameObject.name} 파링 성공!");
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
        Debug.Log($"{gameObject.name} 사망!");
        Destroy(gameObject); // 또는 애니메이션 처리 등
    }
    public abstract void ResetStun(); // 공격조건 만족할시 때리기
    protected abstract bool ShouldAttack(); // 공격조건 만족할시 때리기
    protected abstract void Attack(); // 공격(애니메이션같은거 넣으면될듯)
    protected abstract void Move(); // 이동 또는 추적
}
