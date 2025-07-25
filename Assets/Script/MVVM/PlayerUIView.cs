using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIView : MonoBehaviour
{

    [SerializeField] public PlayerViewModel playerViewModel;

    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text stoneText;
    [SerializeField] TMP_Text ironText;
    [SerializeField] TMP_Text diamondText;

    [SerializeField] Slider hpSlider;
    [SerializeField] Slider gatheringSlider;

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
        else if (e.PropertyName == "CompleteGathering")
        {
           for(int i = 0; i<playerViewModel.itemList.Count; i++)
            {
                var name = playerViewModel.itemList[i].itemData.ItemName;
                if (name == "Gold")
                {
                    goldText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
                else if (name == "Log")
                {
                    logText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
                else if (name == "Stone")
                {
                    stoneText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
                else if (name == "Iron")
                {
                    ironText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
                else if (name == "Diamond")
                {
                    diamondText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
            }
        }

        UpdateUI();
    }
    private void UpdateUI()
    {
        //playerNameText.text = playerViewModel.Name;
        hpSlider.value = playerViewModel.Health;
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.E) && playerViewModel.IsGathering)
        {
            gatheringSlider.value += Time.deltaTime;
            gatheringSlider.gameObject.SetActive(true);

            if (gatheringSlider.value.Equals(gatheringSlider.maxValue))
            {
                playerViewModel.CompleteGathering();
            }
        }
        else
        {
            gatheringSlider.value = 0;
            gatheringSlider.gameObject.SetActive(false);
        }
    }
}
