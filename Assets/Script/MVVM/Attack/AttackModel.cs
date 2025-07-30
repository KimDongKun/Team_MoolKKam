using System.Collections.Generic;
using UnityEngine;

public class AttackModel
{
    public string Name;
    public int Damage;
    public float Range;
    public AttackType Type; // Basic, Skill, Parry, Counter
    public float Cooldown;
    public string AnimationTrigger;
    

}
public class AttackModelList
{
}


public enum AttackType
{   
    Basic,
    Skill,
    Parry,
    Counter
}