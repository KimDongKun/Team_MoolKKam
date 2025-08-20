using UnityEngine;

public class TentacleSmash : BossPattern, IBossPattern
{
    public float Excute(Transform player)
    {
        this.patternObject.SetActive(true);
        return this.duration;
    }

    public void ReadyPattern()
    {
        throw new System.NotImplementedException();
    }
}
