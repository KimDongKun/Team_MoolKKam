using System.Collections.Generic;
using UnityEngine;

public class TowerController : BuildObject
{
    public int Damage;

    public float detectionRange = 10f; // ���� ����
    public Transform firePoint;        // �߻� ��ġ
    public GameObject projectilePrefab; // �߻�ü ������
    public float fireRate = 1f;        // ���� �ֱ�(�ʴ� �߻�)
    public float projectileSpeed = 15f; // �߻� �ʱ� �ӵ�

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
            // ���� ����� �� ã��
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
