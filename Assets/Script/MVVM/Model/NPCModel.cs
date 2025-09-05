using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class NPCModel
{
    public PlayerModel PlayerModel;
    public TradeModel tradeModel;

    public GameObject TradeUI { get; set; }
    public Texture2D NPCImage { get; set; }
    public string Name { get; set; }
    public List<string> NPCTalk { get; set; }
    public bool isTrigger { get; set; }
    public bool isNPCTrader { get; set; }
    

    public NPCModel(string name, Texture2D npcImage,List<string> talks = null, bool isTrader = false, List<InventoryData> costItems = null)
    {
        Name = name;
        NPCImage = npcImage;
        NPCTalk = talks;
        tradeModel = new TradeModel(PlayerModel, costItems);
    }

}
