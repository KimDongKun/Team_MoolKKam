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
    public Transform player; // �÷��̾� ��ġ ����
    //  private �� protected �� �ٲ㼭 �ڽĿ��� ���� ����
    protected float currentHealth;
    private float stunTime = 2;
    public Slider slider;
    public GameObject dropItem;

    public GameObject nightWaveManager;

    // Ǯ ����
    private EnemyPool pool;
    public void SetPool(EnemyPool p) => pool = p;

    protected virtual void Start() => currentHealth = maxHealth;

    //  Ǯ���� ������ ��(���� ����) �Ź� ȣ��� �ʱ�ȭ ��
    public virtual void ResetForSpawn()
    {
        nightWaveManager = GameObject.Find("NightWave");
        currentHealth = maxHealth;
        stunned = false;
        spawned = true;               // ���� �ִϸ��̼� ���� ����/����
        if (!anim) anim = GetComponent<Animator>();
        if (anim)
        {
            anim.Rebind();
            anim.Update(0f);
            anim.ResetTrigger("Stun");
            anim.ResetTrigger("Attack");
        }
        player = GameObject.FindWithTag("Player")?.transform;
        // HP UI �ʱ�ȭ �ʿ�� ���⼭
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
        if (spawned) return; // ���� ���¸� ������ ����
        Flip();
        currentHealth -= amount;
        slider.value = currentHealth;
        Debug.Log($"{gameObject.name} �ǰ�! ���� ü��: {currentHealth} �������� {attack.Type}");
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
      yield return new WaitForSeconds(stunTime); // ���� ���� �ð�
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
        Debug.Log($"{gameObject.name} ���!");
        //  Destroy(gameObject);
        //  Ǯ�� �ݳ�
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