using UnityEngine;

public class TentacleSmash : BossPattern, IBossPattern
{
    
    public float Excute(Transform player)
    {
        canDamage = false;
        this.patternObject.SetActive(true);
        return this.duration;
    }



    public void ReadyPattern()
    {
    
        throw new System.NotImplementedException();
    }
}
