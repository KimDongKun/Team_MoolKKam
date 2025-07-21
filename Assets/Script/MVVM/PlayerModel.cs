using TreeEditor;
using UnityEngine;

public class PlayerModel
{
    public string Name { get; set; }
    public float Health { get; set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public GameObject resourceObject;
    public bool isAttacking { get; set; }
    public bool IsGathering { get; private set; }
    public ItemData CurrentItem { get; private set; }

    public void StartAttack(WeaponController weapon)
    {
        weapon.EnableDamage();
    }
    public void EndAttack(WeaponController weapon)
    {
        weapon.DisableDamage();
    }
    public void StartGathering(ItemData ore, GameObject obj)
    {
        CurrentItem = ore;
        IsGathering = true;
        resourceObject = obj;
    }

    public void CompleteGathering()
    {
        IsGathering = false;
        CurrentItem = null;
        resourceObject = null;
    }
    public PlayerModel() //테스트 셋업용
    {
        Name = "Test";
        Health = 10f;
        Attack = 5f;
        Speed = 3f;
    }
}
