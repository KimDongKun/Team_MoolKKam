using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class NPCViewModel
{
    private NPCModel npcModel;
    public TradeModel tradeModel => npcModel.tradeModel;

    public GameObject tradeUI => npcModel.TradeUI;
    public bool isTrigger => npcModel.isTrigger;
    public bool isNPCTrader => npcModel.isNPCTrader;

    public event Action<string> OnDialogueUpdate;
    public event Action OnUpgradeAction;

    public int dialogueIndex = 0;

    public int costGold;
    public int costFirst;
    public int costSecond;
    public int costThird;
    public int costFourth;

    public string Name
    {
        get { return npcModel.Name; }
        set
        {
            if (npcModel.Name != value)
            {
                npcModel.Name = value;
            }
        }
    }
    public Texture2D Image
    {
        get { return npcModel.NPCImage; }
        set
        {
            if (npcModel.NPCImage)
            {
                npcModel.NPCImage = value;
                Debug.Log("NPC :" + npcModel.Name);
            }
        }
    }

    public List<string> Talks => npcModel.NPCTalk;

    
    public void ShowNextDialogue()
    {
        if (dialogueIndex < Talks.Count && npcModel.isTrigger)
        {
            OnDialogueUpdate?.Invoke(Talks[dialogueIndex++]);
        }
        else
        {
            ResetDialoque();
        }
    }
    public void ShowTradeUI()
    {
        TradeViewModel tradeViewModel = new TradeViewModel(tradeModel);
        var setCostDate = tradeViewModel.SetUpgradeCostValue(npcModel.PlayerModel.weaponModel);
        costGold = setCostDate[0].Quantity;
        costFirst = setCostDate[1].Quantity;
        costSecond = setCostDate[2].Quantity;
        costThird = setCostDate[3].Quantity;
        costFourth = setCostDate[4].Quantity;

        
        Debug.Log("Check");
    }

    public void ResetDialoque()
    {
        npcModel.isTrigger = false;
        dialogueIndex = 0;
    }

    public NPCViewModel(NPCModel model)
    {
        npcModel = model;
    }

}
