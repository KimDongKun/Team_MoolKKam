using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float damage = 10f;

    public GameObject hpSlider;
    public Animator anim;

    private float currentHealth;

    protected virtual void Start() => currentHealth = maxHealth;

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} �ǰ�! ���� ü��: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ���!");
        Destroy(gameObject); // �Ǵ� �ִϸ��̼� ó�� ��
    }

    protected abstract bool ShouldAttack(); // �������� �����ҽ� ������
    protected abstract void Attack(); // ����(�ִϸ��̼ǰ����� ������ɵ�)
    protected abstract void Move(); // �̵� �Ǵ� ����
}
