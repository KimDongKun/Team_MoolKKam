using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BossPattern : MonoBehaviour
{
    public GameObject patternObject;
    public float duration;
    public Animator anim;
    public int damage = 10;
    private HashSet<GameObject> alreadyHit = new();
    public bool canDamage = false;

    public void EnableDamage()
    {
        alreadyHit.Clear(); // ���ο� ���� ������ �� �ʱ�ȭ
        canDamage = true;
        StartCoroutine(DamageCool(0.3f)); // ���� ���� �ð� ����
    }

    private IEnumerator DamageCool(float time)
    {
        yield return new WaitForSeconds(time);
        canDamage = false;
    }



    public void OnTriggerStay(Collider other)
    {
        if (!canDamage) return;
        if (!other.CompareTag("Player")) return;

        var player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (alreadyHit.Contains(other.gameObject)) return; // �̹� �¾����� ����
        Debug.Log($"BossPattern OnTriggerStay: Hit Player {other.gameObject.name}");
        player.TakeDamage(damage);
        alreadyHit.Add(other.gameObject);
    }

}

