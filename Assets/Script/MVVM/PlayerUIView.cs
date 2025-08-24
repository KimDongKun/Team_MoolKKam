using System;
using System.ComponentModel;
using System.Linq;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class PlayerUIView : MonoBehaviour
{

    [SerializeField] public PlayerViewModel playerViewModel;
    [SerializeField] public NPCViewModel npcViewModel;

    [SerializeField] TMP_Text goldText;
    [SerializeField] TMP_Text logText;
    [SerializeField] TMP_Text stoneText;
    [SerializeField] TMP_Text ironText;
    [SerializeField] TMP_Text diamondText;
    [SerializeField] TMP_Text potionText;

    [SerializeField] Slider hpSlider;
    [SerializeField] Slider MpSlider;
    [SerializeField] TMP_Text PlayerHP;
    [SerializeField] TMP_Text PlayerMP;

    [SerializeField] Slider gatheringSlider;

    [Header("NPC UI")]
    [SerializeField] GameObject npcUI;
    [SerializeField] Button npcNextTextButton;
    [SerializeField] RawImage npcImage;
    [SerializeField] TMP_Text npcName;
    [SerializeField] TMP_Text npcTalk;

    [Header("Upgrade UI")]
    [SerializeField] GameObject upgradeUI;
    [SerializeField] GameObject upgradeEffectUI;
    [SerializeField] TMP_Text weaponLevel;
    [SerializeField] TMP_Text Gold;
    [SerializeField] TMP_Text firstVaule;
    [SerializeField] TMP_Text secondVaule;
    [SerializeField] TMP_Text thirdVaule;
    [SerializeField] TMP_Text forthVaule;
    [SerializeField] Button UpgradeButton;

    [Header("Trader UI")]
    [SerializeField] GameObject tradeUI;
    [SerializeField] Button RandomPickButton;
    [SerializeField] RandomEnhanceStonePickController randomPickController;

    [Header("Build UI")]
    [SerializeField] BuildController buildController;

    

    public void Init(PlayerViewModel vm)
    {
        playerViewModel = vm;
        UpdateUI();
        playerViewModel.PropertyChanged += OnPlayerViewModelPropertyChanged;
    }
    public void OnNPCPropertyChanged(PlayerViewModel playerViewModel)
    {
        npcViewModel = new NPCViewModel(playerViewModel.npcModel);
        npcViewModel.OnDialogueUpdate += UpdateDialogue;

        Debug.Log("NPC PropertyChanged 구독 시작");
        npcNextTextButton.onClick.RemoveAllListeners();
        npcNextTextButton.onClick.AddListener(npcViewModel.ShowNextDialogue);
    }
    public void OnPlayerViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
       // Debug.Log(e.PropertyName);
        // 변경된 속성에 따라 UI 업데이트
        if (e.PropertyName == "Name")
        {
            //goldText.text = playerViewModel.Name;
            Debug.Log(playerViewModel.Name);
        }
        else if (e.PropertyName == "Health")
        {
            PlayerHP.text = $"{playerViewModel.Health}/200";
            hpSlider.value = playerViewModel.Health;
            //  Debug.Log(playerViewModel.Health);
        }
        else if(e.PropertyName == "Mp"){
            //   Debug.Log("MP 변경됨: " + playerViewModel.Mp);
            PlayerMP.text = $"{playerViewModel.Mp}/50";
            MpSlider.value = playerViewModel.Mp;
        }
        else if (e.PropertyName == "TakeDamage")
        {
            hpSlider.value = playerViewModel.Health;
        }
        else if (e.PropertyName == "CompleteGathering" || e.PropertyName == "InventoryUpdate")
        {
            Debug.Log("호출된 프로퍼티:" + e.PropertyName);
            for (int i = 0; i < playerViewModel.itemList.Count; i++)
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
                else if(name == "Potion")
                {
                    Debug.Log("포션찾음");
                    potionText.text = playerViewModel.itemList[i].Quantity.ToString();
                }
            }
        }
        UpdateUI();
    }
   
    public void UpdateUI()
    {
        hpSlider.value = playerViewModel.Health;
        MpSlider.value = playerViewModel.Mp;

        PlayerHP.text = $"{playerViewModel.Health}/200";
        PlayerMP.text = $"{playerViewModel.Mp}/50";
        if (npcViewModel != null)
        {
            SetCostData();
        }
        
    }
    void UpdateDialogue(string dialogue)
    {
        npcTalk.text = dialogue;
        if (!npcViewModel.isTrigger)
        {
            npcViewModel.dialogueIndex = 0;
            npcUI.SetActive(false);
        }
    }
    void SetCostData()
    {
        Debug.Log("현재 레벨" + playerViewModel.WeaponLevel);
        weaponLevel.text = playerViewModel.WeaponLevel;

        var gold = playerViewModel.itemList.FirstOrDefault(item => item.itemData.ItemID == "0");
        var first = playerViewModel.itemList.FirstOrDefault(item => item.itemData.ItemID == "01");
        var second = playerViewModel.itemList.FirstOrDefault(item => item.itemData.ItemID == "02");
        var third = playerViewModel.itemList.FirstOrDefault(item => item.itemData.ItemID == "03");
        var forth = playerViewModel.itemList.FirstOrDefault(item => item.itemData.ItemID == "04");

        int goldQuantity = gold != null ? gold.Quantity : 0;
        int firstQuantity = first != null ? first.Quantity : 0;
        int secondQuantity = second != null ? second.Quantity : 0;
        int thirdQuantity = third != null ? third.Quantity : 0;
        int forthQuantity = forth != null ? forth.Quantity : 0;


        //Gold.text = $"{goldQuantity.ToString()}/{npcViewModel.costGold.ToString()}";
        firstVaule.text = $"{firstQuantity.ToString()}/{npcViewModel.costFirst.ToString()}";
        secondVaule.text = $"{secondQuantity.ToString()}/{npcViewModel.costSecond.ToString()}";
        thirdVaule.text = $"{thirdQuantity.ToString()}/{npcViewModel.costThird.ToString()}";
        forthVaule.text = $"{forthQuantity.ToString()}/{npcViewModel.costFourth.ToString()}";
    }
    public void TradeButtonAddListener()
    {
        randomPickController.RandomStonePickList(playerViewModel);
        playerViewModel.InventoryUpdate();
    }
    public void UpgradeButtonAddListener()
    {
        playerViewModel.UpgradeWeaponPlayer();
        upgradeEffectUI.SetActive(false);
        upgradeEffectUI.SetActive(true);
        playerViewModel.InventoryUpdate();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (playerViewModel.IsGathering)
            {
                gatheringSlider.value += Time.deltaTime;
                gatheringSlider.gameObject.SetActive(true);

                if (gatheringSlider.value.Equals(gatheringSlider.maxValue))
                {
                    playerViewModel.CompleteGathering();
                }
            }
        }
        else
        {
            gatheringSlider.value = 0;
            gatheringSlider.gameObject.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.E) && playerViewModel.IsNpcMeeting)
        {
           
            OnNPCPropertyChanged(playerViewModel);
            npcImage.texture = npcViewModel.Image;
            npcName.text = npcViewModel.Name;

            if (npcViewModel.isNPCTrader)
            {
                Debug.Log("NPC : 거래시작");
                npcViewModel.tradeUI.SetActive(true);
                npcViewModel.ShowTradeUI();
                SetButtonData();
                SetCostData();
            }
            else
            {
                npcUI.SetActive(true);
                npcViewModel.ShowNextDialogue();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (playerViewModel.Health < 200)
            {
                playerViewModel.HealingPotion(20);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            npcUI.SetActive(false);
            upgradeUI.gameObject.SetActive(false);
            tradeUI.gameObject.SetActive(false);
            buildController.buildUI.SetActive(false);
            // playerViewModel.
        }
    }

    public void EscapeUi()
    {
        npcUI.SetActive(false);
    }

    void SetButtonData()
    {
        TradeViewModel tradeViewModel = new TradeViewModel(npcViewModel.tradeModel);
        UpgradeButton.onClick.RemoveAllListeners();
        RandomPickButton.onClick.RemoveAllListeners();
        var costItemList = tradeViewModel.TryTrade(playerViewModel.itemList, npcViewModel.tradeModel.costitemList);
        if (costItemList)
        {
            Debug.Log("거래조건 충족함.");
            UpgradeButton.onClick.AddListener(() => tradeViewModel.Trade(playerViewModel.itemList, npcViewModel.tradeModel.costitemList));
            UpgradeButton.onClick.AddListener(() => UpgradeButtonAddListener());
            UpgradeButton.onClick.AddListener(() => SetButtonData());

            RandomPickButton.onClick.AddListener(() => tradeViewModel.Trade(playerViewModel.itemList, npcViewModel.tradeModel.costitemList));
            RandomPickButton.onClick.AddListener(() => TradeButtonAddListener());
            RandomPickButton.onClick.AddListener(() => SetButtonData());
        }
        else
        {
            Debug.Log("거래조건 충족하지못함.");
        }
    }
}
