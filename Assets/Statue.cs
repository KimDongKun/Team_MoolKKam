using UnityEngine;
using UnityEngine.UI;

public class Statue : MonoBehaviour, IDamageable
{
    public int maxHp = 100;
    public int hp;
    public Slider hpSlider;

    void OnEnable()
    {
       hp = maxHp;
        if (hpSlider) { hpSlider.maxValue = maxHp; hpSlider.value = hp; }
    }

    public void TakeDamage(int amount)
    {
        if (hp <= 0) return;
        hp = Mathf.Max(0, hp - amount);
        if (hpSlider) hpSlider.value = hp;

        if (hp <= 0)
        {
            // 게임오버 처리
            // GameManager.Instance.GameOver();
        }
    }
}
