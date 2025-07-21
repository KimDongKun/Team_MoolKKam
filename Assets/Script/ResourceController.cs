using System;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    public resourceType type = resourceType.None;

    public ItemData itemData;

    public enum resourceType
    {
        None,
        Tree,
        Rock
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            player.playerModel.StartGathering(itemData, this.gameObject);
            Debug.Log("�ڿ� ȹ�� ���� bool => true : "+type.ToString());
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            Debug.Log("�ڿ� ȹ�� ���� bool => false : " + type.ToString());
        }

    }
}
