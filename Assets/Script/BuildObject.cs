using UnityEngine;
using UnityEngine.UI;

public class BuildObject : MonoBehaviour, IDamageable
{
    public Slider hpUI;
    public int currentHP;
    public int maxHP;

    public bool isInstall = false;

    private void OnEnable()
    {
        currentHP = maxHP;

        hpUI.maxValue = maxHP;
        hpUI.value = maxHP;
    }
    public void GetDamage()
    {
        currentHP -= 1;
        this.hpUI.value = currentHP;

        if (currentHP.Equals(0))
        {
            DistroyObject();
        }

    }
    private void DistroyObject()
    {
        this.gameObject.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        GetDamage();
    }
}
