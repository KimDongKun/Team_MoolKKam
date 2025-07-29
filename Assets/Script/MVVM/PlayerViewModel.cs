using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class PlayerViewModel : INotifyPropertyChanged
{
    private PlayerModel playerModel;
    public string HealthText => $"HP: {playerModel.Health}";
    public Color HealthColor => playerModel.Health < 30 ? Color.red : Color.green;

    public NPCModel npcModel=>playerModel.NPCModel;
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
    public void AddItem(ItemData data)
    {
        var item = itemList.FirstOrDefault(i => i.itemData.ItemName == data.ItemName);
        if (item != null)
        {
            item.Quantity += data.MaxStack;//MaxStack��� ���� ������ (1~3)������ ������ ����.
        }
        else
        {
            
            itemList.Add(new InventoryData(data, data.MaxStack));
        }
    }
   
    public void TakeDamage(int dmg)
    {
        playerModel.Health -= dmg;
        Debug.Log("������ ���� :" + playerModel.Health);
        OnPropertyChanged("Health");
    }
    public void CompleteGathering()
    {
        Debug.Log("�ڿ�ä�� �Ϸ�"+ AddItemData.ItemName);
        AddItem(AddItemData);
        playerModel.resourceObject.SetActive(false);
        
        playerModel.CompleteGathering();
        OnPropertyChanged("CompleteGathering");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
