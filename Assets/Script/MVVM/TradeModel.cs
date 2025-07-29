using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TradeModel
{
    public PlayerModel playerModel { get; set; }
    public List<InventoryData> costitemList = new List<InventoryData>();

    public TradeModel(PlayerModel model, List<InventoryData> costItems) 
    {
        playerModel = model;
        costitemList = costItems;
    }
}
