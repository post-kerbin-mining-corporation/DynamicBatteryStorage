using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KSP.UI;
using KSP.UI.Screens;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class ToolbarPanel : MonoBehaviour
  {
    public float SimSituationAltitude
    {
      get { return situationUI.SimSituationAltitude; }
    }

    public void SetToolbarPosition(Vector2 newPos, Vector2 pivot)
    {
      rect.pivot = pivot;
      rect.position = newPos;
    }
    public ToolbarDetailPanel DetailUI
    {
      get { return detailUI; }
    }
    public ToolbarPower PowerUI
    {
      get { return powerUI; }
    }
    protected VesselElectricalData electricalData;

    protected ToolbarSituation situationUI;
    protected ToolbarPower powerUI;
    protected ToolbarDetailPanel detailUI;
    protected ToolbarAdvanced advancedUI;

    protected bool active = true;
    protected bool panelOpen = false;
    protected RectTransform rect;

    protected Text panelTitle;

    public void Awake()
    {
      // Find all the components
      rect = this.GetComponent<RectTransform>();
      panelTitle = Utils.FindChildOfType<Text>("PanelTitleText", transform);

      // Loop Panel
      detailUI = gameObject.AddComponent<ToolbarDetailPanel>();
      detailUI.Initialize(transform);

      // Craft Stats
      powerUI = new ToolbarPower();
      powerUI.Initialize(transform, detailUI);

      // Situation
      situationUI = new ToolbarSituation();
      situationUI.Initialize(transform, this);

      // Advanced
      advancedUI = new ToolbarAdvanced();
      advancedUI.Initialize(transform, this);

      Localize();
    }
    void Localize()
    {
      panelTitle.text = Localizer.Format("#LOC_DynamicBatteryStorage_ToolbarPanel_Title");
    }

    protected void Update()
    {
      if (electricalData != null)
      {
        if (powerUI != null)
        {
          powerUI.Update(electricalData);
        }
        if (detailUI != null)
        {
          detailUI.UpdateData(electricalData);
        }
        if (advancedUI != null)
        {
          advancedUI.Update(electricalData);
        }
      }
    }

    public void UpdateSolarHandlerData()
    {
      if (powerUI != null)
      {
        powerUI.UpdateSolarFields(situationUI.SimSituationPanelScale, situationUI.SimSituationEclipseTime);
      }
      if (electricalData != null)
      {
        for (int i = 0; i < electricalData.AllHandlers.Count; i++)
        {
          if (electricalData.AllHandlers[i].AffectedBySunDistance)
          {
            electricalData.AllHandlers[i].SolarEfficiency = situationUI.SimSituationPanelScale;
          }
        }
      }
    }
    public void SetVariableHandlers(bool state)
    {
      if (electricalData != null)
      {
        for (int i = 0; i < electricalData.AllHandlers.Count; i++)
        {
          if (!electricalData.AllHandlers[i].TimewarpFunctional)
          {
            electricalData.AllHandlers[i].Simulated = state;
          }
        }
      }
    }
    public void SetConstantHandlers(bool state)
    {
      if (electricalData != null)
      {
        for (int i = 0; i < electricalData.AllHandlers.Count; i++)
        {
          if (electricalData.AllHandlers[i].TimewarpFunctional)
          {
            electricalData.AllHandlers[i].Simulated = state;
          }
        }
      }
    }

    public void SetVisible(bool state)
    {
      active = state;
      rect.gameObject.SetActive(state);
    }

    public void SetElectricalData(VesselElectricalData data)
    {
      electricalData = data;
    }
  }
}
