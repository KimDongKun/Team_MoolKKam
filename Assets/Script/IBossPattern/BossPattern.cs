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
        alreadyHit.Clear(); // 새로운 공격 시작할 때 초기화
        canDamage = true;
        StartCoroutine(DamageCool(0.3f)); // 공격 가능 시간 설정
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

        if (alreadyHit.Contains(other.gameObject)) return; // 이미 맞았으면 무시
        Debug.Log($"BossPattern OnTriggerStay: Hit Player {other.gameObject.name}");
        player.TakeDamage(damage);
        alreadyHit.Add(other.gameObject);
    }

}

