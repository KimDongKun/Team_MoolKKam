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

    private Coroutine _parryDashCR; // PlayerViewModel �ʵ忡 ����(�ߺ� ���� ����)



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
                if(value >= playerModel.MaxHealth)
                {
                    playerModel.Health = playerModel.MaxHealth; // �ִ� ü���� �ʰ����� �ʵ��� ����
                }
                else
                {
                    playerModel.Health = value;
                }
                OnPropertyChanged("Health");
            }
        }
    }
    public float Mp
    {
        get { return playerModel.MP; }
        set
        {
                Debug.Log($"Mp : {value}");
            if (playerModel.MP != value)
            {
                if(value >= playerModel.MaxMP)
                {
                    playerModel.MP = playerModel.MaxMP; // �ִ� ������ �ʰ����� �ʵ��� ����
                }
                else
                {
                    playerModel.MP = value;
                }
                OnPropertyChanged("Mp");
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        // �𵨿��� ü�� ���� ó��
        playerModel.Health -= damage;

        // UI ���� �� �߰� ó��
        Debug.Log($"�÷��̾� �ǰ�! ���� ü��: {playerModel.Health}");

        if (playerModel.Health <= 0)
        {
           Debug.Log("�÷��̾� ���!");
            // �÷��̾� ��� ó�� ���� �߰� (��: ���� ���� ȭ�� ǥ�� ��)
        }
    }
    public void AddItem(ItemData data, int Quantity = 0)
    {
        Debug.Log("ȹ���� ������ ���� : " + Quantity);
        var item = itemList.FirstOrDefault(i => i.itemData.ItemName == data.ItemName);
        var resultQuantiy = Quantity != 0 ? Quantity : 0;
        if (item != null)
        {
            Debug.Log(data.ItemName + " : " + Quantity);
            item.Quantity += resultQuantiy;
            //item.Quantity += data.MaxStack;//MaxStack��� ���� ������ (1~3)������ ������ ����.
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

        // ���� �ٶ󺸴� ���� (Y�� ����)
        float yRotation = playerController.transform.rotation.eulerAngles.y;
        Vector3 direction = (yRotation == 90f) ? Vector3.right : Vector3.left;

        rollTargetPos = rollStartPos + direction * playerModel.rollDistance;
        //float rollSpeed = 13f; // ������ �ӵ� 
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
            player.transform.position = rollTargetPos; // ���� ��ġ ����
        }
        else
        {
            float newX = Mathf.Lerp(rollStartPos.x, rollTargetPos.x, t);
            player.transform.position = new Vector3(newX, player.transform.position.y, player.transform.position.z);
        }
    }

    public void MovePlayer(Animator animator,GameObject player)   // �÷��̾� �̵��Լ�
    {
        float h = 0f;
        h = Input.GetAxisRaw("Horizontal");
        // rotate y ���� ������ 90 ���� 270   

        animator.SetFloat("Speed", Mathf.Abs(Input.GetAxis("Horizontal")));
        player.transform.position = new Vector3(player.transform.position.x + (h * playerModel.Speed * Time.deltaTime), player.transform.position.y, player.transform.position.z);
        if (h > 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 90, 0); // ������
            player.transform.rotation = Quaternion.Euler(0, 90, 0);
            playerModel.FacingDirection = 0; // ������
            playerModel.IsPlayerMoving = true; // �÷��̾ �����̰� ����
        }
        else if (h < 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 270, 0); // ����
            player.transform.rotation = Quaternion.Euler(0, 270, 0);
            playerModel.FacingDirection = 1; // ���� 
            playerModel.IsPlayerMoving = true; // �÷��̾ �����̰� ����
        }
        else
        {
                       playerModel.IsPlayerMoving = false; // �÷��̾ �������� ����
        }

  
    }
    public bool canSkill()
    {
        return playerModel.MP >= 5f;
    }

    public void UseSkill(Animator animator, Rigidbody rb, string type)
    {
        if(type == "ParryedAttack")
        {
            if (_parryDashCR != null) return; // �̹� ���� ���̸� ����

            // ���� �ڷ�ƾ
            MonoBehaviour mb = rb.GetComponent<MonoBehaviour>();
            if (mb != null)
            {
                _parryDashCR = mb.StartCoroutine(
                    ParryDoubleDash(animator, rb,
                        forwardForce: 45f,   // 1��/���� ��� ��
                        dashTime: 0.2f,     // �� �� ��� ���� �ð�
                        midPause: 0f      // �߰� ���� �ð�
                    )
                );
            }
            return;
        }
        if(type == "GuardAttackSkill")
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (mouseWorldPos.x > rb.transform.position.x)
            {
                // ������ �ٶ󺸱�
                rb.transform.rotation = Quaternion.Euler(0, 90, 0);
                playerModel.FacingDirection = 0;
            }
            else
            {
                // ���� �ٶ󺸱�
                rb.transform.rotation = Quaternion.Euler(0, 270, 0);
                playerModel.FacingDirection = 1;
            }
            animator.SetTrigger("GuardAttackSkill");
            float dashForce = 45f; // ��� ��  
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(rb.transform.forward * dashForce, ForceMode.VelocityChange);
            rb.useGravity = false; // �߷� ��Ȱ��ȭ
            MonoBehaviour mono = rb.GetComponent<MonoBehaviour>();
            if (mono != null)
            {
                mono.StartCoroutine(RestoreGravity(rb, 0.15f));
            }
        }

    }

    private IEnumerator ParryDoubleDash(Animator animator, Rigidbody rb,
                                       float forwardForce, float dashTime, float midPause)
    {
        Vector3 startPos = rb.position;
        float startYaw = rb.rotation.eulerAngles.y; // ������ ���� �������� �����ϰ� �ʹٸ� ���

        playerModel.HasParried = false;
        playerModel.IsAttacking = true;
        rb.useGravity = false;

        var ctrl = animator.GetComponent<PlayerController>();

        // 1) ���� ���
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(rb.transform.forward * forwardForce, ForceMode.VelocityChange);
        PlayerController playerController = animator.GetComponent<PlayerController>();
        playerController.attackSoundScript.PlaySfx("DashSkill");
        yield return new WaitForSeconds(dashTime);

        // ��� ����
        rb.linearVelocity = Vector3.zero;
        if (midPause > 0f) yield return new WaitForSeconds(midPause);
        playerController.attackSoundScript.PlaySfx("DashSkill");
        // ����Ʈ
        ctrl?.PlaySlashFX(13);

     
        Vector3 backDir = (startPos - rb.position).normalized;

     
        if (backDir.x >= 0f)
        {
            rb.transform.rotation = Quaternion.Euler(0f, 90f, 0f);   // ������
            playerModel.FacingDirection = 0;
        }
        else
        {
            rb.transform.rotation = Quaternion.Euler(0f, 270f, 0f);  // ����
            playerModel.FacingDirection = 1;
        }


        rb.AddForce(rb.transform.forward * forwardForce, ForceMode.VelocityChange);
        yield return new WaitForSeconds(dashTime);

       
        rb.linearVelocity = Vector3.zero;
        rb.position = new Vector3(startPos.x, rb.position.y, rb.position.z);
        rb.transform.rotation = Quaternion.Euler(0f, startYaw, 0f);   //

        rb.useGravity = true;
        playerModel.IsAttacking = false;
        _parryDashCR = null;
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
            Debug.Log($"����Ű����");
            playerModel.IsGrounded = false;

        }

        if (playerModel.IsAttacking)
        {

            Vector3 vel = rb.linearVelocity;
            vel.y = 0f; // Y �ӵ��� ���� (�߷� ����)
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
                playerModel.IsParrying = false; // �ݰ� ���� ����
                return "parry"; // ���� �߿� ���� ������ ������ �ݰ�
            }
            else { 
                return "block"; // ���� �߿��� �������� ���� ����
            }
        }
        if (playerModel.IsRolling)
        {
            return "roll"; // ������ �߿��� �������� ���� ����    
        }
        playerModel.Health -= dmg;
       // Debug.Log("������ ���� :" + playerModel.Health);
       playerModel.IsDamaged = true; // �������� �޾����� ǥ��
        playerModel.Animator.SetTrigger("TakeDamege");
        OnPropertyChanged("Health");
        // ���� �ð� �� false �� �ǵ���
        MonoBehaviour mono = playerModel.Animator.GetComponent<MonoBehaviour>();
        if (mono != null)
        {
            mono.StartCoroutine(ResetDamagedFlag(0.5f)); // 0.5�� �� ����
        }


        return "hit";
    }

    private IEnumerator ResetDamagedFlag(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerModel.IsDamaged = false;
    }


    public void CompleteGathering()
    {
        Debug.Log("�ڿ�ä�� �Ϸ�"+ AddItemData.ItemName);
        AddItem(AddItemData,3);
        playerModel.resourceObject.SetActive(false);
        
        playerModel.CompleteGathering();
        InventoryUpdate();
        OnPropertyChanged("CompleteGathering");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
   //     Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
