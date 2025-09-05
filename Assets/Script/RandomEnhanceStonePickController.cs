using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class RandomEnhanceStonePickController : MonoBehaviour
{
    public PickObject[] enchantStronPrefab;
    public List<InventoryData> pickItems;
    
    void Start()
    {
        enchantStronPrefab.ToList().ForEach(x => x.blind.color = Color.black);
    }
    public void RandomStonePickList(PlayerViewModel playerViewModel)
    {
        enchantStronPrefab.ToList().ForEach(x => x.uiObject.SetActive(false));
        List<InventoryData> randomPickList = new List<InventoryData>();
        for (int i = 0; i< 10; i++)
        {
            int index = Random.Range(0, 10);
            if (index > 0 && index <= 5)
            {
                enchantStronPrefab[i].item.sprite = pickItems.FirstOrDefault(item => item.itemData.ItemID == "01").itemData.Icon;
                playerViewModel.AddItem(pickItems.FirstOrDefault(item => item.itemData.ItemID == "01").itemData, 1);
                Debug.Log($"{index} : �Ϲݰ�ȭ�� ����");
            }
            else if (index > 5 && index <= 8)
            {
                enchantStronPrefab[i].item.sprite = pickItems.FirstOrDefault(item => item.itemData.ItemID == "02").itemData.Icon;
                playerViewModel.AddItem(pickItems.FirstOrDefault(item => item.itemData.ItemID == "02").itemData, 1);
                Debug.Log($"{index} : ��ް�ȭ�� ����");
            }
            else
            {
                enchantStronPrefab[i].item.sprite = pickItems.FirstOrDefault(item => item.itemData.ItemID == "03").itemData.Icon;
                playerViewModel.AddItem(pickItems.FirstOrDefault(item => item.itemData.ItemID == "03").itemData, 1);
                Debug.Log($"{index} : ��Ͱ�ȭ�� ����");
            }
        }
        enchantStronPrefab.ToList().ForEach(x => x.uiObject.SetActive(true));
    }
}
[System.Serializable]
public class PickObject
{
    public Image blind;
    public Image item;
    public GameObject uiObject;
}