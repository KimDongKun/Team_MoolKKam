using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    public NPCModel npcModel;

    public Texture2D npcImage;
    public string npcName;
    public List<string> npcTalks = new List<string>();

    private void Start()
    {
        npcModel = new NPCModel(npcName, npcImage, npcTalks);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
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
