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
        /// playerItemList���� ���(��ȭ)�� ������ �ڿ� ���� �����ϹǷ� �ش� ��ũ��Ʈ�� ��ȭ�θ� ����Ǵ� �ŷ��� ����
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
                //(�ڿ� �������� ����)�ŷ� �Ұ� - ���� ���� �ʿ�.
                Debug.Log($"�ڿ� �������� ����) �ŷ��Ұ�");
                return;
            }
        }

    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
