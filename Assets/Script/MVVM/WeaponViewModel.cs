using UnityEngine;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

public class WeaponViewModel : INotifyPropertyChanged
{
    private WeaponModel weaponModel;
    
    public void UpgradeWeapon()
    {
        weaponModel.UpgradeLevel+=1;
        weaponModel.Damage = 10 * weaponModel.UpgradeLevel;
    }
    public WeaponViewModel(WeaponModel model)
    {
        weaponModel = model;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
        Debug.Log($"프로퍼티 이름 {propertyName}");
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
