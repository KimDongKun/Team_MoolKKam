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

    public void Trade(PlayerModel playerModel, List<InventoryData> costItemList)
    {
        ///
        /// playerItemList에는 골드(재화)를 포함한 자원 전부 포함하므로 해당 스크립트로 재화로만 진행되는 거래도 가능
        ///
        var playerItemList = playerModel.GetItemList;
        var playerCostList = new List<InventoryData>();

        for (int i = 0; i < playerItemList.Count; i++)
        {
            var item = playerItemList.FirstOrDefault(c => c.itemData.ItemName == costItemList[i].itemData.ItemName);
            bool isCost = (item.Quantity - costItemList[i].Quantity) >= 0 ? true : false;
            if (isCost)
            {
                playerCostList.Add(costItemList[i]);
            }
            else
            {
                //(자원 부족으로 인한)거래 불가 - 로직 구현 필요.
                Debug.Log($"자원 부족으로 인한) 거래불가");
                return;
            }
        }

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"프로퍼티 이름 {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
