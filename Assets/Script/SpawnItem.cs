using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public InventoryData ItemData;
    private PlayerController player;

    public Transform playerPos;
    private void Start()
    {
        playerPos = GameObject.Find("Player").transform;

    }
    private void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, playerPos.position+new Vector3(0,2,0), 5*Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            //Debug.Log("NPC플레이어 충돌" + npcModel.Name);
            UISoundController.Instance.PlayUISound("GetItem");

            PlayerViewModel playerViewModel = new PlayerViewModel(player.playerModel);
            player.playerViewModel.AddItem(ItemData.itemData,ItemData.Quantity);
            this.gameObject.SetActive(false);
        }
    }

}
