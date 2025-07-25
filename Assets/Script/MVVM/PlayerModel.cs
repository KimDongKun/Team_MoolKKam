using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Windows;

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
    public List<InventoryData> GetItemList = new List<InventoryData>();

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
    public PlayerModel(string name) //테스트 셋업용
    {
        Name = name;
        Health = 10f;
        Attack = 5f;
        Speed = 3f;
    }
}
