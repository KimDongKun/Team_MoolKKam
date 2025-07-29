using System.ComponentModel;
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

    [SerializeField] Slider hpSlider;
    [SerializeField] Slider gatheringSlider;

    [Header("NPC UI")]
    [SerializeField] GameObject npcUI;
    [SerializeField] Button npcNextTextButton;
    [SerializeField] RawImage npcImage;
    [SerializeField] TMP_Text npcName;
    [SerializeField] TMP_Text npcTalk;

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

        npcNextTextButton.onClick.RemoveAllListeners();
        npcNextTextButton.onClick.AddListener(npcViewModel.ShowNextDialogue);
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
   
    public void UpdateUI()
    {
        hpSlider.value = playerViewModel.Health;
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
            npcViewModel.ShowNextDialogue();
            npcImage.texture = npcViewModel.Image;
            npcName.text = npcViewModel.Name;
            npcUI.SetActive(true);
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && playerViewModel.IsNpcMeeting)
        {
            npcUI.SetActive(false);
            // playerViewModel.
        }
    }
}
