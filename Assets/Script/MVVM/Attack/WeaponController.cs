using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class WeaponController : MonoBehaviour
{
    public WeaponModel weaponModel;
    public string targetTag = "enemy";
    private bool canDamage = false;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    public AttackModel attackModel;
    public Vector3 DefaultRange;
    private BoxCollider col;
    public VisualEffect dot;
    public void Start()
    {
        DefaultRange = GetComponent<BoxCollider>().size; // 기본 범위 크기 저장
        col = GetComponent<BoxCollider>(); 
        Debug.Log($"WeaponController Start: DefaultRange = {DefaultRange}");
    }
    public void EnableDamage(AttackModel attack)
    {
        attackModel = attack;
        canDamage = true;
        
        UpdateHitbox(attack);
        damagedEnemies.Clear(); // 🧹 공격 시작할 때 맞은 적 목록 초기화
        StartCoroutine(DamageTime(0.1f)); // 공격 가능 시간 설정
    }
    IEnumerator DamageTime(float time)
    {
        yield return new WaitForSeconds(time);
        canDamage = false; // 일정 시간 후 공격 불가
    }

    public void DisableDamage()
    {
        canDamage = false;
    }


    public void Update()
    {
        // 공격 중일 때만 Hitbox를 업데이트
        GameObject weapone = GameObject.FindWithTag("Weapon");
        Transform weaponTransform = weapone.transform;
        Vector3 worldposition = transform.TransformPoint(weaponTransform.position);
      //   Debug.Log($"WeaponController Update: worldposition = {worldposition}");
        GameObject goal = GameObject.FindWithTag("Finish");
        dot.SetVector3("Weapon", weapone.transform.position);
        dot.SetVector3("pivot", goal.transform.position);
    }

    public void UpdateHitbox(AttackModel attack)
    {
        // 크기 설정
        col.size = attack.Range;

        // center는 Pivot(0,0,0) 기준이므로
        // 앞으로 절반, 위로 절반 이동시켜서 앞+위쪽으로 커지도록 설정
        col.center = new Vector3(0, 0, attack.Range.z / 2f);
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
            health.TakeDamage((int)weaponModel.Damage, attackModel);
            damagedEnemies.Add(other.gameObject); // 맞은 적 등록
        }
    }
}
