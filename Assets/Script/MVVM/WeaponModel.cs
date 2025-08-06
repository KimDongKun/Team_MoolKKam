using UnityEngine;

public class WeaponModel
{
    public string Name { get; set; }
    public float Damage {  get; set; }
    public int UpgradeLevel { get; set; }
    
    public WeaponModel()
    {
        Name = "Sword";
        Damage = 10 * UpgradeLevel;
        UpgradeLevel = 1;
    }
}
