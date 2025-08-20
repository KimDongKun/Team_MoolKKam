
using System.Collections;
using System.Threading;
using UnityEngine;

public class TentacleWhip : BossPattern, IBossPattern
{
    public float Excute(Transform player)
    {
        StartCoroutine(Attack(player));

        return duration;
    }

    public IEnumerator Attack(Transform player)
    {
        float timer = 0f;

        // 1) 플레이어 따라다니기
        while (timer < duration-1f)
        {
            timer += Time.deltaTime;

            this.patternObject.SetActive(true);
            var obj = this.patternObject;
            Vector3 resultPos = new Vector3(player.transform.position.x, obj.transform.position.y, obj.transform.position.z);
            obj.transform.position = Vector3.Lerp(obj.transform.position, resultPos, Time.deltaTime * 3f);

            yield return null;
        }
            anim.SetBool("isAttack", true);
        
    }
    public void ReadyPattern()
    {
        anim.SetBool("isAttack", false);
    }

}

