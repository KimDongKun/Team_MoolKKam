using System.Collections.Generic;
using System;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public int damage = 10;
    public string targetTag = "enemy";
    private bool canDamage = false;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    public AttackModel attackModel;
    public void EnableDamage(AttackModel attack)
    {
        attackModel = attack;
        canDamage = true;
        damagedEnemies.Clear(); // 🧹 공격 시작할 때 맞은 적 목록 초기화
    }

    public void DisableDamage()
    {
        canDamage = false;
    }

    private void OnTriggerStay(Collider other)
    {
        string objectTeg = other.tag;
        if (!canDamage) return;
       // Debug.Log($"WeaponController OnTriggerStay: {other.gameObject.name} with tag {other.tag}");
        if (!objectTeg.Equals(targetTag)) return;
        if (damagedEnemies.Contains(other.gameObject)) return; // 이미 맞은 적은 무시
        // 데미지 처리
        var health = other.GetComponent<EnemyController>();
        if (health != null)
        {
            health.TakeDamage(damage,attackModel);
            damagedEnemies.Add(other.gameObject); // 맞은 적 등록
        }
    }
}
