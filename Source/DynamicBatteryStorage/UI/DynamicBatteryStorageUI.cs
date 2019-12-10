using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.UI.Screens;
using KSP.Localization;

namespace DynamicBatteryStorage.UI
{

  /// <summary>
  /// Mode the window is in
  /// </summary>
  enum WindowMode
  {
      Thermal, Electrical
  }

  [KSPAddon(KSPAddon.Startup.FlightAndEditor, false)]
  public class DynamicBatteryStorageUI : UIBaseWindow
  {

    #region GUI Variables
    private string windowTitle = "";

    int modeFlag = 0;
    string[] modeStrings = new string[] { "ELECTRICAL", "THERMAL"};
    static WindowMode windowMode = WindowMode.Electrical;
    #endregion

    #region GUI Widgets
    private UIElectricalView electricalView;
    private UIThermalView thermalView;
    #endregion

    #region Vessel Data
    private Vessel activeVessel;
    private VesselDataManager vesselData;
    private EditorVesselDataManager editorVesselData;
    private int partCount = 0;
    #endregion

    public VesselThermalData ThermalData
    {
      get
      {
        if (HighLogic.LoadedSceneIsFlight)
          return vesselData.HeatData;
        else
          return editorVesselData.HeatData;
      }
    }

    public VesselElectricalData ElectricalData
    {
      get {
        if (HighLogic.LoadedSceneIsFlight)
          return vesselData.ElectricalData;
        else
          if (editorVesselData)
            return editorVesselData.ElectricalData;
        return null;
       }
    }

    /// <summary>
    /// Find the vessel data manager in flight or editor
    /// </summary>
    public void FindData()
    {
      if (HighLogic.LoadedSceneIsFlight)
      {
        activeVessel = FlightGlobals.ActiveVessel;
        vesselData = activeVessel.GetComponent<VesselDataManager>();
        partCount = activeVessel.Parts.Count;
        if (Settings.DebugUIMode)
          Utils.Log("[UI]: Located Flight data");
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        editorVesselData = EditorVesselDataManager.Instance;
        if (Settings.DebugUIMode)
          Utils.Log("[UI]: Located Editor data");
      }
    }

    /// <summary>
    /// Initialize the UI widgets, do localization, set up styles
    /// </summary>
    protected override void InitUI()
    {
      windowTitle = Localizer.Format("#LOC_DynamicBatteryStorage_UI_WindowName");
      modeStrings = new string[] { Localizer.Format("#LOC_DynamicBatteryStorage_UI_ElectricalModeName"), Localizer.Format("#LOC_DynamicBatteryStorage_UI_ThermalModeName")};

      thermalView = new UIThermalView(this);
      electricalView = new UIElectricalView(this);

      base.InitUI();
    }


    protected override void Start()
    {
      base.Start();

      if (HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor)
        FindData();
    }

    /// <summary>
    /// Draw the UI
    /// </summary>
    protected override void Draw()
    {
      // Fallback to try to get data if we don't have any
      if (HighLogic.LoadedSceneIsFlight)
      {
        if (FlightGlobals.ActiveVessel != null)
        {
          if (vesselData == null)
            FindData();
        }
      }
      if (HighLogic.LoadedSceneIsEditor)
      {
        if (editorVesselData == null)
          FindData();
      }
      base.Draw();
    }


    /// <summary>
    /// Draw the window
    /// </summary>
    /// <param name="windowId">window ID</param>
    protected override void DrawWindow(int windowId)
    {
      // Draw the header/tab controls
      DrawHeaderArea();

      if ((HighLogic.LoadedSceneIsFlight && vesselData != null) ||
        (HighLogic.LoadedSceneIsEditor && editorVesselData != null))
      {
        GUILayout.Space(3f);
        switch (modeFlag)
        {
          case 0:
            electricalView.Draw();
            break;
          case 1:
            thermalView.Draw();
            break;
        }
      }
      else
      {
        GUILayout.Label("No Vessel Located");
      }
      GUI.DragWindow();
    }


    /// <summary>
    /// Draw the header area
    /// </summary>
    private void DrawHeaderArea()
    {
      GUILayout.BeginHorizontal();
      modeFlag = GUILayout.SelectionGrid(modeFlag, modeStrings, 2, GUIResources.GetStyle("radio_text_button"));

      GUILayout.FlexibleSpace();
      GUILayout.Label(windowTitle, GUIResources.GetStyle("window_header"), GUILayout.MaxHeight(26f), GUILayout.MinHeight(26f), GUILayout.MinWidth(350f));

      GUILayout.FlexibleSpace();
      Rect buttonRect = GUILayoutUtility.GetRect(22f, 22f);
      GUI.color = resources.GetColor("cancel_color");
      if (GUI.Button(buttonRect, "", GUIResources.GetStyle("button_cancel")))
      {
        ToggleWindow();
      }

      GUI.DrawTextureWithTexCoords(buttonRect, GUIResources.GetIcon("cancel").iconAtlas, GUIResources.GetIcon("cancel").iconRect);
      GUI.color = Color.white;
      GUILayout.EndHorizontal();
    }

    int ticker = 0;
    void Update()
    {
      // Perform updates of the two views
      if (showWindow && (vesselData != null && activeVessel != null) || editorVesselData != null )
      {
        if (ticker >= Settings.UIUpdateInterval)
        {
          ticker = 0;
          switch (modeFlag)
          {
            case 0:
              if (electricalView != null)
                electricalView.Update();
              break;
            case 1:
              if (thermalView != null)
                thermalView.Update();
              break;
          }
        }
        ticker += 1;
      }

      if (HighLogic.LoadedSceneIsFlight)
      {
        // Handle refresh when switching ships
        if (FlightGlobals.ActiveVessel != null)
        {
          if (activeVessel != null)
          {
            if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
            {
                ResetAppLauncher();
            }
          }
          else
          {
            ResetAppLauncher();
          }
        }
        if (activeVessel != null)
        {
          if (partCount != activeVessel.parts.Count || activeVessel != FlightGlobals.ActiveVessel)
          {
            ResetAppLauncher();
          }
        }
      }
    }


    void ResetAppLauncher()
    {
      if (!Settings.Enabled) return;
      if (Settings.DebugUIMode)
        Utils.Log("[UI]: Reset App Launcher");
      FindData();
      if (stockToolbarButton == null)
      {
        stockToolbarButton = ApplicationLauncher.Instance.AddModApplication(
            OnToolbarButtonToggle,
            OnToolbarButtonToggle,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            DummyVoid,
            ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH | ApplicationLauncher.AppScenes.FLIGHT,
            (Texture)GameDatabase.Instance.GetTexture(toolbarUIIconURLOff, false));
      }

    }

  }


}
