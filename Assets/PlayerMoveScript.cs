using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMoveScript : MonoBehaviour
{
    public GameObject player;
    

    public float h;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        player.transform.position = new Vector3(player.transform.position.x + (h * 5f * Time.deltaTime), player.transform.position.y,player.transform.position.z);  
    }
}
