using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : MonoBehaviour
{

    [SerializeField] PlayerViewModel playerViewModel;

    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text stoneText;
    [SerializeField] TMP_Text ironText;
    [SerializeField] TMP_Text diamondText;

    [SerializeField] Slider hpSlider;

    public void Init(PlayerViewModel vm)
    {
        playerViewModel = vm;
        //playerViewModel = new PlayerViewModel();
        UpdateUI();
        playerViewModel.PropertyChanged += OnPlayerViewModelPropertyChanged;
    }
    private void OnPlayerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // 변경된 속성에 따라 UI 업데이트
        if (e.PropertyName == "Name")
        {
            //playerNameText.text = playerViewModel.Name;
        }
        else if (e.PropertyName == "Health")
        {
            hpSlider.value = playerViewModel.Health;
        }
    }
    private void UpdateUI()
    {
        //playerNameText.text = playerViewModel.Name;
        hpSlider.value = playerViewModel.Health;
    }
}
