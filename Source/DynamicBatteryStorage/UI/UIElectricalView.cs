using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
  public class UIElectricalView: UIWidget
  {

    DynamicBatteryStorageUI dataHost;

    List<ModuleDataHandler> cachedHandlers;

    Dictionary<string, List<ModuleDataHandler>> producerCats;
    Dictionary<string, List<ModuleDataHandler>> consumerCats ;

    bool showDetails = false;

    // Positive power flow flag
    bool charging = false;

    List<UIExpandableItem> producerCategoryUIItems;
    List<UIExpandableItem> consumerCategoryUIItems;

    #region GUI Strings

    string totalPowerConsumptionHeader = "";
    string totalPowerProductionHeader = "";
    string totalPowerConsumption = "0 EC/s";
    string totalPowerProduction = "0 EC/s";

    string powerFlowUnits = "";
    string powerUnits = "";
    string timeUnits = "";

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
      dataHost = uiHost;
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: [ElectricalView]: New instance created");
    }

    /// <summary>
    /// Triggers on creation, localizes relevant strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();
      totalPowerConsumptionHeader = "Total Power Consumption";
      totalPowerProductionHeader = "Total Power Generation";
      powerFlowUnits = "EC/s";
      powerUnits = "EC";
      timeUnits = "s";
    }

    /// <summary>
    /// Main UI method, should do all drawing
    /// </summary>
    public void Draw()
    {
      DrawUpperPanel();
      DrawDetailPanel();
    }

    /// <summary>
    /// Draws the upper panel with flxues and battery states
    /// </summary>
    void DrawUpperPanel()
    {
      GUILayout.BeginHorizontal();

      GUILayout.BeginVertical();
      Rect flowRect = GUILayoutUtility.GetRect(60f, 32f);
      UIUtils.IconDataField(flowRect, UIHost.GUIResources.GetIcon("lightning"), netPowerFlux, UIHost.GUIResources.GetStyle("data_field"));
      GUILayout.EndVertical();

      GUILayout.BeginVertical();

      Rect batteryRect = GUILayoutUtility.GetRect(60, 32f);
      UIUtils.IconDataField(batteryRect, UIHost.GUIResources.GetIcon("battery"), availableBattery, UIHost.GUIResources.GetStyle("data_field"));
      Rect chargeRect = GUILayoutUtility.GetRect(60, 32f);
      UIUtils.IconDataField(chargeRect, UIHost.GUIResources.GetIcon("timer"), chargeTime, UIHost.GUIResources.GetStyle("data_field"));

      GUILayout.EndVertical();

      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the Detail area with info about detailed production and consumption per module
    /// </summary>
    void DrawDetailPanel()
    {
      GUILayout.BeginHorizontal();

      if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("positive_button")))
        showDetails = !showDetails;

      Rect overlayRect = GUILayoutUtility.GetLastRect();
      
      GUI.Label(overlayRect, totalPowerProductionHeader, UIHost.GUIResources.GetStyle("positive_category_header"));
      GUI.Label(overlayRect, totalPowerProduction, UIHost.GUIResources.GetStyle("positive_category_header_field"));

      if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("negative_button")))
        showDetails = !showDetails;

      overlayRect = GUILayoutUtility.GetLastRect();

      
      GUI.Label(overlayRect, totalPowerConsumptionHeader, UIHost.GUIResources.GetStyle("negative_category_header"));
      GUI.Label(overlayRect, totalPowerConsumption, UIHost.GUIResources.GetStyle("negative_category_header_field"));

      GUILayout.EndHorizontal();

      if (showDetails)
      {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0 ; i < producerCategoryUIItems.Count ; i++)
        {
          producerCategoryUIItems[i].Draw();
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical();
        for (int i = 0 ; i < consumerCategoryUIItems.Count ; i++)
        {
          consumerCategoryUIItems[i].Draw();
        }
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
      }

    }

    /// <summary>
    /// Updates the data for drawing - strings and handler data caches
    /// </summary>
    public void Update()
    {
      if (dataHost.ElectricalData != null)
      {
        UpdateHeaderPanelData();
        UpdateDetailPanelData();
      }
    }

    /// <summary>
    /// Updates the header string data
    /// </summary>
    void UpdateHeaderPanelData()
    {
      double EC = 0d;
      double maxEC = 0d;
      double netPower = dataHost.ElectricalData.CurrentProduction + dataHost.ElectricalData.CurrentConsumption;
      dataHost.ElectricalData.GetElectricalChargeLevels(out EC, out maxEC);

      if (netPower == 0d)
      {
        charging = false;
        netPowerFlux = String.Format("{0:F1} {1}", netPower, powerFlowUnits);
        chargeTime = String.Format("{0}", "Never");
      }
      else if (netPower > 0d)
      {
        charging = true;
        netPowerFlux = String.Format("▲ {0:F1} {1}", netPower, powerFlowUnits);

        if (maxEC - EC < 0.01d)
          chargeTime = "0 s";
        else
          chargeTime = String.Format("{0} s", FormatUtils.FormatTimeString((maxEC - EC) / netPower));
      }
      else
      {
        charging = false;
        netPowerFlux = String.Format("▼ {0:F1} {1}", netPower, powerFlowUnits);
        if (EC < 0.01d)
          chargeTime = "0 s";
        else
          chargeTime = String.Format("{0} s", FormatUtils.FormatTimeString(EC / netPower));
      }

      totalPowerConsumption = String.Format("▼ {0:F1} {1}", 
        dataHost.ElectricalData.CurrentConsumption,
        powerFlowUnits);
      totalPowerProduction = String.Format("▲ {0:F1} {1}", 
        dataHost.ElectricalData.CurrentProduction,
        powerFlowUnits);
      availableBattery = String.Format("{0:F0} / {1:F0} ({2:F1}%)", EC, maxEC, EC/maxEC * 100d);
    }
    

    /// <summary>
    /// Updates the detail panel data - this is mostly rebuilding the handler list
    /// </summary>
    void UpdateDetailPanelData()
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
        for (int i = 0 ; i < producerCategoryUIItems.Count ; i++)
        {
          producerCategoryUIItems[i].Update();
        }
        for (int i = 0 ; i < consumerCategoryUIItems.Count ; i++)
        {
          consumerCategoryUIItems[i].Update();
        }
      }
    }

    /// <summary>
    /// Rebuild the set of data handlers that will be drawn
    /// </summary>
    /// <param name="handlers">The list of data handlers to build the draw list from</param>
    void RebuildCachedList(List<ModuleDataHandler> handlers)
    {
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: [ElectricalView]: Rebuilding electrical handler category map");

      // Rebuild the blank dictionary
      consumerCats = new Dictionary<string, List<ModuleDataHandler>>();
      producerCats = new Dictionary<string, List<ModuleDataHandler>>();
      foreach(KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
      {
        consumerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
        producerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
      }

      // Sort through all handlers found and add to the appropriate category
      for (int i = 0; i < handlers.Count; i++)
      {
        foreach(KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
        {
          if (categoryEntry.Value.FindAll(module => module == handlers[i].ModuleName()).Count > 0)
          {
            if (handlers[i].IsProducer())
              producerCats[categoryEntry.Key].Add(handlers[i]);
            else
              consumerCats[categoryEntry.Key].Add(handlers[i]);
            if (Settings.DebugUIMode)
              Utils.Log(String.Format("[UI]: [ElectricalView]: Added {0} (Producer = {1}) to category {2}", 
                handlers[i].PartTitle(), handlers[i].IsProducer(), categoryEntry.Key));
          }
        }
      }
      // Build all the new UI elements
      producerCategoryUIItems = new List<UIExpandableItem>();
      consumerCategoryUIItems = new List<UIExpandableItem>();
      foreach(KeyValuePair<string, List<ModuleDataHandler>> entry in producerCats)
      {
        // Currently always generated with Show = false
        producerCategoryUIItems.Add(new UIExpandableItem(entry.Key, entry.Value, dataHost, false));
      }
      foreach(KeyValuePair<string, List<ModuleDataHandler>> entry in consumerCats)
      {
        // Currently always generated with Show = false
        consumerCategoryUIItems.Add(new UIExpandableItem(entry.Key, entry.Value, dataHost, false));
      }
      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }
  }
}
