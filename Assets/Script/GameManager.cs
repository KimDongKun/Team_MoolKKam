using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public PlayerUIView playerUIView;
    public List<InventoryData> playerInventory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.playerModel = new PlayerModel("플레이어A",player.playerMaterial);
        player.playerViewModel = new PlayerViewModel(player.playerModel);
        player.playerModel.Animator = player.GetComponent<Animator>();
        player.weaponController.weaponModel = player.playerModel.weaponModel;
        playerUIView.Init(player.playerViewModel);
        for (int i = 0; i < playerInventory.Count; i++)
            player.playerViewModel.AddItem(playerInventory[i].itemData);

    }

}
