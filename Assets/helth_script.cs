using UnityEngine;

public class helth_script : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    void Start() => currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} 피격! 현재 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} 사망!");
        Destroy(gameObject); // 또는 애니메이션 처리 등
    }
}
