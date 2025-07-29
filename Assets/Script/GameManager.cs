using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public PlayerUIView playerUIView;
    public PlayerController player2;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //player.playerModel = new PlayerModel("플레이어A");
        //player.playerModel.Animator = player.GetComponent<Animator>();
        //player.playerViewModel = new PlayerViewModel(player.playerModel);
        //playerUIView.Init(player.playerViewModel);

        player2.playerModel = new PlayerModel("플레이어B");
        player2.playerModel.Animator = player2.GetComponent<Animator>();
        player2.playerViewModel = new PlayerViewModel(player2.playerModel);
        playerUIView.Init(player2.playerViewModel);
    }

}
