using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildController : MonoBehaviour
{
    public PlayerController player;
    public List<InventoryData> barricadeCostItems;
    public List<InventoryData> towerCostItems;
    private List<InventoryData> selectCostItem = new List<InventoryData>();

    public bool isBuilding = false;

    public GameObject buildUI;
    public Button barricadeButton;
    public Button towerButton;

    public GameObject barricadePrefab;
    public GameObject towerPrefab;

    private BuildModel useBuildModel;
    private BuildModel barricade;
    private BuildModel tower;

    
    private void Start()
    {
        barricade = new BuildModel(barricadePrefab, new Vector3(0, -0.4f,0),BuildModel.BuildType.Barricade);
        tower = new BuildModel(towerPrefab, new Vector3(0, 1.8f,0),BuildModel.BuildType.Tower);

        barricadeButton.onClick.AddListener(()=>BuildStartButton(barricade));
        towerButton.onClick.AddListener(() => BuildStartButton(tower));
    }

    public void BuildStartButton(BuildModel buildModel)
    {
        selectCostItem = new List<InventoryData>();
        if (buildModel.buildType == BuildModel.BuildType.Barricade)
        {
            selectCostItem = barricadeCostItems;
        }
        else if(buildModel.buildType == BuildModel.BuildType.Tower)
        {
            selectCostItem = towerCostItems;
        }
        
        TradeModel tradeModel = new TradeModel(player.playerModel, selectCostItem);
        TradeViewModel tradeViewModel = new TradeViewModel(tradeModel);

        isBuilding = tradeViewModel.TryTrade(player.playerModel.GetItemList, selectCostItem);
        if (isBuilding)
        {
            useBuildModel = buildModel;
            useBuildModel.BuildModelPrefab.SetActive(true);
            buildUI.SetActive(false);
        }
        
    }
    public void BuildClick()
    {
        isBuilding = false;
    }
    private void Update()
    {
        float mouse_X = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        if (isBuilding && mouse_X > -10f && mouse_X < 45)
        {
            
            useBuildModel.BuildModelPrefab.transform.position = new Vector3(mouse_X, useBuildModel.InstallPos.y, 0);

            if (Input.GetMouseButtonDown(0))
            {
                TradeModel tradeModel = new TradeModel(player.playerModel, selectCostItem);
                TradeViewModel tradeViewModel = new TradeViewModel(tradeModel);
                tradeViewModel.Trade(player.playerModel.GetItemList, selectCostItem);
                player.playerViewModel.InventoryUpdate();

                useBuildModel.BuildModelPrefab.SetActive(false);
                GameObject ins = Instantiate(useBuildModel.BuildModelPrefab, useBuildModel.BuildModelPrefab.transform.position, Quaternion.identity);
                ins.tag = "Building";
                ins.GetComponent<BuildObject>().isInstall = true;
                ins.SetActive(true);
                BuildClick();
            }
        }
    }
}
