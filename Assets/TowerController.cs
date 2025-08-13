using System.Collections.Generic;
using UnityEngine;

public class TowerController : BuildObject
{
    public int Damage;

    public float detectionRange = 10f; // 감지 범위
    public Transform firePoint;        // 발사 위치
    public GameObject projectilePrefab; // 발사체 프리팹
    public float fireRate = 1f;        // 공격 주기(초당 발사)
    public float projectileSpeed = 15f; // 발사 초기 속도

    private float fireTimer = 0f;
    
    void Start()
    {
        SphereCollider col = gameObject.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = detectionRange;
    }

    void OnTriggerStay(Collider other)
    {
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null && isInstall)
        {
            // 가장 가까운 적 찾기
            Enemy nearest = GetNearestEnemy();
            if (nearest != null)
            {
                fireTimer += Time.deltaTime;
                if (fireTimer >= 1f / fireRate)
                {
                    Fire(nearest);
                    fireTimer = 0f;
                }
            }
        }
    }

    Enemy GetNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRange);
        Enemy nearest = null;
        float nearestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Enemy e = hit.GetComponent<Enemy>();
            if (e != null)
            {
                float dist = Vector3.Distance(transform.position, e.transform.position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = e;
                }
            }
        }
        return nearest;
    }

    void Fire(Enemy target)
    {
        GameObject projObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        AttackModel attackModel = new AttackModel();
        attackModel.Name = "Tower";
        attackModel.Damage = Damage;
        attackModel.Type = AttackType.Basic;

        ProjectileController proj = projObj.GetComponent<ProjectileController>();
        proj.SetAttackModel(attackModel);
        proj.Launch(target.transform, projectileSpeed);
    }
}
