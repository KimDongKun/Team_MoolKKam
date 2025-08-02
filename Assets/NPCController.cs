using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public NPCModel npcModel;
    public InventoryData[] itemData;


    public Texture2D npcImage;
    public string npcName;
    public bool isNpcTrader = false;
    public List<string> npcTalks = new List<string>();

    private void Start()
    {
        Debug.Log(npcName + npcModel.isNPCTrader);
        var itemDataList = itemData.ToList();
        npcModel = new NPCModel(npcName, npcImage, npcTalks, isNpcTrader, itemDataList);
        npcModel.isNPCTrader = isNpcTrader;
        Debug.Log(npcName + npcModel.isNPCTrader);
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            //Debug.Log("NPC플레이어 충돌" + npcModel.Name);
            player.playerModel.StartNPCTrigger(npcModel);
        }
    }


    private void OnTriggerExit(Collider other) 
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            player.playerModel.CompleteNPCTrigger();
        }

    }
}
