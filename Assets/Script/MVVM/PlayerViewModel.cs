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
    public string HealthText => $"HP: {playerModel.Health}";
    public Color HealthColor => playerModel.Health < 30 ? Color.red : Color.green;

    public ItemData AddItemData => playerModel.CurrentItem;
    public List<InventoryData> itemList => playerModel.GetItemList;
    public bool IsGathering => playerModel.IsGathering;
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
    public void AddItem(ItemData data)
    {
        var item = itemList.FirstOrDefault(i => i.itemData.ItemName == data.ItemName);
        if (item != null)
        {
            item.Quantity += data.MaxStack;//MaxStack��� ���� ������ (1~3)������ ������ ����.
        }
        else
        {
            
            itemList.Add(new InventoryData(data, data.MaxStack));
        }
    }

    public void RollPlayer(Animator animator , Rigidbody rb)
    {
        float h = Input.GetAxisRaw("Horizontal");
        animator.SetTrigger("Roll");
        if (h > 0)
        {
            rb.linearVelocity += (Vector3.right * 8f);

        }
        else if(h<0)
        {
            rb.linearVelocity += (Vector3.left * 8f);
        }
        playerModel.IsRolling = true;

    }

    public void MovePlayer(Animator animator,GameObject player)   // �÷��̾� �̵��Լ�
    {
        float h = 0f;
        h = Input.GetAxisRaw("Horizontal");
        // rotate y ���� ������ 90 ���� 270   

        animator.SetFloat("Speed", Mathf.Abs(h));
        player.transform.position = new Vector3(player.transform.position.x + (h * playerModel.Speed * Time.deltaTime), player.transform.position.y, player.transform.position.z);
        if (h > 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 130, 0); // ������
            playerModel.FacingDirection = 0; // ������
            playerModel.IsPlayerMoving = true; // �÷��̾ �����̰� ����
        }
        else if (h < 0)
        {
            playerModel.TargetRotation = Quaternion.Euler(0, 230, 0); // ����
            playerModel.FacingDirection = 1; // ���� 
            playerModel.IsPlayerMoving = true; // �÷��̾ �����̰� ����
        }
        else
        {
                       playerModel.IsPlayerMoving = false; // �÷��̾ �������� ����
        }

            player.transform.rotation = Quaternion.Lerp(player.transform.rotation, playerModel.TargetRotation, Time.deltaTime * playerModel.RotationSpeed);
        Vector3 fixedEuler = player.transform.rotation.eulerAngles;
        fixedEuler.x = 0f;
        fixedEuler.z = 0f;
        player.transform.rotation = Quaternion.Euler(fixedEuler);
        playerModel.TargetRotation = player.transform.rotation;
    }
    public void JumpPlayer(bool space, Animator animator,Rigidbody rb)
    {
        float hight = rb.linearVelocity.y;
        animator.SetFloat("Hight", hight);
        animator.SetBool("Ground", playerModel.IsGrounded);
        if (space && playerModel.IsGrounded && !playerModel.IsAttacking && !playerModel.IsRolling)
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * playerModel.JumpForce, ForceMode.Impulse);
            //   rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            Debug.Log($"����Ű����");
            playerModel.IsGrounded = false;

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
    public void TakeDamage(int dmg)
    {
        playerModel.Health -= dmg;
        Debug.Log("������ ���� :" + playerModel.Health);
        OnPropertyChanged("Health");
    }
    public void CompleteGathering()
    {
        Debug.Log("�ڿ�ä�� �Ϸ�"+ AddItemData.ItemName);
        AddItem(AddItemData);
        playerModel.resourceObject.SetActive(false);
        
        playerModel.CompleteGathering();
        OnPropertyChanged("CompleteGathering");
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
