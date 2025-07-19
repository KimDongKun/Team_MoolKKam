using System.ComponentModel;
using UnityEngine;

public class PlayerViewModel : INotifyPropertyChanged
{
    private PlayerModel playerModel;
    public string HealthText => $"HP: {playerModel.Health}";
    public Color HealthColor => playerModel.Health < 30 ? Color.red : Color.green;

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

    public void TakeDamage(int dmg)
    {
        playerModel.Health -= dmg;
        Debug.Log("������ ���� :" + playerModel.Health);
        OnPropertyChanged("Health");
    }
    public void CompleteGathering()
    {
        Debug.Log("�ڿ�ä�� �Ϸ�");
        playerModel.resourceObject.SetActive(false);
        playerModel.CompleteGathering();
        
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
