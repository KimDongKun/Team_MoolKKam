using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerViewModel : INotifyPropertyChanged
{
    private PlayerModel playerModel;
    public string WeaponLevel => $"LV {playerModel.weaponModel.UpgradeLevel}";
    public NPCModel npcModel => playerModel.NPCModel;
    public string HealthText => $"HP: {playerModel.Health}";
    public Color HealthColor => playerModel.Health < 30 ? Color.red : Color.green;
    private float rollTimer = 0f;




    public ItemData AddItemData => playerModel.CurrentItem;
    public List<InventoryData> itemList => playerModel.GetItemList;
    public bool IsGathering => playerModel.IsGathering;
    public bool IsNpcMeeting => playerModel.IsNpcMeeting;
    public PlayerViewModel(PlayerModel model)
    {
        playerModel = model;
    }
    public string Name
    {
        get { return playerModel.Name; }
        set
        {
            if (playerModel.Name != value)
            {
                playerModel.Name = value;
                OnPropertyChanged("Name");
            }
        }
    }

    public float Health
    {
        get { return playerModel.Health; }
        set
        {
            if (playerModel.Health != value)
            {
                playerModel.Health = value;
                OnPropertyChanged("Health");
            }
        }
    }
    public void ReceiveDamage(float damage)
    {
        // 모델에서 체력 감소 처리
        playerModel.Health -= damage;

        // UI 갱신 등 추가 처리
        Debug.Log($"플레이어 피격! 남은 체력: {playerModel.Health}");

        if (playerModel.Health <= 0)
        {
           Debug.Log("플레이어 사망!");
            // 플레이어 사망 처리 로직 추가 (예: 게임 오버 화면 표시 등)
        }
    }
    public void AddItem(ItemData data, int Quantity = 0)
    {
        Debug.Log("획득한 아이템 갯수 : " + Quantity);
        var item = itemList.FirstOrDefault(i => i.itemData.ItemName == data.ItemName);
        var resultQuantiy = Quantity != 0 ? Quantity : 0;
        if (item != null)
        {
            Debug.Log(data.ItemName + " : " + Quantity);
            item.Quantity += resultQuantiy;
            //item.Quantity += data.MaxStack;//MaxStack대신 랜덤 갯수값 (1~3)사이의 값으로 수정.
        }
        else
        {
            itemList.Add(new InventoryData(data, resultQuantiy));
        }
        InventoryUpdate();
    }
    public void UpgradeWeaponPlayer()
    {
        WeaponViewModel weaponViewModel = new WeaponViewModel(playerModel.weaponModel);
        weaponViewModel.UpgradeWeapon();

        InventoryUpdate();
    }
    public void InventoryUpdate() => OnPropertyChanged("InventoryUpdate");

    private Vector3 rollStartPos;
    private Vector3 rollTargetPos;
    public void RollPlayer(Animator animator , Rigidbody rb)
    {
        rollTimer = 0f;
        PlayerController playerController = animator.GetComponent<PlayerController>();
        float h = Input.GetAxisRaw("Horizontal");
        animator.SetTrigger("Roll");
        animator.SetBool("IsRolling", true);
        playerModel.IsRolling = true;
        rollTimer = 0f;
        rollStartPos = playerController.transform.position;

        // 현재 바라보는 방향 (Y축 기준)
        float yRotation = playerController.transform.rotation.eulerAngles.y;
        Vector3 direction = (yRotation == 90f) ? Vector3.right : Vector3.left;

        rollTargetPos = rollStartPos + direction * playerModel.rollDistance;
        //float rollSpeed = 13f; // 구르기 속도 
        //if (h > 0)
        //{
        //    rb.linearVelocity = (Vector3.right * rollSpeed);

        //}
        //else if(h<0)
        //{
        //    rb.linearVelocity = (Vector3.left * rollSpeed);
        //}

    }


    public void Rolling(GameObject player)
    {
        rollTimer += Time.deltaTime;
        float t = rollTimer / playerModel.rollTime;

        if (t >= 1f)
        {
            playerModel.IsRolling = false;
            player.transform.position = rollTargetPos; // 최종 위치 고정
        }
        else
        {
            float newX = Mathf.Lerp(rollStartPos.x, rollTargetPos.x, t);
            player.transform.position = new Vector3(newX, player.transform.position.y, player.transform.position.z);
        }
    }

    public void MovePlayer(Animator animator,GameObject player)   // 플레이어 이동함수
    {
        float h = 0f;
        h = Input.GetAxisRaw("Horizontal");
        // rotate y 설정 오른쪽 90 왼쪽 270   

        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        player.transform.position = new Vector3(player.transform.position.x + (h * playerModel.Speed * Time.deltaTime), player.transform.position.y, player.transform.position.z);
        if (h > 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 90, 0); // 오른쪽
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
            playerModel.FacingDirection = 0; // 오른쪽
            playerModel.IsPlayerMoving = true; // 플레이어가 움직이고 있음
        }
        else if (h < 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 270, 0); // 왼쪽
            player.transform.rotation = Quaternion.Euler(0, 270, 0);
            playerModel.FacingDirection = 1; // 왼쪽 
            playerModel.IsPlayerMoving = true; // 플레이어가 움직이고 있음
        }
        else
        {
                       playerModel.IsPlayerMoving = false; // 플레이어가 움직이지 않음
        }

  
    }

    public void UseSkill(Animator animator, Rigidbody rb, string type)
    {
        if(type == "ParryedAttack")
        {
            float dashForce = 45f; // 대시 힘  
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(rb.transform.forward * dashForce, ForceMode.VelocityChange);
            rb.useGravity = false; // 중력 비활성화
            playerModel.HasParried = false; // 패링 상태 해제
        MonoBehaviour mono = rb.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            mono.StartCoroutine(RestoreGravity(rb, 0.15f));
        }
        }
        if(type == "GuardAttackSkill")
        {
            animator.SetTrigger("GuardAttackSkill");
            float dashForce = 45f; // 대시 힘  
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(rb.transform.forward * dashForce, ForceMode.VelocityChange);
            rb.useGravity = false; // 중력 비활성화
            MonoBehaviour mono = rb.GetComponent<MonoBehaviour>();
            if (mono != null)
            {
                mono.StartCoroutine(RestoreGravity(rb, 0.15f));
            }
        }

    }
    private IEnumerator RestoreGravity(Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = true;
    }





    public void GuardPlayer(Animator animator, Rigidbody rb)
    {
        animator.SetBool("IsGuard", true);
        playerModel.IsGuarding = true;

    }

    public void JumpPlayer(bool space, Animator animator,Rigidbody rb)
    {
        float hight = rb.linearVelocity.y;
        animator.SetFloat("Hight", hight);
        animator.SetBool("Ground", playerModel.IsGrounded);
        if (space && playerModel.IsGrounded && !playerModel.IsAttacking && !playerModel.IsRolling && !playerModel.IsGuarding)
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * playerModel.JumpForce, ForceMode.Impulse);
            //   rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            Debug.Log($"점프키누름");
            playerModel.IsGrounded = false;

        }

        if (playerModel.IsAttacking)
        {

            Vector3 vel = rb.linearVelocity;
            vel.y = 0f; // Y 속도를 고정 (중력 무시)
          //  rb.linearVelocity = vel;

            return;
        }


        if (rb.linearVelocity.y < 0.2 && !playerModel.IsGrounded)
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (playerModel.FallMultiplier + 1f) * Time.deltaTime;
        }
        else if (rb.linearVelocity.y > 1.5 && !Input.GetKey(KeyCode.Space))
        {
            rb.linearVelocity += Vector3.up * Physics.gravity.y * (playerModel.LowJumpMultiplier - 1) * Time.deltaTime;
        }
        

    }
    public string TakeDamage(int dmg)
    {
        if (playerModel.IsGuarding)
        {
            playerModel.Animator.SetTrigger("Block");
            if (playerModel.IsParrying)
            {
                playerModel.IsParrying = false; // 반격 상태 해제
                return "parry"; // 가드 중에 적의 공격을 막으면 반격
            }
            else { 
                return "block"; // 가드 중에는 데미지를 받지 않음
            }
        }
        if (playerModel.IsRolling)
        {
            return "roll"; // 구르는 중에는 데미지를 받지 않음    
        }
        playerModel.Health -= dmg;
       // Debug.Log("데미지 받음 :" + playerModel.Health);
        OnPropertyChanged("Health");
        return "hit";
    }
    public void CompleteGathering()
    {
        Debug.Log("자원채취 완료"+ AddItemData.ItemName);
        AddItem(AddItemData);
        playerModel.resourceObject.SetActive(false);
        
        playerModel.CompleteGathering();
        OnPropertyChanged("CompleteGathering");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
   //     Debug.Log($"프로퍼티 이름 {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
