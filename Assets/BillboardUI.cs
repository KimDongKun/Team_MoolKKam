using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Quaternion initialRotation;
    public float fixedZ = 90f; // ���ϴ� Z ȸ�� ��
    void Awake()
    {
        Vector3 euler = transform.rotation.eulerAngles;
        euler.y = fixedZ; // Z�� ���ϴ� ������
        initialRotation = Quaternion.Euler(euler);
    }

    void LateUpdate()
    {
        transform.rotation = initialRotation;
    }
}
