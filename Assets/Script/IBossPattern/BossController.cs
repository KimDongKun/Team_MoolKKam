
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossController : MonoBehaviour
{
    public PlayerController player;
    public AudioController audioController;
    public List<GameObject> gameObjects;
    public List<IBossPattern> patternList;

    public GameObject BossUI;
    public GameObject EndingUI;

    public Slider healthUi;
    private float nextAttackTime = 0;
    private GameObject selectPattern;
    private float MaxHp = 500f;
    private float health = 500f;
    public bool isAlive = true;
    public Material bossMat;

    public AudioSource bossSound;
    public AudioClip[] bossClips;
    private void Start()
    {
        patternList = new List<IBossPattern>();
        for(int i = 0; i<gameObjects.Count; i++)
        {
            patternList.Add(gameObjects[i].GetComponent<IBossPattern>());
        }
        healthUi.maxValue = MaxHp;
        healthUi.value = MaxHp; // Assuming max health is 500
        bossMat.SetColor("_BaseColor", Color.white);
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        health -= damage;
        healthUi.value = health; // Assuming max health is 500   
        
        StartCoroutine(GetDamageColor());
        Debug.Log($"���� ü��: {health}");
        if (health <= 0)
        {
            isAlive = false;
            BossUI?.SetActive(false);
            EndingUI?.SetActive(true);
            this.gameObject.SetActive(false);
            Debug.Log("������ �׾����ϴ�.");
            BossSoundPlay(bossClips[1]);
        }
        else
        {
            BossSoundPlay(bossClips[0]);
        }
    }   
    IEnumerator GetDamageColor()
    {
        bossMat.SetColor("_BaseColor", Color.red);
        Debug.Log("���� ���ݹ޾� �ǰݻ���");
        yield return new WaitForSeconds(0.25f);
        bossMat.SetColor("_BaseColor", Color.white);

    }
    private void Update()
    {
       

        if (Time.time >= nextAttackTime && isAlive)
        {
            Debug.Log("�������� Ÿ�̸� ����");
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
    public void BossSoundPlay(AudioClip clip)
    {
        bossSound.resource = clip;
        bossSound.Play();
    }
    public void RandomPattern()
    {
        selectPattern?.SetActive(false);

        int index = UnityEngine.Random.Range(0, patternList.Count);
        IBossPattern pattern = patternList[index];
        selectPattern = gameObjects[index];
        selectPattern.SetActive(true);
        // ���� ���� + �ش� ������ �ҿ�ð� ��ȯ

        float duration = pattern.Excute(player.transform);

        // ���� ������ ���� �ð� + �ҿ�ð�
        nextAttackTime = Time.time + duration;
    }
}
