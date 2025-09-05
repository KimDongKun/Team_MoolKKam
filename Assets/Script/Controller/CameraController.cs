using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTr;
    private Vector3 camPos;
    public float camSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camPos = new Vector3(playerTr.position.x, transform.position.y, transform.position.z);
        this.transform.position = camPos;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }
    void FollowPlayer()
    {
        camPos = new Vector3(playerTr.position.x, transform.position.y, transform.position.z);
        this.transform.position = Vector3.Lerp(this.transform.position, camPos, camSpeed * Time.deltaTime);
    }
}
