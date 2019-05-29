using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DynamicBatteryStorage;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  public class UIElectricalView: UIView
  {


    public float SolarSimulationScalar
    {
      get { return solarSimulationScalar; }
      set { solarSimulationScalar = value; }
    }

    // Positive power flow flag
    bool charging = false;
    float solarSimulationScalar;
    UISolarPanelManager solarManager;

    #region GUI Strings

    string batteryStatusHeader = "";
    string batteryChargeDepleted = "";
    string batteryChargeStable = "";
    string batteryChargeFullCharge = "";
    string batteryChargeCharging = "";

    string netPowerFlux = "0";
    string availableBattery = "100";
    string chargeTime = "";

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="uiHost">The instance of the main UI class to link to </param>
    public UIElectricalView(DynamicBatteryStorageUI uiHost):base(uiHost)
    {

      if (HighLogic.LoadedSceneIsEditor)
        solarManager = new UISolarPanelManager(uiHost, this);

      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in producerCats)
      {
        // Currently always generated with Show = false
        producerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width, powerFlowUnits));
      }
      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in consumerCats)
      {
        // Currently always generated with Show = false
        consumerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width, powerFlowUnits));
      }

      if (Settings.DebugUIMode)
        Utils.Log("[UI]: [ElectricalView]: New instance created");
    }

    /// <summary>
    /// Triggers on creation, localizes relevant strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();
      totalConsumptionHeader = Localizer.Format("LOC_DynamicBatteryStorage_UI_TotalPowerConsumptionTitle");
      totalProductionHeader = Localizer.Format("LOC_DynamicBatteryStorage_UI_TotalPowerGenerationTitle");
      batteryStatusHeader = Localizer.Format("LOC_DynamicBatteryStorage_UI_BatteryManagerTitle");
      batteryChargeDepleted = Localizer.Format("#LOC_DynamicBatteryStorage_UI_BatteryTimeDepleted");
      batteryChargeStable = Localizer.Format("#LOC_DynamicBatteryStorage_UI_BatteryTimeStable");
      batteryChargeCharging = Localizer.Format("#LOC_DynamicBatteryStorage_UI_BatteryTimeCharging");
      batteryChargeFullCharge = Localizer.Format("#LOC_DynamicBatteryStorage_UI_BatteryTimeFullCharge");
    }

    /// <summary>
    /// Draws the upper panel with flxues and battery states
    /// </summary>
    protected override void DrawUpperPanel()
    {
      GUILayout.BeginHorizontal(GUILayout.Height(180f));

      GUILayout.BeginVertical(GUILayout.MaxWidth(150f));
      GUILayout.FlexibleSpace();
      Rect flowRect = GUILayoutUtility.GetRect(80f, 48f);
      UIUtils.IconDataField(flowRect, UIHost.GUIResources.GetIcon("lightning"), netPowerFlux, UIHost.GUIResources.GetStyle("data_field_large"));
      GUILayout.FlexibleSpace();
      GUILayout.EndVertical();

      GUILayout.BeginVertical(GUILayout.MaxWidth(200f));
      GUILayout.FlexibleSpace();
      GUILayout.BeginVertical(UIHost.GUIResources.GetStyle("block_background"));
      GUILayout.Label(batteryStatusHeader, UIHost.GUIResources.GetStyle("panel_header_centered"));
      Rect batteryRect = GUILayoutUtility.GetRect(80f, 32f);
      UIUtils.IconDataField(batteryRect, UIHost.GUIResources.GetIcon("battery"), availableBattery, UIHost.GUIResources.GetStyle("data_field"));
      Rect chargeRect = GUILayoutUtility.GetRect(80f, 32f);
      UIUtils.IconDataField(chargeRect, UIHost.GUIResources.GetIcon("timer"), chargeTime, UIHost.GUIResources.GetStyle("data_field"));
      GUILayout.EndVertical();
      GUILayout.FlexibleSpace();

      GUILayout.EndVertical();

      if (HighLogic.LoadedSceneIsEditor)
        solarManager.Draw();

      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the Detail area with info about detailed production and consumption per module
    /// </summary>
    protected override void DrawDetailPanel()
    {
      base.DrawDetailPanel();
    }

    /// <summary>
    /// Updates the data for drawing - strings and handler data caches
    /// </summary>
    protected override void Update()
    {
      base.Update();
      if (dataHost.ElectricalData != null && HighLogic.LoadedSceneIsEditor)
        solarManager.Update();
    }

    /// <summary>
    /// Updates the header string data
    /// </summary>
    protected override UpdateHeaderPanelData()
    {
      double EC = 0d;
      double maxEC = 0d;
      double netPower = dataHost.ElectricalData.CurrentConsumption + dataHost.ElectricalData.GetSimulatedElectricalProdution(solarSimulationScalar);

      dataHost.ElectricalData.GetElectricalChargeLevels(out EC, out maxEC);

      if (netPower == 0d)
      {
        charging = false;
        netPowerFlux = String.Format("{0:F2} {1}", netPower, powerFlowUnits);
        chargeTime = String.Format("{0}", batteryChargeStable);
      }
      else if (netPower > 0d)
      {

        charging = true;
        netPowerFlux = String.Format("▲ {0:F2} {1}", netPower, powerFlowUnits);
        if (HighLogic.LoadedSceneIsFlight)
        {
          if (maxEC - EC < 0.01d)
            chargeTime = "0 s";
          else
            chargeTime = String.Format("{0} {1}", batteryChargeCharging, FormatUtils.FormatTimeString((maxEC - EC) / netPower));
        } else
        {
          chargeTime = String.Format("{0} {1}", batteryChargeFullCharge, FormatUtils.FormatTimeString(maxEC / netPower));
        }
      }
      else
      {
        charging = false;
        netPowerFlux = String.Format("<color=red> ▼ {0:F2} {1}</color>", netPower, powerFlowUnits);
        if (EC < 0.01d)
          chargeTime = "0 s";
        else
          chargeTime = String.Format("{0} in {1}", batteryChargeDepleted, FormatUtils.FormatTimeString(EC / netPower));
      }

      totalConsumption = String.Format("▼ {0:F2} {1}",
        dataHost.ElectricalData.CurrentConsumption,
        powerFlowUnits);
      totalProduction = String.Format("▲ {0:F2} {1}",
        dataHost.ElectricalData.GetSimulatedElectricalProdution(solarSimulationScalar),
        powerFlowUnits);
      availableBattery = String.Format("{0:F0} / {1:F0} ({2:F1}%)", EC, maxEC, EC/maxEC * 100d);
    }


    /// <summary>
    /// Updates the detail panel data - this is mostly rebuilding the handler list
    /// </summary>
    protected virtual void UpdateDetailPanelData()
    {
      // If no cached list, rebuild it from scratch
      if (cachedHandlers == null)
        RebuildCachedList(dataHost.ElectricalData.AllHandlers);

      // If the list changed, rebuild it from components
      var firstNotSecond = dataHost.ElectricalData.AllHandlers.Except(cachedHandlers).ToList();
      var secondNotFirst = cachedHandlers.Except(dataHost.ElectricalData.AllHandlers).ToList();
      if ( firstNotSecond.Any() || secondNotFirst.Any())
      {
        if (Settings.DebugUIMode)
        {
          Utils.Log("[UI]: [ElectricalView]: Cached handler list does not appear to match the current handler list");
        }
        RebuildCachedList(dataHost.ElectricalData.AllHandlers);
      }
      else
      {
        // Just update if no changes
        for (int i = 0 ; i < categoryNames.Count ; i++)
        {
          producerCategoryUIItems[categoryNames[i]].Update(solarSimulationScalar);
          consumerCategoryUIItems[categoryNames[i]].Update(solarSimulationScalar);
        }
      }
    }
  }
}
