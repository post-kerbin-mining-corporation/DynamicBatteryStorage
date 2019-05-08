using System;
using System.Collections.Generic;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
  public class UIElectricalView: UIWidget
  {

    List<ModuleDataHandler> cachedHandlers;

    Dictionary<string, List<ModuleDataHandler> producerCats ;
    Dictionary<string, List<ModuleDataHandler> consumerCats ;

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
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: [ElectricalView]: New instance created");
    }

    /// <summary>
    /// Triggers on creation, localizes relevant strings
    /// </summary>
    protected virtual void Localize()
    {
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
      Rect flowRect = GUILayoutUtility.GetRect(200f, 64f);
      UIUtils.IconDataField(flowRect, UIHost.GUIResources.GetIcon("lightning"), netPowerFlux, UIHost.GUIResources.GetStyle("data_field"));
      GUILayout.EndVertical();

      GUILayout.BeginVertical();

      Rect batteryRect = GUILayoutUtility.GetRect(200f, 64f);
      UIUtils.IconDataField(batteryRect, UIHost.GUIResources.GetIcon("battery"), availableBattery, UIHost.GUIResources.GetStyle("data_field"));
      Rect chargeRect = GUILayoutUtility.GetRect(200f, 64f);
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

      if (GUILayout.Button("", UIHost.GUIResources.GetStyle("positive_button")))
        showDetails = !showDetails;

      GUI.Label( GUILayoutUtility.GetLastRect(), totalPowerProductionHeader, UIHost.GUIResources.GetStyle("positive_category_header"));
      GUI.Label( GUILayoutUtility.GetLastRect(), totalPowerProduction, UIHost.GUIResources.GetStyle("positive_category_header_field"));

      if (GUILayout.Button("", UIHost.GUIResources.GetStyle("negative_button")))
        showDetails = !showDetails;

      GUI.Label( GUILayoutUtility.GetLastRect(), totalPowerConsumptionHeader, UIHost.GUIResources.GetStyle("negative_category_header"));
      GUI.Label( GUILayoutUtility.GetLastRect(), totalPowerConsumption, UIHost.GUIResources.GetStyle("negative_category_header_field"));

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
        GUILayout.EndnHorizontal();
      }

    }


    /// <summary>
    /// Updates the data for drawing - strings and handler data caches
    /// </summary>
    public void Update()
    {
      if (uiUIHost.ElectricalData != null)
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
      double maxEC = 0d;
      double EC = 0d;
      double netPower = uiUIHost.ElectricalData.CurrentProduction - uiUIHost.ElectricalData.CurrentConsumption;
      double currentElectricity = uiUIHost.activeVessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out EC, out maxEC);

      if (netPower > 0d)
      {
        charging = true;
        netPowerFlux = String.Format("▲ {0:F1} {1}", netPower, powerFlowUnits);
        if (maxEC - EC < 0.01d)
          chargeTimeField = "0 s";
        else
          chargeTimeField = String.Format("{0}", FormatUtils.FormatTimeString((maxEC - EC) / netPower));
      }
      else
      {
        charging = false;
        netPowerFlux = String.Format("▼ {0:F1} {1}", netPower, powerFlowUnits);
        if (EC < 0.01d)
          chargeTimeField = "0 s";
        else
          chargeTimeField = String.Format("{0}", FormatUtils.FormatTimeString(EC / netPower));
      }

      totalPowerConsumption = String.Format("▼ {0:F1}", uiUIHost.ElectricalData.CurrentConsumption);
      totalPowerProduction = String.Format("▲ {0:F1}", uiUIHost.ElectricalData.CurrentProduction);
      availableBattery = String.Format("{0:F0} / {1:F0} ({2:F1}%)", EC, maxEC, EC/maxEC * 100d);
    }


    /// <summary>
    /// Updates the detail panel data - this is mostly rebuilding the handler list
    /// </summary>
    void UpdateDetailPanelData()
    {
        // If no cached list, rebuild it from scratch
        if (cachedHandlers == null)
          RebuildCachedList(uiUIHost.ElectricalData.handlers);

        // If the list changed, rebuild it from components
        var firstNotSecond = catHandlers.Except(cachedHandlers).ToList();
        var secondNotFirst = cachedHandlers.Except(catHandlers).ToList();
        if ( !firstNotSecond.Any() && !secondNotFirst.Any())
        {
          if (Settings.DebugUIMode)
            Debug.Log("[UI]: [ElectricalView]: Cached handler list does not appear to match the current handler list");
          RebuildCachedList(uiUIHost.ElectricalData.handlers);
        } else
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
        Debug.Log("[UI]: [ElectricalView]: Rebuilding electrical handler category map");

      // Rebuild the blank dictionary
      consumerCats = new Dictionary<string, List<ModuleDataHandler>();
      producerCats = new Dictionary<string, List<ModuleDataHandler>();
      foreach(KeyValuePair<string, List<string> entry in HandlerCategories.HandlerCategoryMap)
      {
        consumerCats.Add(entry.value, new List<ModuleDataHandler()>);
        producerCats.Add(entry.value, new List<ModuleDataHandler()>);
      }

      // Sort through all handlers found and add to the appropriate category
      for (int i = 0; i < handlers.Count; i++)
      {
        foreach(KeyValuePair<string, List<string> entry in HandlerCategories.HandlerCategoryMap)
        {
          if ( entry.Value.Find(module => module == handlers[i].ModuleName()).Any() );
          {
            if (handlers[i].IsProducer())
              producerCats[entry.Key].Add(handlers[i]);
            else
              consumerCats[entry.Key].Add(handlers[i]);


            if (Settings.DebugUIMode)
              Debug.Log("[UI]: [ElectricalView]: Added {0} (Producer = {1}) to category {2}", handlers[i].PartTitle(), handlers[i].IsProducer(), entry.Key);
          }
        }
      }

      // Build all the new UI elements
      producerCategoryUIItems = new List<UIExpandableItem>();
      consumerCategoryUIItems = new List<UIExpandableItem>();
      foreach(KeyValuePair<string, List<ModuleDataHandler> entry in producerCats)
      {
        // Currently always generated with Show = false
        producerCategoryUIItems.Add(new UIExpandableItem(entry.Key, entry.Value, host, false));
      }
      foreach(KeyValuePair<string, List<ModuleDataHandler> entry in consumerCats)
      {
        // Currently always generated with Show = false
        consumerCategoryUIItems.Add(new UIExpandableItem(entry.Key, entry.Value, host, false));
      }

      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }
  }
}
