using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public Sprite Icon;
    public int MaxStack = 1;
}
