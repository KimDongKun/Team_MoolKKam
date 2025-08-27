using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private AttackModel attackModel;

    public bool isBoss= false;
    public float gravity = -9.81f;
    
    private Transform target;
    private Vector3 velocity;
    private bool launched = false;

    Vector3 setTarget;
    float setSpeed;

    public void SetAttackModel(AttackModel attackModel)
    {
        this.attackModel = attackModel;
    }
    

    public void Launch(Transform target, float speed)
    {
        setTarget = target.transform.position;
        setSpeed = speed;
        this.target = target;
        launched = true;
    }

    public void Launch_Arrow(Transform target, float speed)
    {
        this.target = target;
        Vector3 toTarget = this.target.position - transform.position;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float distanceXZ = toTargetXZ.magnitude;
        float heightDiff = toTarget.y - transform.position.y;

        // 포물선 공식에 따라 발사 각도 계산
        float vSquared = speed * speed;
        float g = -gravity;

        float underRoot = vSquared * vSquared - g * (g * distanceXZ * distanceXZ + 2 * heightDiff * vSquared);
        if (underRoot <= 0f)
        {
            Debug.LogWarning("목표에 도달 불가 - 속도 부족");
            Destroy(gameObject);
            return;
        }

        float root = Mathf.Sqrt(underRoot);
        float angle = Mathf.Atan((vSquared - root) / (g * distanceXZ)); // 낮은 각도 버전

        // 초기 속도 벡터 계산
        Vector3 dirXZ = toTargetXZ.normalized;
        float vy = speed * Mathf.Sin(angle);
        float vxz = speed * Mathf.Cos(angle);

        velocity = dirXZ * vxz + Vector3.up * vy;
        launched = true;
    }

    void Update()
    {
        if (!launched || target == null) return;
        this.transform.position = Vector3.MoveTowards(this.transform.position, setTarget + (Vector3.up*2), Time.deltaTime* setSpeed);

        float dis = Vector3.Distance(transform.position, target.position);

        if (dis <= 0f)
        {
            Destroy(this.gameObject);
        }

        // 적에 도달 시
        if (dis <= 2.4f)
        {
            Debug.Log("충돌"+ target.name);


            if (isBoss) target.GetComponent<PlayerController>()?.TakeDamage(10);
            else
            {
                target.GetComponent<Enemy>()?.TakeDamage(attackModel.Damage, attackModel);
            }
            Destroy(this.gameObject);
        }
        
    }
}
