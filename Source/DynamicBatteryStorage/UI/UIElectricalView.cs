using System;
using System.Collections.Generic;
using DynamicBatteryStorage;

namespace DynamicBatteryStorage.UI
{
  public class UIElectricalView
  {

    protected DynamicBatteryStorageUI host;
    List<ModuleDataHandler> cachedHandlers;

    Dictionary<string, List<ModuleDataHandler> handlerCats ;

    bool showDetails = false;

    // Positive power flow flag
    bool charging = false;

    List<UIExpandableItem> categoryUIItems;

    #region GUI Strings

    string totalPowerConsumption = "";
    string totalPowerProduction = "";
    string netPowerFlux = "";
    string availableBattery = "";
    string chargeTimeField = "";

    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="uiHost">The instance of the main UI class to link to </param>
    public UIElectricalView(DynamicBatteryStorageUI uiHost)
    {
      host = uiHost;
      if (Settings.DebugUIMode)
        Debug.Log("[UI]: [ElectricalView]: New instance created");
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
      GUILayout.Label(netPowerFlux, host.GUIResources.GetStyle("net_power_field"));
      GUILayout.EndVertical();

      GUILayout.BeginVertical();
      GUILayout.Label(availableBattery, host.GUIResources.GetStyle("data_field"));
      GUILayout.Label(chargeTimeField, host.GUIResources.GetStyle("data_field"));
      GUILayout.EndVertical();

      GUILayout.EndHorizontal();
    }

    /// <summary>
    /// Draws the Detail area with info about detailed production and consumption per module
    /// </summary>
    void DrawDetailPanel()
    {
      GUILayout.BeginHorizontal();

      if (GUILayout.Button(totalPowerProduction, host.GUIResources.GetStyle("produced_power_field")))
        showDetails = !showDetails;
      if (GUILayout.Button(totalPowerConsumption, host.GUIResources.GetStyle("consumed_power_field")))
        showDetails = !showDetails;

      GUILayout.EndHorizontal();

      if (showDetails)
      {

      }

    }


    /// <summary>
    /// Updates the data for drawing - strings and handler data caches
    /// </summary>
    public void Update()
    {
      if (uiHost.ElectricalData != null)
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
      double netPower = uiHost.ElectricalData.CurrentProduction - uiHost.ElectricalData.CurrentConsumption;
      double currentElectricity = uiHost.activeVessel.GetConnectedResourceTotals(PartResourceLibrary.ElectricityHashcode, out EC, out maxEC);

      if (netPower > 0d)
      {
        charging = true;
        netPowerFlux = String.Format("+ {0:F1} EC/s", netPower);
        if (maxEC - EC < 0.01d)
          chargeTimeField = "0 s";
        else
          chargeTimeField = String.Format("{0}", FormatUtils.FormatTimeString((maxEC - EC) / netPower));
      }
      else
      {
        charging = false;
        netPowerFlux = String.Format("- {0:F1} EC/s", netPower);
        if (EC < 0.01d)
          chargeTimeField = "0 s";
        else
          chargeTimeField = String.Format("{0}", FormatUtils.FormatTimeString(EC / netPower));
      }

      totalPowerConsumption = String.Format("- {0:F1}", uiHost.ElectricalData.CurrentConsumption);
      totalPowerProduction = String.Format("+ {0:F1}", uiHost.ElectricalData.CurrentProduction);
      availableBattery = String.Format("{0:F0} / {1:F0} ({2:F1}%)", EC, maxEC, EC/maxEC * 100d);
    }


    /// <summary>
    /// Updates the detail panel data - this is mostly rebuilding the handler list
    /// </summary>
    void UpdateDetailPanelData()
    {
        // If no cached list, rebuild it from scratch
        if (cachedHandlers == null)
          RebuildCachedList(uiHost.ElectricalData.handlers);

        // If the list changed, rebuild it from components
        var firstNotSecond = catHandlers.Except(cachedHandlers).ToList();
        var secondNotFirst = cachedHandlers.Except(catHandlers).ToList();
        if ( !firstNotSecond.Any() && !secondNotFirst.Any())
        {
          if (Settings.DebugUIMode)
            Debug.Log("[UI]: [ElectricalView]: Cached handler list does not appear to match the current handler list");
          RebuildCachedList(uiHost.ElectricalData.handlers);
        } else
        {
          // Just update if no changes
          for (int i = 0 ; i < categoryUIItems.Count ; i++)
          {
            categoryUIItems[i].Update();
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
      handlerCats = new Dictionary<string, List<ModuleDataHandler>();
      foreach(KeyValuePair<string, List<string> entry in HandlerCategories.HandlerCategoryMap)
      {
        handlerCats.Add(entry.value, new List<ModuleDataHandler()>);
      }

      // Sort through all handlers found and add to the appropriate category
      for (int i = 0; i < handlers.Count; i++)
      {
        foreach(KeyValuePair<string, List<string> entry in HandlerCategories.HandlerCategoryMap)
        {
          if ( entry.Value.Find(module => module == handlers[i].ModuleName()).Any() );
          {
            handlerCats[entry.Key].Add(handlers[i]);
            if (Settings.DebugUIMode)
              Debug.Log("[UI]: [ElectricalView]: Added {0} to category {1}", handlers[i].PartTitle(), entry.Key);
          }
        }
      }

      // Build all the new UI elements
      categoryUIItems = new List<UIExpandableItem>();
      foreach(KeyValuePair<string, List<ModuleDataHandler> entry in handlerCats)
      {
        // Currently always generated with Show = false
        categoryUIItems.Add(new UIExpandableItem(entry.Key, entry.Value, host, false));
      }

      // cache the list of handlers to detect changes
      cachedHandlers = new List<ModuleDataHandler>(handlers);
    }
  }
}
