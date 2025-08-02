using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class TradeViewModel : INotifyPropertyChanged
{
    private TradeModel tradeModel;

    public TradeViewModel(TradeModel tradeModel)
    {
        this.tradeModel = tradeModel;
    }

    public bool TryTrade(List<InventoryData> playerItemList, List<InventoryData> costItemList)
    {
        ///
        /// playerItemList에는 골드(재화)를 포함한 자원 전부 포함하므로 해당 스크립트로 재화로만 진행되는 거래도 가능
        ///
        for (int i = 0; i < costItemList.Count; i++)
        {
            var item = playerItemList.FirstOrDefault(c => c.itemData.ItemName == costItemList[i].itemData.ItemName);
            if(item == null) return false;

            bool isCost = (item.Quantity - costItemList[i].Quantity) >= 0 ? true : false;
            if (!isCost)
            {
                Debug.Log($"자원 부족으로 인한) 거래불가");
                return false;
            }
        }

        return true;
    }
    public void Trade(List<InventoryData> playerItemList, List<InventoryData> costItemList)
    {
        if (TryTrade(playerItemList, costItemList))
        {
            for (int i = 0; i < costItemList.Count; i++)
            {
                var item = playerItemList.FirstOrDefault(c => c.itemData.ItemName == costItemList[i].itemData.ItemName);
                item.Quantity -= costItemList[i].Quantity;
            }
        }
        else
        {
            Debug.Log("강화재료 및 재화 부족.");
        }
    }
    public void WeaponUpgrade(PlayerModel playerModel, List<InventoryData> costItemList)
    {
        if (TryTrade(playerModel.GetItemList, costItemList))
        {
            playerModel.weaponModel.UpgradeLevel++;
        }
    }
    public List<InventoryData> SetUpgradeCostValue(WeaponModel weaponModel)
    {
        ///
        /// Weapon LV관련하여 case문으로 대체.
        /// 로직 구현시 각각 필요한 갯수에 로직을 할당시켜 규칙적인 배열을 구현예정
        /// 추후 따로 로직구현 필요.(중요도 : 하)
        ///

        var costItemList = tradeModel.costitemList;
        
        switch (weaponModel.UpgradeLevel)
        {
            case 1:
                costItemList[0].Quantity = 100;
                costItemList[1].Quantity = 20;
                costItemList[2].Quantity = 10;
                costItemList[3].Quantity = 5;
                costItemList[4].Quantity = 1;
                return costItemList;
            case 2:
                costItemList[0].Quantity = 300;
                costItemList[1].Quantity = 40;
                costItemList[2].Quantity = 20;
                costItemList[3].Quantity = 10;
                costItemList[4].Quantity = 3;
                return costItemList;
            case 3:
                costItemList[0].Quantity = 500;
                costItemList[1].Quantity = 80;
                costItemList[2].Quantity = 40;
                costItemList[3].Quantity = 20;
                costItemList[4].Quantity = 5;
                return costItemList;
            case 4:
                costItemList[0].Quantity = 1000;
                costItemList[1].Quantity = 150;
                costItemList[2].Quantity = 60;
                costItemList[3].Quantity = 30;
                costItemList[4].Quantity = 10;
                return costItemList;
            case 5:
                costItemList[0].Quantity = 3000;
                costItemList[1].Quantity = 200;
                costItemList[2].Quantity = 150;
                costItemList[3].Quantity = 50;
                costItemList[4].Quantity = 20;
                return costItemList;
            default:
                costItemList[0].Quantity = 10000;
                costItemList[1].Quantity = 300;
                costItemList[2].Quantity = 200;
                costItemList[3].Quantity = 100;
                costItemList[4].Quantity = 30;
                return costItemList;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"프로퍼티 이름 {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
