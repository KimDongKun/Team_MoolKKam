using UnityEngine;

public class PlayerModel
{
    public string Name { get; set; }
    public float Health { get; set; }
    public float Attack { get; set; }
    public float Speed { get; set; }

    public PlayerModel() //테스트 셋업용
    {
        Name = "Test";
        Health = 10f;
        Attack = 5f;
        Speed = 3f;
    }
}
