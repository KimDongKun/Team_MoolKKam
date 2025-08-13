using System.ComponentModel;
using UnityEngine;

public class BuildViewModel : INotifyPropertyChanged
{
    public TradeViewModel tradeViewModel { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"������Ƽ �̸� {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
