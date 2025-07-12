using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TilingTest : MonoBehaviour
{
    public MeshRenderer background;
    public MeshRenderer ground;

    public float h;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        background.material.mainTextureOffset = new Vector2(background.material.mainTextureOffset.x + (h*0.1f * Time.deltaTime), 0);
        ground.material.mainTextureOffset = new Vector2(ground.material.mainTextureOffset.x + (h * 1.5f * Time.deltaTime), 0);
    }
    
}
