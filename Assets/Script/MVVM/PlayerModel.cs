
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    public WeaponModel weaponModel;

    public string Name { get; set; }
    public float Health { get; set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public GameObject resourceObject;
    public bool isAttacking { get; set; }
    public bool IsGathering { get; private set; }
    public bool IsNpcMeeting { get; private set; }
    public NPCModel NPCModel { get; set; }
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

    public void StartNPCTrigger(NPCModel npcModel)
    {
        NPCModel = npcModel;
        npcModel.PlayerModel = this;
        NPCModel.isTrigger = true;
        IsNpcMeeting = true;
    }
    public void CompleteNPCTrigger()
    {
        NPCModel = null;
        IsNpcMeeting = false;
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
        weaponModel = new WeaponModel();
        Name = name;
        Health = 10f;
        Attack = 5f;
        Speed = 3f;
    }
}
