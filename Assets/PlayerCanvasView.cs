using UnityEngine;

public class PlayerCanvasView : MonoBehaviour
{
    public Transform playerTr;
    
    void Update()
    {
        Vector3 canvasPos = new Vector3(playerTr.position.x, transform.position.y, transform.position.z);
        this.transform.position = canvasPos;
    }
}
