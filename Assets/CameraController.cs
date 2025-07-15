using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTr;
    private Vector3 camPos;
    public float camSpeed = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camPos = new Vector3(playerTr.position.x, transform.position.y, transform.position.z);
        this.transform.position = Vector3.Lerp(this.transform.position, camPos, camSpeed*Time.deltaTime);
    }
}
