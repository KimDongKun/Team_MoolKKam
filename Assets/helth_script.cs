using UnityEngine;

public class helth_script : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} �ǰ�! ���� ü��: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ���!");
        Destroy(gameObject); // �Ǵ� �ִϸ��̼� ó�� ��
    }
}
