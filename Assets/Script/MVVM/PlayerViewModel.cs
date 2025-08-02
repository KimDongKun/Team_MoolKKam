using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerViewModel : INotifyPropertyChanged
{
    private PlayerModel playerModel;
    public string WeaponLevel => $"LV {playerModel.weaponModel.UpgradeLevel}";
    public string HealthText => $"HP: {playerModel.Health}";
    public Color HealthColor => playerModel.Health < 30 ? Color.red : Color.green;

    public NPCModel npcModel => playerModel.NPCModel;
    public ItemData AddItemData => playerModel.CurrentItem;
    public List<InventoryData> itemList => playerModel.GetItemList;
    public bool IsGathering => playerModel.IsGathering;
    public bool IsNpcMeeting => playerModel.IsNpcMeeting;
    public PlayerViewModel(PlayerModel model)
    {
        playerModel = model;
    }
    public string Name
    {
        get { return playerModel.Name; }
        set
        {
            if (playerModel.Name != value)
            {
                playerModel.Name = value;
                OnPropertyChanged("Name");
            }
        }
    }

    public float Health
    {
        get { return playerModel.Health; }
        set
        {
            if (playerModel.Health != value)
            {
                playerModel.Health = value;
                OnPropertyChanged("Health");
            }
        }
    }
    public void AddItem(ItemData data, int Quantity = 0)
    {
        Debug.Log("획득한 아이템 갯수 : " + Quantity);
        var item = itemList.FirstOrDefault(i => i.itemData.ItemName == data.ItemName);
        var resultQuantiy = Quantity != 0 ? Quantity : 0;
        if (item != null)
        {
            Debug.Log(data.ItemName + " : " + Quantity);
            item.Quantity += resultQuantiy;
            //item.Quantity += data.MaxStack;//MaxStack대신 랜덤 갯수값 (1~3)사이의 값으로 수정.
        }
        else
        {
            itemList.Add(new InventoryData(data, resultQuantiy));
        }
        InventoryUpdate();
    }
    public void UpgradeWeaponPlayer()
    {
        WeaponViewModel weaponViewModel = new WeaponViewModel(playerModel.weaponModel);
        weaponViewModel.UpgradeWeapon();
        
        InventoryUpdate();
    }
    public void InventoryUpdate() => OnPropertyChanged("InventoryUpdate");
   
    public void TakeDamage(int dmg)
    {
        playerModel.Health -= dmg;
        Debug.Log("데미지 받음 :" + playerModel.Health);
        OnPropertyChanged("Health");
    }
    public void CompleteGathering()
    {
        Debug.Log("자원채취 완료"+ AddItemData.ItemName);
        AddItem(AddItemData);
        playerModel.resourceObject.SetActive(false);
        
        playerModel.CompleteGathering();
        OnPropertyChanged("CompleteGathering");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"프로퍼티 이름 {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
