using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private PrefabsSO prefabs;
    [SerializeField] private SettingsSO settings;
    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI txtPower;
    private int ResourceAmount;
    [Header("Buttons")]
    [SerializeField] private Image imgPowerPlantSelected;
    [SerializeField] private Image imgBarracksSelected;
    [Space]
    [SerializeField] private TextMeshProUGUI txtPowerPlantPrice;
    [SerializeField] private TextMeshProUGUI txtBarracksPrice;
    [Header("Information")]
    [SerializeField] private GameObject PanelBarracks;
    [SerializeField] private GameObject PanelPowerPlant;
    [SerializeField] private GameObject PanelSelectedUnits;
    [Header("SelectedUnits")]
    [SerializeField] private TextMeshProUGUI txtSelectedCount;
    [SerializeField] private Transform SelectUnitArea;
    private List<SelectedUnitUI> SelectedUnitList = new List<SelectedUnitUI>();

    public static UIManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
        LoadResources();
        SetButtons();
        CreateSelectUnitList();
    }

    private void Start()
    {
        ClosePanels();
    }

    private void CreateSelectUnitList()
    {
        for (int i = 0; i < settings.MaxSelectUnitCount; i++)
        {
            SelectedUnitList.Add(Instantiate(prefabs.SelectUnitUI, SelectUnitArea).GetComponent<SelectedUnitUI>());
        }
    }

    private void SetButtons()
    {
        txtPowerPlantPrice.text = "$" + settings.PowerPlantBuildPrice.ToString();
        txtBarracksPrice.text = "$" + settings.BarracksBuildPrice.ToString();
    }

    public void AddResource(int amount)
    {
        ResourceAmount += amount;
        txtPower.text = ResourceAmount.ToString();
        PlayerPrefs.SetInt("ResourceAmount", ResourceAmount);
    }

    public bool SpendResource(int amount)
    {
        if (ResourceAmount >= amount)
        {
            ResourceAmount -= amount;
            txtPower.text = ResourceAmount.ToString();
            PlayerPrefs.SetInt("ResourceAmount", ResourceAmount);
            return true;
        }
        return false;
    }

    private void LoadResources()
    {
        ResourceAmount = PlayerPrefs.GetInt("ResourceAmount", 50);
        txtPower.text = ResourceAmount.ToString();
    }

    public void btn_PowerPlant()
    {
        if (ResourceAmount >= settings.PowerPlantBuildPrice)
        {
            imgPowerPlantSelected.enabled = true;
            imgBarracksSelected.enabled = false;
            GridBuildingSystem.Instance.SelectToBuild(BuildingType.PowerPlant);
            ClosePanels();
        }
    }

    public void btn_Barracks()
    {
        if (ResourceAmount >= settings.BarracksBuildPrice)
        {
            imgPowerPlantSelected.enabled = false;
            imgBarracksSelected.enabled = true;
            GridBuildingSystem.Instance.SelectToBuild(BuildingType.Barracks);
            ClosePanels();
        }
    }

    public void UnSelectBuildingType()
    {
        imgPowerPlantSelected.enabled = false;
        imgBarracksSelected.enabled = false;
    }

    public void SelectBarracks()
    {
        GridBuildingSystem.Instance.HideFlag();
        PanelPowerPlant.SetActive(false);
        PanelBarracks.SetActive(true);
        PanelSelectedUnits.SetActive(false);
    }

    public void SelectPowerPlant()
    {
        GridBuildingSystem.Instance.HideFlag();
        PanelBarracks.SetActive(false);
        PanelPowerPlant.SetActive(true);
        PanelSelectedUnits.SetActive(false);
    }

    public void SelectUnits()
    {
        GridBuildingSystem.Instance.HideFlag();
        PanelBarracks.SetActive(false);
        PanelPowerPlant.SetActive(false);
        PanelSelectedUnits.SetActive(true);
    }

    public SelectedUnitUI SelectUnit(int id)
    {
        txtSelectedCount.text = id.ToString();
        return SelectedUnitList[id - 1];
    }

    private void ClosePanels()
    {
        GridBuildingSystem.Instance.HideFlag();
        PanelBarracks.SetActive(false);
        PanelPowerPlant.SetActive(false);
        PanelSelectedUnits.SetActive(false);
    }
}
