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
    public void OnPlayerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Debug.Log(e.PropertyName);
        // 변경된 속성에 따라 UI 업데이트
        if (e.PropertyName == "Name")
        {
            goldText.text = playerViewModel.Name;
            Debug.Log(playerViewModel.Name);
        }
        else if (e.PropertyName == "Health")
        {
            hpSlider.value = playerViewModel.Health;
            Debug.Log(playerViewModel.Health);
        }
        else if(e.PropertyName == "TakeDamage")
        {
            hpSlider.value = playerViewModel.Health;
        }

        UpdateUI();
    }
    private void UpdateUI()
    {
        //playerNameText.text = playerViewModel.Name;
        hpSlider.value = playerViewModel.Health;
        Debug.Log("hp 슬라이드:"+ hpSlider.value);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            playerViewModel.TakeDamage(1);
            Debug.Log("스페이스바 입력 " + playerViewModel.Health);
        }
    }
}
