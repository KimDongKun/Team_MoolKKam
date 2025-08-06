using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public InventoryData ItemData;
    private PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            //Debug.Log("NPC�÷��̾� �浹" + npcModel.Name);
            PlayerViewModel playerViewModel = new PlayerViewModel(player.playerModel);
            player.playerViewModel.AddItem(ItemData.itemData,ItemData.Quantity);
            this.gameObject.SetActive(false);
        }
    }

}
