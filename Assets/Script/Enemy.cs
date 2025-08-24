using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;
    public bool stunned = false;
    public GameObject hpSlider;
    public Animator anim;
    public bool spawned = true;
    private bool facingRight = true;
    public Transform player; // 플레이어 위치 참조
    //  private → protected 로 바꿔서 자식에서 접근 가능
    protected float currentHealth;
    private float stunTime = 2;
    public Slider slider;
    public GameObject dropItem;

    public GameObject nightWaveManager;

    // 풀 참조
    private EnemyPool pool;
    public void SetPool(EnemyPool p) => pool = p;

    protected virtual void Start() => currentHealth = maxHealth;

    //  풀에서 꺼내질 때(스폰 직전) 매번 호출될 초기화 훅
    public virtual void ResetForSpawn()
    {
        nightWaveManager = GameObject.Find("NightWave");
        currentHealth = maxHealth;
        stunned = false;
        spawned = true;               // 스폰 애니메이션 동안 무적/무시
        if (!anim) anim = GetComponent<Animator>();
        if (anim)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.ResetTrigger("Stun");
            anim.ResetTrigger("Attack");
        }
        player = GameObject.FindWithTag("Player")?.transform;
        // HP UI 초기화 필요시 여기서
    }

    public void Flip()
    {
        float dir = Mathf.Sign(player.position.x - transform.position.x);
        if (dir > 0 && !facingRight) facingRight = true;
        else if (dir < 0 && facingRight) facingRight = false;
        float yRotation = facingRight ? 90f : 270f;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    public virtual void TakeDamage(int amount, AttackModel attack)
    {
        if (spawned) return; // 스폰 상태면 데미지 무시
        Flip();
        currentHealth -= amount;
        slider.value = currentHealth;
        Debug.Log($"{gameObject.name} 피격! 현재 체력: {currentHealth} 공격유형 {attack.Type}");
        if (attack.Type == AttackType.Parry || attack.Type == AttackType.Skill)
        {
            anim?.SetTrigger("Stun");
            anim?.Play("Stun");
            stunned = true;
            StartCoroutine(Stun());
            
        }
        if (currentHealth <= 0) Die();
    }

    IEnumerator Stun()
    {
      yield return new WaitForSeconds(stunTime); // 스턴 지속 시간
        stunned = false;
      
    }


    public void FixedUpdate()
    {
        if (!nightWaveManager.GetComponent<NightWaveManager>().IsNight)
        {
            Die();
        }
        if (ShouldAttack()) Attack();
        else Move();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        //  Destroy(gameObject);
        //  풀로 반납
        Vector3 position = transform.position;
        position.y += 1.5f;
        Instantiate(dropItem, position, Quaternion.identity);
        pool?.Despawn(this);
    }

    public abstract void ResetStun();
    protected abstract bool ShouldAttack();
    protected abstract void Attack();
    protected abstract void Move();
}