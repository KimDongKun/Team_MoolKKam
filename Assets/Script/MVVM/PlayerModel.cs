using NUnit.Framework;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerModel
{
    
    public string Name { get; set; }
    public float Health { get; set; }
    public float Attack { get; set; }
    public float Speed { get; set; }
    
    public float JumpForce { get; set; }  // ���� �� 13 
    public float FallMultiplier { get; set; } //= 2.5f; // ���� �� �߷� ����ġ 3.5
    public float LowJumpMultiplier { get; set; }// = 2.5f; // �ּ� ���� ���� 2
    
    public int FacingDirection { get; set; } // �÷��̾ �ٶ󺸴� ���� 0: ������, 1: ����

    public float RotationSpeed { get; set; } //= 10f; // ȸ�� �ӵ� 10

    public bool IsPlayerMoving { get; set; } // �÷��̾ �����̰� �ִ��� ����

    public bool IsParrying { get; set; } // �÷��̾ ��� ������ ����

    public bool IsGrounded { get; set; } // = true; // �÷��̾ ���� �ִ��� ����
    public bool IsRolling { get; set; } // �÷��̾ ������ �ִ��� ����

    public bool HasParried { get; set; } // �÷��̾ �� �ߴ��� ����    

    public bool HasJumpAttacked { get; set; } // �÷��̾ ���� ������ �ߴ��� ����
    public bool IsGuarding { get; set; } // �÷��̾ ��� ������ ���� 

    public float rollDistance = 13f; // �÷��̾� ������ �Ÿ�  

    public float rollSpeed = 8f; // �÷��̾� ������ �ӵ�

    public float rollTime = 1.2f; // �÷��̾� ������ �ð�

    public bool IsChaged { get; set; } // �÷��̾ ���� ������ ����   
    public Animator Animator { get; set; } // �÷��̾� �ִϸ�����
    public Quaternion TargetRotation { get; set; } // �÷��̾� ȸ���� 10

    [SerializeField] public float maxChargeTime = 3f; // �� ���� �ð�
    public float chargeTime = 0f;
    public int currentLevel = 0;

    public GameObject resourceObject;
    public bool IsAttacking { get; set; }
    public bool IsGathering { get; private set; }
    public ItemData CurrentItem { get; private set; }
    public List<InventoryData> GetItemList = new List<InventoryData>();

    public void StartAttack(WeaponController weapon, AttackModel attackModel)
    {
        weapon.EnableDamage(attackModel);


    }
    public void EndAttack(WeaponController weapon)
    {
        weapon.DisableDamage();
    }
    public void StartGathering(ItemData ore, GameObject obj)
    {
        CurrentItem = ore;
        IsGathering = true;
        resourceObject = obj;
    }

    public void CompleteGathering()
    {
        IsGathering = false;
        CurrentItem = null;
        resourceObject = null;
    }
    public PlayerModel(string name) //�׽�Ʈ �¾���
    {
        Name = name;
        Health = 100f;
        Attack = 5f;
        Speed = 9f;
        JumpForce = 15f; // ���� ��
        FallMultiplier = 3.5f; // ���� �� �߷� ����ġ
        LowJumpMultiplier = 2f; // �ּ� ���� ����
        RotationSpeed = 17f; // ȸ�� �ӵ�
        IsGrounded = true; // �÷��̾ ���� �ִ��� ����
        TargetRotation = Quaternion.identity; // �ʱ� ȸ����
        IsAttacking = false;
        IsRolling = false; // �÷��̾ ������ �ִ��� ����

    }
}
