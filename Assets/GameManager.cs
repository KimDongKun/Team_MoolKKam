using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerMoveScript player;
    public PlayerUIView playerUIView;

    public PlayerMoveScript playerMoveScript;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.playerModel = new PlayerModel();
        player.playerViewModel = new PlayerViewModel(player.playerModel);
        playerUIView.Init(player.playerViewModel);
    }
}
