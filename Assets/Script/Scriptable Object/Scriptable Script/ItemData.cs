using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    public string ItemID;
    public string ItemName;
    public Sprite Icon;
    public int MaxStack = 1;
}
[System.Serializable]
public class InventoryData
{
    public ItemData itemData;
    public int Quantity = 1;

    public InventoryData(ItemData item, int qt)
    {
        itemData = item;
        Quantity = qt;
    }
}
