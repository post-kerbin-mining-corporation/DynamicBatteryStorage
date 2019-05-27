using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
  public class UIElectricalView: UIWidget
  {


    public float SolarSimulationScalar
    {
      get { return solarSimulationScalar; }
      set { solarSimulationScalar = value; }
    }
    DynamicBatteryStorageUI dataHost;

    List<ModuleDataHandler> cachedHandlers;

    List<string> categoryNames;

    Dictionary<string, List<ModuleDataHandler>> producerCats;
    Dictionary<string, List<ModuleDataHandler>> consumerCats;
    
    bool showDetails = false;

    // Positive power flow flag
    bool charging = false;
    float solarSimulationScalar;
    UISolarPanelManager solarManager;


    float col_width = 280f;
    Dictionary<string, UIExpandableItem> producerCategoryUIItems;
    Dictionary<string, UIExpandableItem> consumerCategoryUIItems;

    #region GUI Strings

    string batteryStatusHeader = "";
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
      col_width = (uiHost.WindowPosition.width -20f)/ 2f;
      if (HighLogic.LoadedSceneIsEditor)
        solarManager = new UISolarPanelManager(uiHost, this);

      // Build all of the base data and UI components
      producerCategoryUIItems = new Dictionary<string, UIExpandableItem>();
      consumerCategoryUIItems = new Dictionary<string, UIExpandableItem>();

      consumerCats = new Dictionary<string, List<ModuleDataHandler>>();
      producerCats = new Dictionary<string, List<ModuleDataHandler>>();
      categoryNames = HandlerCategories.HandlerCategoryMap.Keys.ToList();

      foreach (KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
      {
        consumerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
        producerCats.Add(categoryEntry.Key, new List<ModuleDataHandler>());
      }
      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in producerCats)
      {
        // Currently always generated with Show = false
        producerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width));
      }
      foreach (KeyValuePair<string, List<ModuleDataHandler>> entry in consumerCats)
      {
        // Currently always generated with Show = false
        consumerCategoryUIItems.Add(entry.Key, new UIExpandableItem(entry.Key, entry.Value, dataHost, false, col_width));
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
      totalPowerConsumptionHeader = "Total Power Consumption";
      totalPowerProductionHeader = "Total Power Generation";
      batteryStatusHeader = "Battery Status";
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
    void DrawDetailPanel()
    {
      if (showDetails)
        UIHost.windowPos.height = 500f;
      else
        UIHost.windowPos.height = 270f;
      GUILayout.BeginHorizontal();

      if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("positive_button"), GUILayout.Width(col_width)))
        showDetails = !showDetails;

      Rect overlayRect = GUILayoutUtility.GetLastRect();
      
      GUI.Label(overlayRect, totalPowerProductionHeader, UIHost.GUIResources.GetStyle("positive_category_header"));
      GUI.Label(overlayRect, totalPowerProduction, UIHost.GUIResources.GetStyle("positive_category_header_field"));

      if (GUILayout.Button(" ", UIHost.GUIResources.GetStyle("negative_button"), GUILayout.Width(col_width)))
        showDetails = !showDetails;

      overlayRect = GUILayoutUtility.GetLastRect();

      
      GUI.Label(overlayRect, totalPowerConsumptionHeader, UIHost.GUIResources.GetStyle("negative_category_header"));
      GUI.Label(overlayRect, totalPowerConsumption, UIHost.GUIResources.GetStyle("negative_category_header_field"));

      GUILayout.EndHorizontal();

      if (showDetails)
      {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(GUILayout.Width(col_width));
        GUILayout.Space(1f);
        for (int i = 0 ; i < categoryNames.Count ; i++)
        {
          producerCategoryUIItems[categoryNames[i]].Draw();
         
        }
        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(col_width));
        GUILayout.Space(1f);
        for (int i = 0 ; i < categoryNames.Count ; i++)
        {
          consumerCategoryUIItems[categoryNames[i]].Draw();
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
        if (HighLogic.LoadedSceneIsEditor)
          solarManager.Update();
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
      double netPower = dataHost.ElectricalData.CurrentConsumption + dataHost.ElectricalData.GetSimulatedElectricalProdution(solarSimulationScalar);
      dataHost.ElectricalData.GetElectricalChargeLevels(out EC, out maxEC);

      if (netPower == 0d)
      {
        charging = false;
        netPowerFlux = String.Format("{0:F1} {1}", netPower, powerFlowUnits);
        chargeTime = String.Format("{0}", "Stable");
      }
      else if (netPower > 0d)
      {
        
        charging = true;
        netPowerFlux = String.Format("▲ {0:F1} {1}", netPower, powerFlowUnits);
        if (HighLogic.LoadedSceneIsFlight)
        {
          if (maxEC - EC < 0.01d)
            chargeTime = "0 s";
          else
            chargeTime = String.Format("Charged in {0}", FormatUtils.FormatTimeString((maxEC - EC) / netPower));
        } else
        {
          chargeTime = String.Format("Full recharge in {0}", FormatUtils.FormatTimeString(maxEC / netPower));
        }
      }
      else
      {
        charging = false;
        netPowerFlux = String.Format("<color=red> ▼ {0:F1} {1}</color>", netPower, powerFlowUnits);
        if (EC < 0.01d)
          chargeTime = "0 s";
        else
          chargeTime = String.Format("Depletion in {0}", FormatUtils.FormatTimeString(EC / netPower));
      }

      totalPowerConsumption = String.Format("▼ {0:F1} {1}", 
        dataHost.ElectricalData.CurrentConsumption,
        powerFlowUnits);
      totalPowerProduction = String.Format("▲ {0:F1} {1}", 
        dataHost.ElectricalData.GetSimulatedElectricalProdution(solarSimulationScalar),
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
        for (int i = 0 ; i < categoryNames.Count ; i++)
        {
          producerCategoryUIItems[categoryNames[i]].Update(solarSimulationScalar);
          consumerCategoryUIItems[categoryNames[i]].Update(solarSimulationScalar);
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

      foreach (KeyValuePair<string, List<string>> categoryEntry in HandlerCategories.HandlerCategoryMap)
      {
        consumerCats[categoryEntry.Key] = new List<ModuleDataHandler>();
        producerCats[categoryEntry.Key] = new List<ModuleDataHandler>();
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

      for (int i = 0; i < categoryNames.Count; i++)
      {
        producerCategoryUIItems[categoryNames[i]].RefreshHandlers(producerCats[categoryNames[i]]);
        consumerCategoryUIItems[categoryNames[i]].RefreshHandlers(consumerCats[categoryNames[i]]);
      }


      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }
  }
}
