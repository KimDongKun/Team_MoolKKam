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
    
    public float JumpForce { get; set; }  // 점프 힘 13 
    public float FallMultiplier { get; set; } //= 2.5f; // 낙하 시 중력 가중치 3.5
    public float LowJumpMultiplier { get; set; }// = 2.5f; // 최소 점프 높이 2
    
    public int FacingDirection { get; set; } // 플레이어가 바라보는 방향 0: 오른쪽, 1: 왼쪽

    public float RotationSpeed { get; set; } //= 10f; // 회전 속도 10

    public bool IsPlayerMoving { get; set; } // 플레이어가 움직이고 있는지 여부

    public bool IsGrounded { get; set; } // = true; // 플레이어가 땅에 있는지 여부
    public bool IsRolling { get; set; } // 플레이어가 구르고 있는지 여부
    
    
    public Quaternion TargetRotation { get; set; } // 플레이어 회전값 10

    public GameObject resourceObject;
    public bool IsAttacking { get; set; }
    public bool IsGathering { get; private set; }
    public ItemData CurrentItem { get; private set; }
    public List<InventoryData> GetItemList = new List<InventoryData>();

    public void StartAttack(WeaponController weapon)
    {
        weapon.EnableDamage();
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
    public PlayerModel(string name) //테스트 셋업용
    {
        Name = name;
        Health = 10f;
        Attack = 5f;
        Speed = 9f;
        JumpForce = 15f; // 점프 힘
        FallMultiplier = 3.5f; // 낙하 시 중력 가중치
        LowJumpMultiplier = 2f; // 최소 점프 높이
        RotationSpeed = 17f; // 회전 속도
        IsGrounded = true; // 플레이어가 땅에 있는지 여부
        TargetRotation = Quaternion.identity; // 초기 회전값
        IsAttacking = false;
        IsRolling = false; // 플레이어가 구르고 있는지 여부

    }
}
