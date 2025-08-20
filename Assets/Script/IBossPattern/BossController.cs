
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public PlayerController player;
    public List<GameObject> gameObjects;
    public List<IBossPattern> patternList;

    public GameObject BossUI;

    private float nextAttackTime = 0;
    private GameObject selectPattern;

    public bool isAlive = true;

    private void Start()
    {
        patternList = new List<IBossPattern>();
        for(int i = 0; i<gameObjects.Count; i++)
        {
            patternList.Add(gameObjects[i].GetComponent<IBossPattern>());
        }
    }
    private void Update()
    {
       

        if (Time.time >= nextAttackTime && isAlive)
        {
            Debug.Log("공격패턴 타이머 오버");
            RandomPattern();
        }
        else if (!isAlive)
        {
            var dis = Vector3.Distance(player.transform.position, this.transform.position);
            if (dis<=30)
            {
                isAlive = true;
                BossUI?.SetActive(true);
            }
        }
    }
    public void RandomPattern()
    {
        selectPattern?.SetActive(false);

        int index = Random.Range(0, patternList.Count);
        IBossPattern pattern = patternList[index];
        selectPattern = gameObjects[index];
        selectPattern.SetActive(true);
        // 공격 실행 + 해당 공격의 소요시간 반환

        float duration = pattern.Excute(player.transform);

        // 다음 공격은 현재 시간 + 소요시간
        nextAttackTime = Time.time + duration;
    }
}
