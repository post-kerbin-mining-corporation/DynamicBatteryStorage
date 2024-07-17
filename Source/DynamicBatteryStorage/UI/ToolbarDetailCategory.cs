using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class ToolbarDetailCategory : MonoBehaviour
  {
    public bool Visible { get { return visible; } }
    private RectTransform rect;
    private Text categoryName;
    private Text categoryValue;
    private Text expandCarat;
    private Button expandButton;
    private RectTransform expandCaratRect;
    private Transform categoryArea;
    private GameObject categoryAreaObj;

    private string powerFlowUnits = "";
    private bool visible = false;
    private bool showParts = false;

    private ToolbarDetailPanel panel;
    private List<ModuleDataHandler> partHandlers;
    private List<ToolbarDetailPart> partWidgets;

    public void Awake()
    {
      partHandlers = new List<ModuleDataHandler>();
      partWidgets = new List<ToolbarDetailPart>();
      FindComponents();
    }

    void FindComponents()
    {
      // Find all the components
      rect = this.GetComponent<RectTransform>();
      categoryName = Utils.FindChildOfType<Text>("CategoryName", transform);
      categoryValue = Utils.FindChildOfType<Text>("CategoryValue", transform);
      expandCarat = Utils.FindChildOfType<Text>("ExpandIcon", transform);
      expandCaratRect = expandCarat.GetComponent<RectTransform>();
      expandButton = Utils.FindChildOfType<Button>("CategoryHeader", transform);
      categoryArea = transform.FindDeepChild("CategoryBody");
      categoryAreaObj = categoryArea.gameObject;
      expandButton.onClick.AddListener(delegate { TogglePartList(); });

      powerFlowUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalFlowUnits");
    }
    protected void TogglePartList()
    {
      showParts = !showParts;
      categoryAreaObj.SetActive(showParts);
      for (int i = 0; i < partWidgets.Count; i++)
      {
        partWidgets[i].PartVisible = showParts;
      }
      if (showParts)
      {
        expandCarat.text = "▼";
      }
      else
      {
        expandCarat.text = "▶";
      }
      panel.RecalculatePanelPositionData();
    }
    public float GetHeight()
    {
      float totalHeight = 0f;
      if (!visible)
      {
        return totalHeight;
      }
      totalHeight += 22.7f;
      if (showParts)
      {
        totalHeight += (partHandlers.Count() * 30.73f);
      }
      return totalHeight;
    }
    public void SetVisible(bool on)
    {
      visible = on;
      gameObject.SetActive(visible);
    }

    public void SetCategory(UIHandlerCategory associatedCategory)
    {
      categoryName.text = associatedCategory.title;
      expandCarat.text = "▶";
    }
    public void RefreshWithNewHandlers(List<ModuleDataHandler> handlers, ToolbarDetailPanel hostPanel)
    {
      panel = hostPanel;
      partHandlers = handlers;
      SetVisible(partHandlers.Count > 0);
      DestroyPartWidgets();
      CreatePartWidgets();
      panel.RecalculatePanelPositionData();
    }
    private void DestroyPartWidgets()
    {
      if (partWidgets != null && partWidgets.Count > 0)
      {
        for (int i = 0; i < partWidgets.Count; i++)
        {
          GameObject.Destroy(partWidgets[i].gameObject);
        }
      }
      partWidgets.Clear();
    }
    private void CreatePartWidgets()
    {
      partWidgets = new List<ToolbarDetailPart>();
      for (int i = 0; i < partHandlers.Count; i++)
      {
        Utils.Log("[UI]: Generating a new category widget", Utils.LogType.UI);
        GameObject newObj = (GameObject)GameObject.Instantiate(SystemsMonitorAssets.PartItemPrefab, Vector3.zero, Quaternion.identity);
        newObj.transform.SetParent(categoryArea, false);

        ToolbarDetailPart newWidget = newObj.AddComponent<ToolbarDetailPart>();
        newWidget.SetPart(partHandlers[i]);
        newWidget.PartVisible = showParts;

        partWidgets.Add(newWidget);

      }
    }

    public void Update()
    {
      if (visible)
      {
        double totalValue = 0f;
        for (int i = 0; i < partHandlers.Count; i++)
        {
          if (partHandlers[i].Simulated)
            totalValue += partHandlers[i].GetValue();
        }
        if (totalValue > 0f)
        {
          categoryValue.text = String.Format("▲ {0:F2} {1}", Math.Abs(totalValue), powerFlowUnits);
        }
        else if (totalValue < 0f)
        {
          categoryValue.text = String.Format("▼ {0:F2} {1}", Math.Abs(totalValue), powerFlowUnits);
        }
        else
        {
          categoryValue.text = String.Format("0.0 {0}", powerFlowUnits);
        }
      }
    }
  }
}
