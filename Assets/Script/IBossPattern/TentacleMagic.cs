
using System.Collections;
using System.Threading;
using System.Linq;
using UnityEngine;

public class TentacleMagic : BossPattern, IBossPattern
{
    public GameObject[] tentacles;
    public GameObject magicPrefab;
    public float Excute(Transform player)
    {
        StartCoroutine(Attack(player));

        return duration;
    }

    public IEnumerator Attack(Transform player)
    {
        float timer = 0f;

        // 1) 플레이어 조준
        while (timer < duration-1f)
        {
            timer += Time.deltaTime;

            this.patternObject.SetActive(true);

            for(int i = 0; i<tentacles.Length; i++)
            {
                Vector3 dir = player.position - tentacles[i].transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                tentacles[i].transform.rotation = Quaternion.Euler(0, 0, angle);
            }

            yield return null;
        }
        anim.SetBool("isAttack", true);

        for (int i = 0; i < tentacles.Length; i++)
        {
            GameObject magic = Instantiate(magicPrefab, tentacles[i].transform.position, Quaternion.identity);
            magic.GetComponent<ProjectileController>().isBoss = true;
            magic.GetComponent<ProjectileController>().Launch(player,40);
        }
        
        
    }
    public void ReadyPattern()
    {
        anim.SetBool("isAttack", false);
    }

}

