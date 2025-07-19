using UnityEngine;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class GameManager : MonoBehaviour
{
    public PlayerMoveScript player;
    public PlayerUIView playerUIView;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.playerModel = new PlayerModel();
        player.playerViewModel = new PlayerViewModel(player.playerModel);
        playerUIView.Init(player.playerViewModel);
    }

}
