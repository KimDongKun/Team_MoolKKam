using UnityEngine;
using System.Collections;

public class CameraShake2D : MonoBehaviour
{
    public static CameraShake2D Instance;
    private void Awake()
    {
        Instance = this;
    }

    private Coroutine shakeCoroutine;

    /// <summary>
    /// ��鸲 ����
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;
        Quaternion originalRot = Quaternion.identity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // -1 ~ 1 ���� ������
            float z = Random.Range(-1f, 1f) * magnitude;

            // z�� ȸ�� ����
            transform.rotation = Quaternion.Euler(0, 0, z);

            yield return null;
        }

        // ���� ȸ������ ����
        transform.rotation = originalRot;
        shakeCoroutine = null;
    }
}