using System.Collections.Generic;
using UnityEngine;

public class damege_triger_script : MonoBehaviour
{
    public int damage = 10;
    public string targetTag = "Enemy";

    private bool canDamage = false;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

    public void EnableDamage()
    {
        canDamage = true;
        damagedEnemies.Clear(); // 🧹 공격 시작할 때 맞은 적 목록 초기화
    }

    public void DisableDamage()
    {
        canDamage = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!canDamage) return;
        if (!other.CompareTag(targetTag)) return;
        if (damagedEnemies.Contains(other.gameObject)) return; // ✅ 이미 맞은 적은 무시

        // 데미지 처리
        var health = other.GetComponent<helth_script>();
        if (health != null)
        {
            health.TakeDamage(damage);
            damagedEnemies.Add(other.gameObject); // ✅ 맞은 적 등록
        }
    }
}
