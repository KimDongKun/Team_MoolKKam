using UnityEngine;

namespace justA.PixelFilter.Example
{
    /// <summary>
    /// This script moves the GameObject up and down in a sinusoidal pattern.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class ExampleMovingScript : MonoBehaviour
    {
        void Update()
        {
            transform.position += new Vector3(0, Mathf.Sin(Time.time) * Time.deltaTime, 0);
        }
    }
}
