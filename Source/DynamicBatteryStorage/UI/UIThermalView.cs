using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DynamicBatteryStorage;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class UIThermalView: UIView
  {

    bool overheating = false;
    #region GUI Strings

    string netHeatFlux = "";
    string vesselHeatStatus = "";

    string vesselHeatOk = "Vessel heat dissipation is sufficient";
    string vesselHeatNotOK = "<color=#fd6868> Vessel heat dissipation is insufficient, cores may overheat</color>";
    #endregion


    public UIThermalView(DynamicBatteryStorageUI uiHost):base(uiHost)
    {
      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in producerCats)
      {
        // Currently always generated with Show = false
        producerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width, heatFlowUnits));
      }
      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in consumerCats)
      {
        // Currently always generated with Show = false
        consumerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width, heatFlowUnits));
      }
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: [ThermalView]: New instance created");
    }

    /// <summary>
    /// Triggers on creation, localizes relevant strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();
      totalConsumptionHeader = Localizer.Format("#LOC_DynamicBatteryStorage_UI_TotalHeatConsumptionTitle");
      totalProductionHeader = Localizer.Format("#LOC_DynamicBatteryStorage_UI_TotalHeatGenerationTitle");
    }

    /// <summary>
    /// Draws the upper panel with flxues and battery states
    /// </summary>
    protected override void DrawUpperPanel()
    {
      GUILayout.BeginHorizontal(GUILayout.Height(80f));

      GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
      GUILayout.FlexibleSpace();
      Rect flowRect = GUILayoutUtility.GetRect(80f, 48f);
      UIUtils.IconDataField(flowRect, UIHost.GUIResources.GetIcon("fire"), netHeatFlux, UIHost.GUIResources.GetStyle("data_field_large"));
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.BeginVertical();
      GUILayout.FlexibleSpace();
      GUILayout.Label(vesselHeatStatus, UIHost.GUIResources.GetStyle("data_field_large"));
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();
      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the Detail area with info about detailed production and consumption per module
    /// </summary>
    protected override void DrawDetailPanel()
    {
      if (showDetails)
        UIHost.windowPos.height = 300f;
      else
        UIHost.windowPos.height = 200f;
      base.DrawDetailPanel();
    }

    /// <summary>
    /// Updates the data for drawing - strings and handler data caches
    /// </summary>
    public override void Update()
    {
      if (dataHost.ThermalData != null)
      {
        UpdateHeaderPanelData();
        UpdateDetailPanelData();
      }
    }

    /// <summary>
    /// Updates the header string data
    /// </summary>
    protected override void UpdateHeaderPanelData()
    {

      double netHeat = dataHost.ThermalData.CurrentConsumption + dataHost.ThermalData.CurrentProduction;

      if (netHeat == 0d)
      {
        overheating = false;
        netHeatFlux = String.Format("{0:F2} {1}", Math.Abs(netHeat), heatFlowUnits);
        vesselHeatStatus = vesselHeatOk;
      }
      else if (netHeat > 0d)
      {

        overheating = true;
        netHeatFlux = String.Format("<color=#fd6868> ▲ {0:F2} {1}</color>", Math.Abs(netHeat), heatFlowUnits);
        vesselHeatStatus = vesselHeatNotOK;
      }
      else
      {
        overheating = false;
        netHeatFlux = String.Format("▼ {0:F2} {1}", Math.Abs(netHeat), heatFlowUnits);
        vesselHeatStatus = vesselHeatOk;
      }

      totalConsumption = String.Format("▼ {0:F2} {1}",
        Math.Abs(dataHost.ThermalData.CurrentConsumption),
        heatFlowUnits);
      totalProduction = String.Format("▲ {0:F2} {1}",
        Math.Abs(dataHost.ThermalData.CurrentProduction),
        heatFlowUnits);

    }


    /// <summary>
    /// Updates the detail panel data - this is mostly rebuilding the handler list
    /// </summary>
    protected virtual void UpdateDetailPanelData()
    {
      // If no cached list, rebuild it from scratch
      if (cachedHandlers == null)
        RebuildCachedList(dataHost.ThermalData.AllHandlers);

      // If the list changed, rebuild it from components
      var firstNotSecond = dataHost.ThermalData.AllHandlers.Except(cachedHandlers).ToList();
      var secondNotFirst = cachedHandlers.Except(dataHost.ThermalData.AllHandlers).ToList();
      if ( firstNotSecond.Any() || secondNotFirst.Any())
      {
        if (Settings.DebugUIMode)
        {
          Utils.Log("[UI]: [ThermalView]: Cached handler list does not appear to match the current handler list");
        }
        RebuildCachedList(dataHost.ThermalData.AllHandlers);
      }
      else
      {
        // Just update if no changes
        for (int i = 0 ; i < categoryNames.Count ; i++)
        {
          producerCategoryUIItems[categoryNames[i]].Update(1.0f);
          consumerCategoryUIItems[categoryNames[i]].Update(1.0f);
        }
      }
    }

  }
}
