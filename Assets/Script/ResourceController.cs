using System;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    public resourceType type = resourceType.None;

    public ItemData itemData;
    public float reTimer=3;

    public enum resourceType
    {
        None,
        Tree,
        Rock
    }
    private void OnEnable()
    {
    }
    private void OnDisable()
    {
        Invoke("ActiveObject", reTimer);
    }
    private void ActiveObject()
    {
        this.gameObject.SetActive(true);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            player.playerModel.StartGathering(itemData, this.gameObject);
            //Debug.Log("Á¢ÃËÇÑ °´Ã¼ ÀÌ¸§:" + player.playerModel.Name);
            //Debug.Log("ÀÚ¿ø È¹µæ À¯¹« bool => true : "+type.ToString());
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out player))
        {
            player.playerModel.CompleteGathering();
            Debug.Log("ÀÚ¿ø È¹µæ À¯¹« bool => false : " + type.ToString());
        }

    }
}
