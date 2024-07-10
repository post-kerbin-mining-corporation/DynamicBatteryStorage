using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{
  class UIDBSManager:UIWidget
  {
    UIElectricalView electricalView;
    ModuleDynamicBatteryStorage controller;
    Vessel activeVessel;
    int partCount = -1;

    // UI strings
    string panelName = "";
    string powerUnits= "";
    string systemStatusTitle = "System Offline";
    string bufferSizeTitle = "";
    string bufferPartTitle = "";
    string debugTitle = "";
    string savedVesselECTitle = "";
    string savedECTitle = "";
    string errorNoController = "";
    string errorTimewarp = "";
    string errorNone = "";
    string currentBufferSize = "";
    string bufferPart = "";
    string savedEC = "";
    string savedVesselEC = "";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="uiHost">The parent UI</param>
    /// <param name="view">The parent ElectricalView panel</param>
    public UIDBSManager(DynamicBatteryStorageUI uiHost, UIElectricalView view): base (uiHost)
    {
      electricalView = view;

      if (Settings.DebugUIMode)
        Utils.Log(String.Format("[UI DBS Manager] Created"));
    }

    /// <summary>
    /// Do localization of UI strings
    /// </summary>
    protected override void Localize()
    {
      base.Localize();
      panelName = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSManagerTitle");
      systemStatusTitle = "System Offline";
      bufferSizeTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSManagerBufferSize"); 
      bufferPartTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSManagerBufferPart");
      debugTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSDebugTitle");
      savedVesselECTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSVesselCachedTitle"); 
      savedECTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSPartCachedTitle");

      errorNoController = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSErrorController");
      errorTimewarp = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSErrorTimewarp", Settings.TimeWarpLimit);

      errorNone = Localizer.Format("#LOC_DynamicBatteryStorage_UI_DBSNoError");

      currentBufferSize = "";
      bufferPart = "";
      savedEC = "";
      powerUnits = Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalUnits");
    }

    /// <summary>
    /// Draw method
    /// </summary>
    public void Draw()
    {
      GUILayout.BeginVertical(UIHost.GUIResources.GetStyle("block_background"));

      GUILayout.Label(panelName, UIHost.GUIResources.GetStyle("panel_header_centered"));

      if (controller == null)
      {
        GUILayout.FlexibleSpace();
        GUILayout.Label(systemStatusTitle, UIHost.GUIResources.GetStyle("panel_header_centered"));
        GUILayout.FlexibleSpace();
      }
      else
      {
        if (!controller.AnalyticMode)
        {
          GUILayout.FlexibleSpace();
          GUILayout.Label(systemStatusTitle, UIHost.GUIResources.GetStyle("panel_header_centered"));
          GUILayout.FlexibleSpace();
        } else
        {

          GUILayout.Space(5);
          GUILayout.Label(systemStatusTitle, UIHost.GUIResources.GetStyle("panel_header_centered"));
          GUILayout.BeginHorizontal();
          GUILayout.Label(bufferSizeTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(190f));
          GUILayout.Label(currentBufferSize, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(80f));
          GUILayout.EndHorizontal();

          GUILayout.BeginHorizontal();
          GUILayout.Label(bufferPartTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(190f));
          GUILayout.Label(bufferPart, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(80f));
          GUILayout.EndHorizontal();

          GUILayout.Space(2);
          GUILayout.Label(debugTitle, UIHost.GUIResources.GetStyle("panel_header_centered"));
          GUILayout.BeginHorizontal();
          GUILayout.Label(savedECTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(190f));
          GUILayout.Label(savedEC, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(100f));
          GUILayout.EndHorizontal();
          GUILayout.BeginHorizontal();
          GUILayout.Label(savedVesselECTitle, UIHost.GUIResources.GetStyle("data_header"), GUILayout.MaxWidth(190f));
          GUILayout.Label(savedVesselEC, UIHost.GUIResources.GetStyle("data_field"), GUILayout.MinWidth(100f));
          GUILayout.EndHorizontal();
        }
      }
      GUILayout.EndVertical();
    }

    /// <summary>
    /// Update UI fields and recalculate quantities
    /// </summary>>
    public void Update()
    {
      if (controller == null)
      {
        systemStatusTitle = errorNoController;

        if (FlightGlobals.ActiveVessel != null)
        {
          if (activeVessel != null)
          {
            if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
              FindController();
          }
          else
          {
            FindController();
          }
        }
        if (activeVessel != null)
        {
          if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
            FindController();
        }
      }
      else
      {
        if (!controller.AnalyticMode)
        {
          systemStatusTitle = errorTimewarp;
        } else if (controller.BufferPart == null)
        {
          systemStatusTitle = errorNoController;
        }
        else
        {
          systemStatusTitle = errorNone;
          currentBufferSize = String.Format("{0:F2} {1}", controller.BufferSize, powerUnits);
          bufferPart = String.Format("{0}", controller.BufferPart.partInfo.title);
          savedEC = String.Format("{0:F1} {1} : {2:F1} {3}", controller.MaxEC, powerUnits, controller.SavedMaxEC, powerUnits);
          savedVesselEC = String.Format("{0:F1} {1}", controller.SavedVesselMaxEC, powerUnits);
        }
      }
    }
    /// <summary>
    /// Finds the main DBS controller
    /// </summary>
    public void FindController()
    {
      activeVessel = FlightGlobals.ActiveVessel;
      partCount = activeVessel.parts.Count;

      controller = activeVessel.GetComponent<ModuleDynamicBatteryStorage>();
    }
  }
}
