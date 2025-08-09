using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Quaternion initialRotation;
    public float fixedZ = 90f; // 원하는 Z 회전 값
    void Awake()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = fixedZ; // Z축 원하는 값으로
        initialRotation = Quaternion.Euler(euler);
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
